using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Stats;
using FlavorfulStory.UI;
using FlavorfulStory.UI.Animation;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Управляет процессом завершения дня в игре. 
    /// Отвечает за переход между днями, восстановление характеристик игрока и отображение экрана сводки. </summary>
    public class DayEndManager : IInitializable, IDisposable
    {
        /// <summary> Компонент для отображения сводки дня с возможностью продолжения игры. </summary>
        private readonly SummaryView _summaryView;

        /// <summary> Колбэк, вызываемый после завершения процесса окончания дня. </summary>
        private Action _onCompleteCallback;

        /// <summary> Контроллер игрока для управления его позицией и получением компонентов. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Множитель восстановления выносливости при истощении (75% от максимума). </summary>
        private const float StaminaMultiplier = 0.75f;

        /// <summary> Transform триггера сна (кровати), используется для размещения игрока при принудительном сне. </summary>
        private readonly Transform _sleepTriggerTransform;

        /// <summary> Менеджер локаций для активации текущей локации игрока после сна. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Флаг, предотвращающий одновременное выполнение нескольких процессов сна. </summary>
        private bool _isProcessingSleep;

        /// <summary> Затемнение экрана при переходах между сценами. </summary>
        private readonly CanvasGroupFader _canvasGroupFader;

        private readonly CancellationTokenSource _cts;

        /// <summary> Конструктор DayEndManager. </summary>
        /// <param name="summaryView"> Компонент отображения сводки дня. </param>
        /// <param name="playerController">Контроллер игрока. </param>
        /// <param name="sleepTrigger"> Триггер сна (кровать). </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        public DayEndManager(SummaryView summaryView,
            PlayerController playerController,
            SleepTrigger sleepTriggerTransform,
            LocationManager locationManager,
            CanvasGroupFader canvasGroupFader)
        {
            _summaryView = summaryView;
            _playerController = playerController;
            _sleepTriggerTransform = sleepTriggerTransform.transform;
            _locationManager = locationManager;
            _canvasGroupFader = canvasGroupFader;
            _isProcessingSleep = false;

            WorldTime.OnDayEnded += OnDayEnded;
            _cts = new CancellationTokenSource();
        }

        /// <summary> Инициализация компонента (реализация IInitializable). </summary>
        public void Initialize() { }

        /// <summary> Освобождение ресурсов и отписка от событий (реализация IDisposable). </summary>
        public void Dispose()
        {
            WorldTime.OnDayEnded -= OnDayEnded;
            _cts?.Cancel();
            _cts?.Dispose();
        }

        /// <summary> Обработчик события окончания дня из системы времени. </summary>
        /// <param name="date"> Дата окончившегося дня. </param>
        private void OnDayEnded(DateTime date) => ExhaustedSleep().Forget();

        /// <summary> Запрос на завершение дня по инициативе игрока (взаимодействие с кроватью). </summary>
        /// <param name="triggerTransform"> Transform объекта-триггера (кровати). </param>
        /// <param name="onCompleteCallback"> Колбэк, вызываемый после завершения процесса. </param>
        public void RequestEndDay(Transform triggerTransform, Action onCompleteCallback)
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            _onCompleteCallback = onCompleteCallback;
            EndDayRoutine().Forget();
            ResetPlayer(triggerTransform).Forget();
        }

        /// <summary> Принудительный сон при истощении игрока (автоматическое завершение дня). </summary>
        private async UniTaskVoid ExhaustedSleep()
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            await UniTask.WhenAll(
                EndDayRoutine(),
                ResetPlayer(_sleepTriggerTransform, true)
            );
        }

        /// <summary> Основная корутина завершения дня.
        /// Управляет последовательностью: пауза времени → затухание → сводка → продолжение. </summary>
        private async UniTask EndDayRoutine()
        {
            WorldTime.Pause();

            // TODO: худ игры скрывается
            // _hud.SetActive(false); //TODO: переделать на норм версию
            await _canvasGroupFader.Hide().AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(_cts.Token);

            _summaryView.Show();
            bool continuePressed = false;
            _summaryView.OnContinuePressed = () => continuePressed = true;
            _summaryView.SetSummary(SummaryView.DefaultSummaryText);

            await _canvasGroupFader.Show().AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(_cts.Token);
            await UniTask.WaitUntil(() => continuePressed, cancellationToken: _cts.Token);

            _onCompleteCallback?.Invoke();

            _summaryView.Hide();
            WorldTime.Unpause();

            await _canvasGroupFader.Hide().AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(_cts.Token);
            await _canvasGroupFader.Show().AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(_cts.Token);

            _isProcessingSleep = false;
            // _hud.SetActive(true); //TODO: переделать на норм версию
        }

        /// <summary> Сброс состояния игрока: восстановление здоровья, выносливости и позиции. </summary>
        /// <param name="triggerTransform"> Transform позиции для размещения игрока. </param>
        /// <param name="exhausted"> Флаг истощения (влияет на восстановление выносливости). </param>
        private async UniTask ResetPlayer(Transform triggerTransform, bool exhausted = false)
        {
            await _canvasGroupFader.Hide().AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(_cts.Token);

            var playerStats = _playerController.GetComponent<PlayerStats>();

            var health = playerStats.GetStat<Health>();
            health.SetValue(health.MaxValue);

            var stamina = playerStats.GetStat<Stamina>();
            stamina.SetValue(exhausted ? stamina.MaxValue * StaminaMultiplier : stamina.MaxValue);

            _playerController.UpdatePosition(triggerTransform);
            await UniTask.Yield(_cts.Token);
            _locationManager.ActivatePlayerCurrentLocation();
            _locationManager.EnableLocation(LocationName.RockyIsland);
        }
    }
}