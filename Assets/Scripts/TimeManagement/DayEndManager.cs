using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using FlavorfulStory.PlacementSystem;
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
        /// <summary> Отображение сводки дня. </summary>
        private readonly SummaryView _summaryView;

        /// <summary> Контроллер игрока. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Позиция точки сна. </summary>
        private readonly SleepTrigger _sleepTrigger;

        /// <summary> Менеджер локаций. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Флаг выполнения процесса сна. </summary>
        private bool _isProcessingSleep;

        /// <summary> Коллбэк завершения. </summary>
        private Action _onCompleteCallback;

        private readonly PlaceableObjectProvider _objectsProvider;

        /// <summary> Инициализирует менеджер окончания дня. </summary>
        /// <param name="summaryView"> Вью для отображения итогов дня. </param>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="locationManager"> Менеджер управления локациями. </param>
        public DayEndManager(SummaryView summaryView, PlayerController playerController,
            LocationManager locationManager, PlaceableObjectProvider objectsProvider)
        {
            _summaryView = summaryView;
            _playerController = playerController;
            _locationManager = locationManager;
            _objectsProvider = objectsProvider;
        }

        /// <summary> Подписывается на события. </summary>
        public void Initialize() => WorldTime.OnDayEnded += OnDayEnded;

        /// <summary> Отписывается от событий. </summary>
        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        /// <summary> Обрабатывает принудительное завершение дня. </summary>
        /// <param name="date"> Текущая дата игрового времени. </param>
        private void OnDayEnded(DateTime date) => ExhaustedSleep();

        /// <summary> Запрашивает завершение дня. </summary>
        /// <param name="onCompleteCallback"> Коллбэк, вызываемый по завершении процесса. </param>
        public void RequestEndDay(Action onCompleteCallback) =>
            ProcessSleepAsync(onCompleteCallback, false).Forget();

        /// <summary> Обрабатывает принудительный сон при истощении. </summary>
        private void ExhaustedSleep() =>
            ProcessSleepAsync(null, true).Forget();

        /// <summary> Выполняет процесс сна. </summary>
        /// <param name="onComplete"> Коллбэк завершения. </param>
        /// <param name="isExhausted"> Флаг принудительного сна. </param>
        private async UniTaskVoid ProcessSleepAsync(Action onComplete, bool isExhausted)
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            if (!isExhausted) WorldTime.BeginNewDay(6);

            await EndDayRoutine();
            _summaryView.HideWithAnimation().Forget();

            Vector3 position;
            var bed = _objectsProvider.GetObjectsOfType<SleepTrigger>().FirstOrDefault();
            if (bed)
            {
                position = bed.transform.position;
            }
            else
            {
                position = Vector3.zero;
                Debug.LogWarning("Кровать не найдена!");
            }

            await RestorePlayerState(position, isExhausted);

            onComplete?.Invoke();
            _isProcessingSleep = false;
        }

        /// <summary> Выполняет рутину завершения дня. </summary>
        private async UniTask EndDayRoutine()
        {
            WorldTime.Pause();
            _locationManager.EnableLocation(LocationName.RockyIsland);
            await ShowSummaryAndWaitForContinue();
            WorldTime.Unpause();
        }

        /// <summary> Восстанавливает состояние игрока после сна. </summary>
        /// <param name="targetPosition"> Целевая позиция для перемещения игрока. </param>
        /// <param name="isExhausted"> Флаг состояния истощения. </param>
        private async UniTask RestorePlayerState(Vector3 targetPosition, bool isExhausted)
        {
            _playerController.RestoreStatsAfterSleep(isExhausted);
            _playerController.SetPosition(targetPosition);
            await UniTask.Yield();
            _locationManager.UpdateActiveLocation();
        }

        /// <summary> Показывает сводку и ожидает продолжения. </summary>
        private async UniTask ShowSummaryAndWaitForContinue()
        {
            _summaryView.SetSummary(SummaryView.DefaultSummaryText);
            await _summaryView.ShowWithAnimation();
            await _summaryView.WaitForContinue();
        }
    }
}