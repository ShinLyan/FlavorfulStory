using System;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure.Services.WindowService;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement.UI;

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
        
        /// <summary> Сервис окон. </summary>
        private readonly IWindowService _windowService;
        
        /// <summary> Спит ли игрок? </summary>
        private bool _isSleeping;

        /// <summary> Начало дня (в часах). </summary>
        private const int DefaultDayStartHour = 6;
        
        /// <summary> Конструктор с внедрением зависимостей. </summary>
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

        /// <summary> Подписка на завершение дня. </summary>
        public void Initialize() => WorldTime.OnDayEnded += OnDayEnded;
        
        /// <summary> Отписка от событий. </summary>
        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        /// <summary> Вручную запрашивает завершение дня (через взаимодействие). </summary>
        /// <param name="onComplete"> Колбэк после завершения последовательности сна. </param>
        public void RequestEndDay(Action onComplete)
        {
            ProcessSleepAsync(false).ContinueWith(() => onComplete?.Invoke()).Forget();
        }

        /// <summary> Обработка завершения дня по таймеру. </summary>
        private void OnDayEnded(DateTime _) => ProcessSleepAsync(true).Forget();

        /// <summary> Основной процесс сна (и восстановления), не даёт запустить второй раз. </summary>
        private async UniTask ProcessSleepAsync(bool isExhausted)
        {
            if (_isSleeping) return;
            
            _isSleeping = true;

            if (!isExhausted) WorldTime.BeginNewDay(DefaultDayStartHour);

            await ExecuteSleepSequence(isExhausted);
            _isSleeping = false;
        }

        /// <summary> Последовательность сна: активация локации, показ окна сводки, восстановление игрока. </summary>
        private async UniTask ExecuteSleepSequence(bool isExhausted)
        {
            _locationManager.EnableLocation(LocationName.RockyIsland);

            await ShowSummaryWindowAsync();
            
            _windowService.CloseWindow<SummaryWindow>();
            RestorePlayer(isExhausted);
        }

        /// <summary> Открывает окно сводки и ждёт его закрытия. </summary>
        private async UniTask ShowSummaryWindowAsync()
        {
            var window = _windowService.GetWindow<SummaryWindow>();
            if (window == null) return;

            window.SetSummary(SummaryWindow.DefaultSummaryText);
            _windowService.OpenWindow<SummaryWindow>();

            await WaitForWindowCloseAsync(window);
        }

        /// <summary> Восстанавливает игрока после сна. </summary>
        private void RestorePlayer(bool isExhausted)
        {
            _playerController.RestoreStatsAfterSleep(isExhausted);
            _playerController.SetPosition(_sleepPosition);
            _locationManager.UpdateActiveLocation();
        }

        /// <summary> Асинхронно ожидает закрытия окна сводки. </summary>
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