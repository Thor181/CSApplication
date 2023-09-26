using CSLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using СSApp.Stuff;

namespace СSApp
{
    public class MainViewModel : INotifyCollectionChanged, INotifyPropertyChanged
    {
        public LoggerWpf LoggerInternal { get; set; }

        public ObservableCollection<string> MessagesCollection { get; set; }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            MessagesCollection = new ObservableCollection<string>();
            MessagesCollection.CollectionChanged += OnCollectionChanged;
            
            LoggerInternal = new LoggerWpf();
            LoggerInternal.MessageReceived += LoggerInternal_MessageReceived;
            
            AppConfig.Instance.Initialize();

            var portWorker = new PortWorker();
            portWorker.OpenPorts();
        }

        private void LoggerInternal_MessageReceived(string message, CSLibrary.Log.LogLevel logLevel, Exception e = null)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (MessagesCollection.Count > 100)
                    MessagesCollection.RemoveAt(0);

                MessagesCollection.Add(message);
            });
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
