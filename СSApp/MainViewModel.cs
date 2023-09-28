using CSLibrary;
using CSLibrary.Data.Logic;
using CSLibrary.Log;
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


        private Dictionary<string, Action<SerialPort, string>> _portsActions;

        public MainViewModel()
        {
            MessagesCollection = new();
            MessagesCollection.CollectionChanged += OnCollectionChanged;

            LoggerInternal = new LoggerWpf();
            LoggerInternal.MessageReceived += LoggerInternal_MessageReceived;

            AppConfig.Instance.Initialize();

            PortWorker = new PortWorker();
            PortWorker.OpenPorts();

            _portsActions = new()
            {
                { AppConfig.Instance.PortInputName, InputPortDataReceived }
            };

            PortWorker.PortDataReceived += (SerialPort port, string data) => { _portsActions[port.PortName].Invoke(port, data); };
        }

        private void InputPortDataReceived(SerialPort port, string data)
        {
            using var userLogic = new UserLogic();

            var findResult = userLogic.FindUserByCardNumber(data);

            if (!findResult.DbAvailable)
            {
                Logger.Instance.Log("База данных недоступна", LogLevel.Error);
                PortWorker.SendResponse(port, PortWorker.x31);
                return;
            }

            if (!findResult.IsSuccess || findResult.Entity == null)
            {
                Logger.Instance.Log(findResult.MessageBuilder.ToString(), LogLevel.Error);
                PortWorker.SendResponse(port, PortWorker.x32);
                return;
            }

            var entity = findResult.Entity;
            
            var isExpired = entity.Before >= DateTime.Now;
            if (isExpired)
            {
                Logger.Instance.Log($"Значение поля {nameof(entity.Before)} больше либо равно текущей дате", LogLevel.Error);
                PortWorker.SendResponse(port, PortWorker.x33);
                return;
            }

            if (port.PortName == AppConfig.Instance.PortInputName && entity.PlaceId == )

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
