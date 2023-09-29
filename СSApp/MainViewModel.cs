using CSLibrary;
using CSLibrary.Data.Logic;
using CSLibrary.Data.Models;
using CSLibrary.Log;
using CSLibrary.Stuff;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
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

            InitializePersistentValus();
        }

        private void InitializePorts()
        {
            PortWorker = new PortWorker();
            PortWorker.OpenPorts();

            _portsActions = new()
            {
                { AppConfig.Instance.PortInputName, InputPortDataReceived }
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

        private void InputPortDataReceived(SerialPort port, string data)
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

            if ((port.PortName == AppConfig.Instance.PortInputName && entity.PlaceId == PersistentValues.OutTerritoryPlace?.Id)
                || (port.PortName == AppConfig.Instance.PortOutputName && entity.PlaceId == PersistentValues.AtTerritoryPlace?.Id))
            {
                PortWorker.SendHexResponse(port, PortWorker.x06);
                WriteCardEvent(entity, port.PortName);
            }
            else
            {
                if (entity.Staff)
                {
                    PortWorker.SendHexResponse(port, PortWorker.x06);
                    WriteCardEvent(entity, port.PortName);
                }
                else
                {
                    PortWorker.SendHexResponse(port, PortWorker.x34);
                }
            }

        }

        private void WriteCardEvent(User user, string portName)
        {
            using var cardEventLogic = new CardEventLogic();

            var cardEvent = new CardEvent();
            cardEvent.Dt = DateTime.Now;
            cardEvent.TypeId = portName == AppConfig.Instance.PortInputName ? PersistentValues.Entrance.Id : PersistentValues.Exit.Id;
            cardEvent.PointId = PersistentValues.Point.Id;
            cardEvent.Card = user.Card;

            cardEventLogic.Add(cardEvent);

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
