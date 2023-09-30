using CSLibrary;
using CSLibrary.Data.Logic;
using CSLibrary.Data.Models;
using CSLibrary.Log;
using CSLibrary.Stuff;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO.Ports;
using СSApp.Models;
using СSApp.Stuff;

namespace СSApp
{
    public class MainViewModel : INotifyCollectionChanged, INotifyPropertyChanged
    {
        public LoggerWpf LoggerInternal { get; set; }

        public ObservableCollection<LineModel> MessagesCollection { get; set; }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public PortWorker PortWorker { get; set; }

        private PersistentValues PersistentValues { get; set; }

        private Dictionary<string, Action<SerialPort, string>> _portsActions;

        public MainViewModel()
        {
            MessagesCollection = new();
            MessagesCollection.CollectionChanged += OnCollectionChanged;

            LoggerInternal = new LoggerWpf();
            LoggerInternal.MessageReceived += LoggerInternal_MessageReceived;

            AppConfig.Instance.Initialize();

            InitializePorts();

            InitializeDatabaseValues();

            InitializePersistentValus();
        }

        private void InitializeDatabaseValues()
        {
            using var initializationLogic = new InitializationLogic();
            var result = initializationLogic.InitializePayTypes();
            var logLevel = result.IsSuccess ? LogLevel.Success : LogLevel.Error;

            Logger.Instance.Log(result.MessageBuilder.ToString(), logLevel);
        }

        private void InitializePorts()
        {
            PortWorker = new PortWorker();
            PortWorker.OpenPorts();

            _portsActions = new()
            {
                { AppConfig.Instance.PortInputName, InputOutputPortDataReceived },
                { AppConfig.Instance.PortOutputName, InputOutputPortDataReceived },
                { AppConfig.Instance.PortQR1Name, QRPortDataReceived },
                { AppConfig.Instance.PortQR2Name, QRPortDataReceived }

            };

            PortWorker.PortDataReceived += (SerialPort port, string data) => { _portsActions[port.PortName].Invoke(port, data); };
        }

        private void InitializePersistentValus()
        {
            PersistentValues = new PersistentValues();

            var result = PersistentValues.Initialize();

            var logLevel = result.IsSuccess ? LogLevel.Success : LogLevel.Error;
            Logger.Instance.Log(result.MessageBuilder.ToString(), logLevel);
        }

        private void InputOutputPortDataReceived(SerialPort port, string data)
        {
            using var userLogic = new UserLogic();

            var findResult = userLogic.FindUserByCardNumber(data);

            if (!findResult.DbAvailable)
            {
                Logger.Instance.Log("База данных недоступна", LogLevel.Error);
                PortWorker.SendHexResponse(port, PortWorker.x31);
                return;
            }

            if (!findResult.IsSuccess || findResult.Entity == null)
            {
                Logger.Instance.Log(findResult.MessageBuilder.ToString(), LogLevel.Error);
                PortWorker.SendHexResponse(port, PortWorker.x32);
                return;
            }

            var entity = findResult.Entity;

            var isExpired = DateTime.Now >= entity.Before;
            if (isExpired)
            {
                Logger.Instance.Log($"Значение поля {nameof(entity.Before)} больше либо равно текущей дате (Номер карты: {entity.Card})", LogLevel.Error);
                PortWorker.SendHexResponse(port, PortWorker.x33);
                return;
            }

            if (port.PortName == AppConfig.Instance.PortInputName && entity.PlaceId == PersistentValues.OutTerritoryPlace?.Id)
            {
                Logger.Instance.Log($"Порт - вход, место - {Constants.OutTerritoryPlaceName}", LogLevel.Info);
                PortWorker.SendHexResponse(port, PortWorker.x06);
                WriteCardEvent(entity, port.PortName);
            }
            else if (port.PortName == AppConfig.Instance.PortOutputName && entity.PlaceId == PersistentValues.AtTerritoryPlace?.Id)
            {
                Logger.Instance.Log($"Порт - выход, место - {Constants.AtTerritoryPlaceName}", LogLevel.Info);
                PortWorker.SendHexResponse(port, PortWorker.x06);
                WriteCardEvent(entity, port.PortName);
            }
            else
            {
                if (entity.Staff)
                {
                    Logger.Instance.Log($"Пользователь {entity.Surname} {entity.Name} {entity.Name} (ID: {entity.Id}) является сотрудником", LogLevel.Success);
                    PortWorker.SendHexResponse(port, PortWorker.x06);
                    WriteCardEvent(entity, port.PortName);
                }
                else
                {
                    Logger.Instance.Log($"Пользователь {entity.Surname} {entity.Name} {entity.Name} (ID: {entity.Id}) не является сотрудником", LogLevel.Warn);
                    PortWorker.SendHexResponse(port, PortWorker.x34);
                }
            }
        }

