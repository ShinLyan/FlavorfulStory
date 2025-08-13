using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет процессом завершения дня. </summary>
    public class DayEndManager : IInitializable, IDisposable
    {
        /// <summary> Компонент для отображения сводки дня с возможностью продолжения игры. </summary>
        private readonly SummaryView _summaryView;

        /// <summary> Колбэк, вызываемый после завершения процесса окончания дня. </summary>
        private Action _onCompleteCallback;

        /// <summary> Контроллер игрока для управления его позицией и получением компонентов. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Позиция триггера сна (кровати), используется для размещения игрока при принудительном сне. </summary>
        private readonly Vector3 _sleepTriggerPosition;

        /// <summary> Менеджер локаций для активации текущей локации игрока после сна. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Флаг, предотвращающий одновременное выполнение нескольких процессов сна. </summary>
        private bool _isProcessingSleep;

        public DayEndManager(
            SummaryView summaryView,
            PlayerController playerController,
            SleepTrigger sleepTrigger,
            LocationManager locationManager)
        {
            _summaryView = summaryView;
            _playerController = playerController;
            _sleepTriggerPosition = sleepTrigger.transform.position;
            _locationManager = locationManager;
        }

        /// <summary> Инициализация компонента (реализация IInitializable). </summary>
        public void Initialize() => WorldTime.OnDayEnded += OnDayEnded;

        /// <summary> Освобождение ресурсов и отписка от событий (реализация IDisposable). </summary>
        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        private void OnDayEnded(DateTime date) => ExhaustedSleep();

        public void RequestEndDay(Action onCompleteCallback) =>
            ProcessSleepAsync(onCompleteCallback, false).Forget();

        private void ExhaustedSleep() =>
            ProcessSleepAsync(null, true).Forget();

        private async UniTaskVoid ProcessSleepAsync(Action onComplete, bool isExhausted)
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            if (!isExhausted) WorldTime.BeginNewDay(6);

            await EndDayRoutine();
            _summaryView.HideWithAnimation().Forget();
            await RestorePlayerState(_sleepTriggerPosition, isExhausted);

            onComplete.Invoke();
        }

        /// <summary> Полная логика конца дня: пауза времени, сводка, продолжение. </summary>
        private async UniTask EndDayRoutine(CancellationToken token = default)
        {
            WorldTime.Pause();

            _locationManager.EnableLocation(LocationName.RockyIsland);

            await ShowSummaryAndWaitForContinue(token);

            _isProcessingSleep = false;
            WorldTime.Unpause();
        }

        private async UniTask RestorePlayerState(Vector3 targetPosition, bool isExhausted)
        {
            _playerController.RestoreStatsAfterSleep(isExhausted);
            _playerController.SetPosition(targetPosition);

            await UniTask.Yield();

            _locationManager.UpdateActiveLocation();
        }

        /// <summary>
        /// Теперь вся анимация — внутри SummaryView.
        /// </summary>
        private async UniTask ShowSummaryAndWaitForContinue(CancellationToken token)
        {
            _summaryView.SetSummary(SummaryView.DefaultSummaryText);
            await _summaryView.ShowWithAnimation();
            await _summaryView.WaitForContinue(token);
        }
    }
}