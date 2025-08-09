using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CSApp.V2a.Services.Options;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSApp.V2a.Services
{
    public class MainScreenService : ObservableObject
    {
        public enum Status
        {
            None = 0,
            Error = 1,
            Success = 2,
        }

        private const string ErrorSound = "Error";
        private const string SuccessSound = "Success";

        private readonly UiOptions _uiOptions;
        private CancellationTokenSource? _previousCts;
        private readonly AudioPlayer _audioPlayer;

        public string CurrentText { get => field; private set => SetProperty(ref field, value); } = string.Empty;
        public string CurrentColor { get => field; private set => SetProperty(ref field, value); }
        public string TextColor { get => field; private set => SetProperty(ref field, value); }

        public MainScreenService(IOptions<UiOptions> uiOptions, AudioPlayer audioPlayer)
        {
            _uiOptions = uiOptions.Value;

#pragma warning disable CA1416 // Validate platform compatibility
            _audioPlayer = audioPlayer;
            _audioPlayer.Load("Assets/Sounds/error.wav", ErrorSound);
            _audioPlayer.Load("Assets/Sounds/success.wav", SuccessSound);
#pragma warning restore CA1416 // Validate platform compatibility

            CurrentText = _uiOptions.HelloMessage;
            CurrentColor = _uiOptions.MainColor;
            TextColor = _uiOptions.TextColor;
        }

        public void Set(string text, Status status)
        {
            CurrentText = text;
            CurrentColor = StatusToColorString(status);

            PlayStatusSound(status);

            _previousCts?.Cancel();
            _previousCts = new CancellationTokenSource();

            ScheduleClear(_previousCts.Token);
        }

        private void ScheduleClear(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(_uiOptions.SecondsToClear));

                if (cancellationToken.IsCancellationRequested)
                    return;

                CurrentText = _uiOptions.HelloMessage;
                CurrentColor = _uiOptions.MainColor;
            }, cancellationToken);
        }

        private string StatusToColorString(Status status)
        {
            return status switch
            {
                Status.Success => _uiOptions.SuccessColor,
                Status.Error => _uiOptions.ErrorColor,
                _ => _uiOptions.MainColor,
            };
        }

        private void PlayStatusSound(Status status)
        {
            switch (status)
            {
                case Status.Success:
                    _audioPlayer.Play(SuccessSound);
                    break;
                case Status.Error:
                    _audioPlayer.Play(ErrorSound);
                    break;
                default:
                    throw new NotImplementedException($"Sound not found for status = '{status}'");
            }
            ;
        }
    }
}
