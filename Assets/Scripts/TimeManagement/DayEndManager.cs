using System;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет процессом завершения игрового дня.. </summary>
    public class DayEndManager : IInitializable, IDisposable
    {
        /// <summary> Контроллер игрока. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Позиция точки сна. </summary>
        private readonly Vector3 _sleepPosition;

        /// <summary> Менеджер локаций. </summary>
        private readonly LocationManager _locationManager;
        private readonly IWindowService _windowService;
        
        private bool _isSleeping;

        private const int DefaultDayStartHour = 6;
        
        public DayEndManager(
            PlayerController playerController,
            SleepTrigger sleepTrigger,
            LocationManager locationManager,
            IWindowService windowService)
        {
            _playerController = playerController;
            _sleepPosition = sleepTrigger.transform.position;
            _locationManager = locationManager;
            _windowService = windowService;
        }

        public void Initialize() => WorldTime.OnDayEnded += OnDayEnded;
        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        public void RequestEndDay(Action onComplete)
        {
            ProcessSleepAsync(false).ContinueWith(() => onComplete?.Invoke()).Forget();
        }

        private void OnDayEnded(DateTime _) => ProcessSleepAsync(true).Forget();

        private async UniTask ProcessSleepAsync(bool isExhausted)
        {
            if (_isSleeping) return;
            
            _isSleeping = true;

            if (!isExhausted) WorldTime.BeginNewDay(DefaultDayStartHour);

            await ExecuteSleepSequence(isExhausted);
            _isSleeping = false;
        }

        private async UniTask ExecuteSleepSequence(bool isExhausted)
        {
            _locationManager.EnableLocation(LocationName.RockyIsland);

            await ShowSummaryWindowAsync();
            
            _windowService.CloseWindow<SummaryWindow>();
            RestorePlayer(isExhausted);
        }

        private async UniTask ShowSummaryWindowAsync()
        {
            var window = _windowService.GetWindow<SummaryWindow>();
            if (window == null) return;

            window.SetSummary(SummaryWindow.DefaultSummaryText);
            _windowService.OpenWindow<SummaryWindow>();

            await WaitForWindowCloseAsync(window);
        }

        private void RestorePlayer(bool isExhausted)
        {
            _playerController.RestoreStatsAfterSleep(isExhausted);
            _playerController.SetPosition(_sleepPosition);
            _locationManager.UpdateActiveLocation();
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