using System;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement.UI;
using FlavorfulStory.Windows;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет процессом завершения игрового дня. </summary>
    public class DayEndManager : IInitializable, IDisposable
    {
        private readonly PlayerController _playerController;
        private readonly LocationManager _locationManager;
        private readonly IWindowService _windowService;
        private readonly PlayerSpawnService _playerSpawnService;
        private readonly SignalBus _signalBus;

        private bool _isProcessingSleep;

        private const int DefaultDayStartHour = 6;

        public DayEndManager(
            PlayerController playerController,
            LocationManager locationManager,
            IWindowService windowService,
            PlayerSpawnService playerSpawnService,
            SignalBus signalBus)
        {
            _playerController = playerController;
            _locationManager = locationManager;
            _windowService = windowService;
            _playerSpawnService = playerSpawnService;
            _signalBus = signalBus;
        }

        public void Initialize() => WorldTime.OnDayEnded += OnDayEnded;
        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        private void OnDayEnded(DateTime _) => ProcessSleepAsync(true).Forget();

        public void RequestEndDay(Action onComplete) =>
            ProcessSleepAsync(false).ContinueWith(() => onComplete?.Invoke()).Forget();

        private async UniTask ProcessSleepAsync(bool isExhausted)
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            if (!isExhausted)
                WorldTime.BeginNewDay(DefaultDayStartHour);
            else
                _signalBus.Fire(new ExhaustedSleepSignal());

            await ExecuteSleepSequence(isExhausted);
            _isProcessingSleep = false;
        }

        private async UniTask ExecuteSleepSequence(bool isExhausted)
        {
            _locationManager.EnableLocation(LocationName.RockyIsland);

            await ShowSummaryWindowAsync();

            _windowService.CloseWindow<SummaryWindow>();

            RestorePlayer(isExhausted);
        }

        private void RestorePlayer(bool isExhausted)
        {
            var spawnPosition = _playerSpawnService.GetSpawnPosition();

            _playerController.RestoreStatsAfterSleep(isExhausted);
            _playerController.SetPosition(spawnPosition);
            _locationManager.UpdateActiveLocation();
        }

        private async UniTask ShowSummaryWindowAsync()
        {
            var window = _windowService.GetWindow<SummaryWindow>();
            window.SetSummary(SummaryWindow.DefaultSummaryText);
            _windowService.OpenWindow<SummaryWindow>();

            await WaitForWindowCloseAsync(window);
        }

        private static UniTask WaitForWindowCloseAsync(SummaryWindow window)
        {
            var tcs = new UniTaskCompletionSource();

            void OnClosed()
            {
                window.Closed -= OnClosed;
                tcs.TrySetResult();
            }

            window.Closed += OnClosed;
            return tcs.Task;
        }
    }
}