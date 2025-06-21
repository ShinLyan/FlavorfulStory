using System;
using System.Collections;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Stats;
using FlavorfulStory.UI;
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

        /// <summary> Компонент для создания эффектов затухания/появления экрана. </summary>
        private readonly Fader _fader;

        /// <summary> Колбэк, вызываемый после завершения процесса окончания дня. </summary>
        private Action _onCompleteCallback;

        /// <summary> MonoBehaviour для запуска корутин (так как DayEndManager не наследуется от MonoBehaviour). </summary>
        private readonly MonoBehaviour _coroutineRunner;

        /// <summary> Контроллер игрока для управления его позицией и получения компонентов. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Множитель восстановления выносливости при истощении (75% от максимума). </summary>
        private const float _staminaMultiplier = 0.75f;

        /// <summary> Transform триггера сна (кровати), используется для размещения игрока при принудительном сне. </summary>
        private readonly Transform _sleepTriggerTransform;

        /// <summary> Менеджер локаций для активации текущей локации игрока после сна. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Флаг, предотвращающий одновременное выполнение нескольких процессов сна. </summary>
        private bool _isProcessingSleep;

        /// <summary> Конструктор DayEndManager. </summary>
        /// <param name="summaryView"> Компонент отображения сводки дня. </param>
        /// <param name="fader"> Компонент затухания экрана. </param>
        /// <param name="coroutineRunner"> MonoBehaviour для запуска корутин. </param>
        /// <param name="playerController">Контроллер игрока. </param>
        /// <param name="sleepTrigger"> Триггер сна (кровать). </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        public DayEndManager(SummaryView summaryView,
            Fader fader,
            MonoBehaviour coroutineRunner,
            PlayerController playerController,
            SleepTrigger sleepTrigger,
            LocationManager locationManager)
        {
            _summaryView = summaryView;
            _fader = fader;
            _coroutineRunner = coroutineRunner;
            _playerController = playerController;
            _sleepTriggerTransform = sleepTrigger.transform;
            _locationManager = locationManager;
            _isProcessingSleep = false;

            WorldTime.OnDayEnded += OnDayEnded;
        }

        /// <summary> Инициализация компонента (реализация IInitializable). </summary>
        public void Initialize() { }

        /// <summary> Освобождение ресурсов и отписка от событий (реализация IDisposable). </summary>
        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        /// <summary> Обработчик события окончания дня из системы времени. </summary>
        /// <param name="date"> Дата окончившегося дня. </param>
        private void OnDayEnded(DateTime date) => ExhaustedSleep();

        /// <summary> Запрос на завершение дня по инициативе игрока (взаимодействие с кроватью). </summary>
        /// <param name="triggerTransform"> Transform объекта-триггера (кровати). </param>
        /// <param name="onCompleteCallback"> Колбэк, вызываемый после завершения процесса. </param>
        public void RequestEndDay(Transform triggerTransform, Action onCompleteCallback)
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            _onCompleteCallback = onCompleteCallback;
            _coroutineRunner.StartCoroutine(ResetPlayer(triggerTransform));
            _coroutineRunner.StartCoroutine(EndDayRoutine());
        }

        /// <summary> Принудительный сон при истощении игрока (автоматическое завершение дня). </summary>
        private void ExhaustedSleep()
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;
            _coroutineRunner.StartCoroutine(ResetPlayer(_sleepTriggerTransform, true));
            _coroutineRunner.StartCoroutine(EndDayRoutine());
        }

        /// <summary> Основная корутина завершения дня
        /// Управляет последовательностью: пауза времени → затухание → сводка → продолжение. </summary>
        /// <returns> IEnumerator для корутины. </returns>
        private IEnumerator EndDayRoutine()
        {
            WorldTime.Pause();

            // TODO: худ игры скрывается

            yield return _fader.FadeOut(Fader.FadeOutTime);

            _summaryView.Show();
            bool continuePressed = false;
            _summaryView.OnContinuePressed = () => continuePressed = true;
            _summaryView.SetSummary(SummaryView.DefaultSummaryText);
            yield return new WaitUntil(() => continuePressed);

            _onCompleteCallback?.Invoke();

            _summaryView.Hide();
            WorldTime.Unpause();

            yield return _fader.FadeIn(Fader.FadeInTime);

            _isProcessingSleep = false;
        }

        /// <summary> Корутина сброса состояния игрока: восстановление здоровья, выносливости и позиции. </summary>
        /// <param name="triggerTransform"> Transform позиции для размещения игрока. </param>
        /// <param name="exhausted"> Флаг истощения (влияет на восстановление выносливости). </param>
        /// <returns>IEnumerator для корутины</returns>
        private IEnumerator ResetPlayer(Transform triggerTransform, bool exhausted = false)
        {
            var playerStats = _playerController.GetComponent<PlayerStats>();

            var health = playerStats.GetStat<Health>();
            health.SetValue(health.MaxValue);

            var stamina = playerStats.GetStat<Stamina>();
            stamina.SetValue(exhausted ? stamina.MaxValue * _staminaMultiplier : stamina.MaxValue);

            _playerController.UpdatePosition(triggerTransform);
            yield return null;
            _locationManager.ActivatePlayerCurrentLocation();
        }
    }
}