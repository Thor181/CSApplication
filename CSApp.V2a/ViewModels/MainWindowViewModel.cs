using CSApp.V2a.Services;
using CSApp.V2a.Services.Options;
using CSApp.V2a.Utils;
using CSLibrary.V2;
using CSLibrary.V2.Data.Logic;
using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CSApp.V2a.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly PortWorker _portWorker;
        private readonly PortWorkerOptions _portWorkerOptions;
        private readonly PersistentValues _persistentValues;

        private Dictionary<string, Action<SerialPort, string>> _portsActions;

        public DateTime DateTime { get => field; set => SetProperty(ref field, value); }
        public MainScreenService MainScreenService { get => field; set => SetProperty(ref field, value); }

        //design mode
        public MainWindowViewModel()
        {
            var timer = new System.Timers.Timer(900);
            timer.Elapsed += (s, e) =>
            {
                DateTime = DateTime.Now;
            };
            timer.Start();
        }

        public MainWindowViewModel(IServiceProvider serviceProvider,
            ILogger logger,
            MainScreenService mainTextService,
            PortWorker portWorker,
            IOptions<PortWorkerOptions> portWorkerOptions,
            PersistentValues persistentValues) : this()
        {
            MainScreenService = mainTextService;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _portWorker = portWorker;
            _portWorkerOptions = portWorkerOptions.Value;
            _persistentValues = persistentValues;

            InitializePorts();
            InitializePersistentValus();
            InitializeDatabaseValues();
        }

        private void InitializePorts()
        {
            _portWorker.OpenPorts();

            _portsActions = new()
            {
                { _portWorkerOptions.PortInputName, InputOutputPortDataReceived },
                { _portWorkerOptions.PortOutputName, InputOutputPortDataReceived },
                { _portWorkerOptions.PortQR1Name, QRPortDataReceived },
                { _portWorkerOptions.PortQR2Name, QRPortDataReceived }
            };

            _portWorker.PortDataReceived += (port, data) => { _portsActions[port.PortName].Invoke(port, data); };
        }

        private void InitializePersistentValus()
        {
            var result = _persistentValues.Initialize();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Инициализация постоянных значений завершена без ошибок. Результат: {result}", result.MessageBuilder.ToString());
            }
            else
            {
                _logger.LogError("Инициализация постоянных значений завершена с ошибкой. Результат: {result}", result.MessageBuilder.ToString());
            }
        }

        private void InitializeDatabaseValues()
        {
            using var scope = _serviceProvider.CreateScope();
            var initializationLogic = scope.ServiceProvider.GetRequiredService<InitializationLogic>();
            var operatorInitResult = initializationLogic.InitializeOperatorEventsIfNotExists();

            if (!operatorInitResult.IsSuccess)
            {
                _logger.LogError("Инициализацияя базы данных завершена с ошибкой. Результат: {result}", operatorInitResult.MessageBuilder.ToString());
                return;
            }
            else
            {
                _logger.LogInformation(operatorInitResult.MessageBuilder.ToString());
            }

            var result = initializationLogic.InitializePayTypes();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Инициализацияя базы данных завершена без ошибок. Результат: {result}", result.MessageBuilder.ToString());
            }
            else
            {
                _logger.LogError("Инициализацияя базы данных завершена с ошибкой. Результат: {result}", result.MessageBuilder.ToString());
            }
        }

        private void InputOutputPortDataReceived(SerialPort port, string data)
        {
            var dataParsedAsInteger = int.TryParse(data, out var dataAsInteger);
            if (dataParsedAsInteger && (dataAsInteger is PortWorker.x01 or PortWorker.x02))
            {
                using var operatorScope = _serviceProvider.CreateScope();
                var operatorLogic = operatorScope.ServiceProvider.GetRequiredService<OperatorEventLogic>();

                int eventTypeId = -1;
                if (dataAsInteger == PortWorker.x01)
                    eventTypeId = _persistentValues.Entrance.Id;
                else if (dataAsInteger == PortWorker.x02)
                    eventTypeId = _persistentValues.Exit.Id;

                var result = operatorLogic.WriteEvent(eventTypeId, _persistentValues.Point.Id);
                
                if (!result.IsSuccess)
                {
                    _logger.LogError("Возникла ошибка при сохранении события оператора в базе данных. Результат: {result}", result.MessageBuilder.ToString());
                    MainScreenService.Set("Проход запрещен", MainScreenService.Status.Error);
                }
                else
                {
                    MainScreenService.Set("Проход разрешен", MainScreenService.Status.Success);
                }

                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var userLogic = scope.ServiceProvider.GetRequiredService<UserLogic>();
            var findResult = userLogic.FindUserByCardNumber(data);

            if (!findResult.DbAvailable)
            {
                const string DbUnavailable = "База данных недоступна";
                _logger.LogError(DbUnavailable);
                _portWorker.SendHexResponse(port, PortWorker.x31);
                MainScreenService.Set(DbUnavailable, MainScreenService.Status.Error);
                return;
            }

            if (!findResult.IsSuccess || findResult.Entity == null)
            {
                _logger.LogError("Поиск пользователя завершился с ошибкой. Результат: {result}", findResult.MessageBuilder.ToString());
                _portWorker.SendHexResponse(port, PortWorker.x32);
                MainScreenService.Set("Пользователь не найден", MainScreenService.Status.Error);
                return;
            }

            var entity = findResult.Entity;

            var isExpired = DateTime.Now >= entity.Before;
            if (isExpired)
            {
                _logger.LogError("Значение поля {field} больше либо равно текущей дате (Номера карты: {card})", nameof(entity.Before), entity.Card);
                _portWorker.SendHexResponse(port, PortWorker.x33);
                MainScreenService.Set("Карта просрочена", MainScreenService.Status.Error);
                return;
            }

            if (port.PortName == _portWorkerOptions.PortInputName && entity.PlaceId == _persistentValues.OutTerritoryPlace?.Id)
            {
                _logger.LogInformation("Порт - вход, место - {place}", CSLibrary.V2.Stuff.Constants.OutTerritoryPlaceName);
                HandleUser(port, entity, userLogic, _persistentValues.AtTerritoryPlace!.Id);
            }
            else if (port.PortName == _portWorkerOptions.PortOutputName && entity.PlaceId == _persistentValues.AtTerritoryPlace?.Id)
            {
                _logger.LogInformation("Порт - выход, место - {place}", CSLibrary.V2.Stuff.Constants.AtTerritoryPlaceName);
                HandleUser(port, entity, userLogic, _persistentValues.OutTerritoryPlace!.Id);
            }
            else
            {
                if (entity.Staff)
                {
                    _logger.LogInformation("Пользователь {surname} {name} {name} (ID: {id}) является сотрудником", entity.Surname, entity.Name, entity.Name2, entity.Id);
                    var eventIsSuccess = WriteCardEvent(entity, port.PortName);

                    if (!eventIsSuccess)
                    {
                        MainScreenService.Set("Ошибка обработки пользователя", MainScreenService.Status.Error);
                    }
                    else
                    {
                        _portWorker.SendHexResponse(port, PortWorker.x06);
                        MainScreenService.Set("Проход разрешен", MainScreenService.Status.Success);
                    }
                }
                else
                {
                    _logger.LogInformation("Пользователь {surname} {name} {name} (ID: {id}) не является сотрудником " +
                        "и не соответствует требованиям алгоритма пропуска (Порт:Вход-Место:За территорией ИЛИ Порт:Выход-Место:На территории). Текущее место: {place}",
                        entity.Surname, entity.Name, entity.Name2, entity.Id, entity.Place.Name);

                    _portWorker.SendHexResponse(port, PortWorker.x34);
                    MainScreenService.Set("Проход запрещен", MainScreenService.Status.Error);
                }
            }
        }

        private void QRPortDataReceived(SerialPort readablePort, string data)
        {
            var dataInterpreter = new DataInterpreter() { Data = data };
            var qrCodeDate = dataInterpreter.GetDate();

            var isTodayDate = DateTime.Today.Date == qrCodeDate.Date;
            if (!isTodayDate)
            {
                _logger.LogWarning("Дата в QR-коде ({code}) отличается от текущей", qrCodeDate.Date.Date);
                SendQRResponse(readablePort, PortWorker.x41);
                MainScreenService.Set("Дата в QR-коде отличается от текущей", MainScreenService.Status.Error);
            }
            else
            {
                var fnNumber = dataInterpreter.GetFNNumber();
                var isFNExists = _portWorkerOptions.FNNumbers.Contains(fnNumber);
                if (!isFNExists)
                {
                    _logger.LogWarning("Номер ФН ({number}) отсутствует в конфиге", fnNumber);
                    SendQRResponse(readablePort, PortWorker.x42);
                    MainScreenService.Set("Номер ФН не соответствует настройкам", MainScreenService.Status.Error);
                }
                else
                {
                    var fpNumber = dataInterpreter.GetFPNumber();

                    var type = readablePort.PortName == _portWorkerOptions.PortQR1Name ? _persistentValues.Entrance : _persistentValues.Exit;
                    var typeId = type.Id;

                    using var scope = _serviceProvider.CreateScope();
                    var qrEventLogic = scope.ServiceProvider.GetRequiredService<QREventLogic>();
                    var result = qrEventLogic.Get<Qrevent>(x => x.Fp == fpNumber && x.Dt.Date == DateTime.Today.Date && x.TypeId == typeId);

                    if (!result.IsSuccess)
                    {
                        _logger.LogError("Не удалось получить {qrevent} из базы данных. Результат: {result}", nameof(Qrevent), result.MessageBuilder.ToString());
                        return;
                    }

                    var todayQREvents = result.Entity?.ToList();

                    if (todayQREvents != null && todayQREvents.Count > 0)
                    {
                        _logger.LogWarning("В базе данных уже присутствуют записи с номером ФП {fp}, текущего дня и типов {type}", fpNumber, type.Name);
                        SendQRResponse(readablePort, PortWorker.x43);
                        MainScreenService.Set("Доступ запрещен", MainScreenService.Status.Error);
                    }
                    else
                    {
                        var sum = dataInterpreter.GetSum();

                        var isAllowed = false;

                        var qrEventIsSuccess = WriteQREvent(typeId, sum, fnNumber, fpNumber);

                        if (!qrEventIsSuccess)
                        {
                            MainScreenService.Set("Проход запрещен", MainScreenService.Status.Error);
                            return;
                        }

                        if (sum >= _portWorkerOptions.N1)
                        {
                            SendQRResponse(readablePort, PortWorker.x06);
                        }
                        else if (_portWorkerOptions.N2 < sum && sum < _portWorkerOptions.N1)
                        {
                            SendQRResponse(readablePort, PortWorker.x07);
                        }
                        else if (sum <= _portWorkerOptions.N2)
                        {
                            SendQRResponse(readablePort, PortWorker.x08);
                        }

                        if (isAllowed)
                        {
                            MainScreenService.Set("Проход разрешен", MainScreenService.Status.Success);
                            return;
                        }
                        else
                        {
                            _logger.LogWarning("Событие в базу данных было записано, но проход не разрешен. Возможно, параметр s не попал в следующие условия: " +
                                $"1. s >= N1: {sum} >= {_portWorkerOptions.N1}\n" +
                                $"2. N2 < s и s < N1: {_portWorkerOptions.N2} < {sum} и {sum} < {_portWorkerOptions.N1}\n" +
                                $"3. s <= N2: {sum} <= {_portWorkerOptions.N2}");
                            return;
                        }
                    }
                }
            }
        }

        private void SendQRResponse(SerialPort readablePort, byte response)
        {
            if (readablePort.PortName == _portWorkerOptions.PortQR1Name)
                _portWorker.SendHexResponse(_portWorker.InputPort, response);
            else if (readablePort.PortName == _portWorkerOptions.PortQR2Name)
                _portWorker.SendHexResponse(_portWorker.OutputPort, response);
        }

        private void HandleUser(SerialPort port, User user, UserLogic userLogic, int placeId)
        {
            var eventIsSuccess = WriteCardEvent(user, port.PortName);

            if (!eventIsSuccess)
            {
                MainScreenService.Set("Ошибка обработки пользователя", MainScreenService.Status.Error);
                return;
            }

            user.PlaceId = placeId;
            
            var saveResult = userLogic.SaveChanges();

            if (!saveResult.IsSuccess)
            {
                _logger.LogError("Возникла ошибка при сохранении изменений пользователя в базе данных. Пользователь: {user}. Результат: {result}",
                    user, saveResult.MessageBuilder.ToString());
                MainScreenService.Set("Ошибка сохранения в базе данных", MainScreenService.Status.Error);
            }
            else
            {
                _portWorker.SendHexResponse(port, PortWorker.x06);
                MainScreenService.Set("Успешно", MainScreenService.Status.Success);
            }
        }

        private bool WriteCardEvent(User user, string portName)
        {
            var typeId = portName == _portWorkerOptions.PortInputName ? _persistentValues.Entrance!.Id : _persistentValues.Exit!.Id;
            var pointId = _persistentValues.Point.Id;

            using var scope = _serviceProvider.CreateScope();
            var cardEventLogic = scope.ServiceProvider.GetRequiredService<CardEventLogic>();
            var result = cardEventLogic.WriteCardEvent(typeId, pointId, user.Card);

            if (result.IsSuccess)
                _logger.LogInformation("Запись события карты завершена без ошибок. Результат: {result}", result.MessageBuilder.ToString());
            else
                _logger.LogError("Запись события карты завершена с ошибкой. Результат: {result}", result.MessageBuilder.ToString());

            return result.IsSuccess;
        }

        private bool WriteQREvent(int typeId, decimal sum, string fn, string fp)
        {
            using var scope = _serviceProvider.CreateScope();
            var qrEventLogic = scope.ServiceProvider.GetRequiredService<QREventLogic>();
            var result = qrEventLogic.WriteQREvent(typeId, sum, fn, fp, _persistentValues.Point.Id, _persistentValues.EmptyPayType.Id);

            if (result.IsSuccess)
                _logger.LogInformation("Запись QR-события завершена без ошибок. Результат: {result}", result.MessageBuilder.ToString());
            else
                _logger.LogError("Запись QR-события завершена с ошибкой. Результат: {result}", result.MessageBuilder.ToString());

            return result.IsSuccess;
        }
    }
}
