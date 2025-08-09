using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CSApp.V2a.Services;
using CSApp.V2a.Services.Options;
using CSApp.V2a.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CSApp.V2a.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
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

        public MainWindowViewModel(ILogger logger, IOptions<UiOptions> uiOptions, MainScreenService mainTextService) : this()
        {
            MainScreenService = mainTextService;
            MainScreenService.Set("as", MainScreenService.Status.Error);
            MainScreenService.Set("as", MainScreenService.Status.Success);
        }
    }
}
