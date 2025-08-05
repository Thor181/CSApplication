using System;
using System.Timers;
using CSApp.V2a.Utils;

namespace CSApp.V2a.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public DateOnly CurrentDate { get => field; set => SetProperty(ref field, value); }
        public TimeOnly CurrentTime { get => field; set => SetProperty(ref field, value); }

        public MainWindowViewModel()
        {
            var timer = new Timer(900);
            timer.Elapsed += (s, e) =>
            {
                var datetime = DateTime.Now;
                CurrentDate = datetime.DateOnly;
                CurrentTime = datetime.TimeOnly;

            };
            timer.Start();
        }
    }
}
