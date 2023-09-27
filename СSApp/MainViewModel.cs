using CSLibrary;
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
            _portsActions = new()
            {
                { AppConfig.Instance.PortInputName, InputPortDataReceived }
            };

            MessagesCollection = new();
            MessagesCollection.CollectionChanged += OnCollectionChanged;

            LoggerInternal = new LoggerWpf();
            LoggerInternal.MessageReceived += LoggerInternal_MessageReceived;

            AppConfig.Instance.Initialize();

            PortWorker = new PortWorker();
            PortWorker.OpenPorts();

            PortWorker.PortDataReceived += (SerialPort port, string data) => { _portsActions[port.PortName].Invoke(port, data); };
        }

        private void InputPortDataReceived(SerialPort port, string data)
        {

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