        private void QRPortDataReceived(SerialPort readablePort, string data)
        {
            var dataInterpreter = new DataInterpreter() { Data = data };
            var date = dataInterpreter.GetDate();

            var isTodayDate = DateTime.Today.Date == date.Date;
            if (!isTodayDate)
            {
                Logger.Instance.Log($"Дата в QR-коде ({date.Date.Date}) отличается от текущей", LogLevel.Warn);
                SendQRResponse(readablePort, PortWorker.x41);
            }
            else
            {
                var fnNumber = dataInterpreter.GetFNNumber();
                var isFNExists = AppConfig.Instance.FNNumbers.Contains(fnNumber);
                if (!isFNExists)
                {
                    Logger.Instance.Log($"Номер ФН ({fnNumber}) отсутствует в конфиге", LogLevel.Warn);
                    SendQRResponse(readablePort, PortWorker.x42);
                }
                else
                {
                    var fpNumber = dataInterpreter.GetFPNumber();

                    var type = readablePort.PortName == AppConfig.Instance.PortQR1Name ? PersistentValues.Entrance : PersistentValues.Exit;
                    var typeId = type.Id;

                    using var qrEventLogic = new QREventLogic();
                    var result = qrEventLogic.Get<Qrevent>(x => x.Fp == fpNumber && x.Dt.Date == DateTime.Today.Date && x.TypeId == typeId);

                    if (!result.IsSuccess)
                    {
                        Logger.Instance.Log(result.MessageBuilder.ToString(), LogLevel.Error);
                        return;
                    }

                    var todayQREvents = result.Entity?.ToList();

                    if (todayQREvents != null && todayQREvents.Any())
                    {
                        Logger.Instance.Log($"В базе уже присутствуют записи с номером ФП {fpNumber}, текущего дня и типом {type.Name}", LogLevel.Warn);
                        SendQRResponse(readablePort, PortWorker.x43);
                    }
                    else
                    {
                        var sum = dataInterpreter.GetSum();
                        SendQRResponse(readablePort, PortWorker.x06);
                        WriteQREvent(typeId, sum, fnNumber, fpNumber);
                    }
                }
            }
        }

        private void SendQRResponse(SerialPort readablePort, byte response)
        {
            if (readablePort.PortName == AppConfig.Instance.PortQR1Name)
                PortWorker.SendHexResponse(PortWorker.InputPort, response);
            else if (readablePort.PortName == AppConfig.Instance.PortQR2Name)
                PortWorker.SendHexResponse(PortWorker.OutputPort, response);
        }

        private void WriteCardEvent(User user, string portName)
        {
            using var cardEventLogic = new CardEventLogic();

            var typeId = portName == AppConfig.Instance.PortInputName ? PersistentValues.Entrance.Id : PersistentValues.Exit.Id;
            var pointId = PersistentValues.Point.Id;

            var result = cardEventLogic.WriteCardEvent(typeId, pointId, user.Card);

            var logLevel = result.IsSuccess ? LogLevel.Success : LogLevel.Error;
            Logger.Instance.Log(result.MessageBuilder.ToString(), logLevel);
        }

        private void WriteQREvent(int typeId, decimal sum, string fn, string fp)
        {
            using var qrEventLogic = new QREventLogic();
            var result = qrEventLogic.WriteQREvent(typeId, sum, fn, fp, PersistentValues.Point.Id, PersistentValues.EmptyPayType.Id);

            var logLevel = result.IsSuccess ? LogLevel.Success : LogLevel.Error;
            Logger.Instance.Log(result.MessageBuilder.ToString(), logLevel);
        }

        private void LoggerInternal_MessageReceived(string message, CSLibrary.Log.LogLevel logLevel, Exception e = null)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (MessagesCollection.Count > 100)
                    MessagesCollection.RemoveAt(100);

                MessagesCollection.Add(new LineModel() { Line = message, LogLevel = logLevel });
            });
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
