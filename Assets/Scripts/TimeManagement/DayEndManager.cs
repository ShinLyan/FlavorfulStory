using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Stats;
using FlavorfulStory.UI;
using FlavorfulStory.UI.Animation;
using Unity.Cinemachine;
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
        private readonly CanvasGroupFader _fader;

        private readonly CanvasGroupFader _hudFader;

        private readonly CinemachineCamera _virtualCamera;

        /// <summary> Конструктор DayEndManager. </summary>
        /// <param name="summaryView"> Компонент отображения сводки дня. </param>
        /// <param name="playerController">Контроллер игрока. </param>
        /// <param name="sleepTrigger"> Триггер сна (кровать). </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        public DayEndManager(SummaryView summaryView,
            PlayerController playerController,
            SleepTrigger sleepTrigger,
            LocationManager locationManager,
            CanvasGroupFader fader,
            [Inject(Id = "HUD")] CanvasGroupFader hudFader,
            CinemachineCamera virtualCamera)
        {
            _summaryView = summaryView;
            _playerController = playerController;
            _sleepTriggerTransform = sleepTrigger.transform;
            _locationManager = locationManager;
            _fader = fader;
            _hudFader = hudFader;
            _virtualCamera = virtualCamera;
            _isProcessingSleep = false;

            WorldTime.OnDayEnded += OnDayEnded;
        }

        /// <summary> Инициализация компонента (реализация IInitializable). </summary>
        public void Initialize() { }

        /// <summary> Освобождение ресурсов и отписка от событий (реализация IDisposable). </summary>
        public void Dispose() { WorldTime.OnDayEnded -= OnDayEnded; }

        /// <summary> Обработчик события окончания дня из системы времени. </summary>
        /// <param name="date"> Дата окончившегося дня. </param>
        private void OnDayEnded(DateTime date) => ExhaustedSleep().Forget();

        /// <summary> Запрос на завершение дня по инициативе игрока (взаимодействие с кроватью). </summary>
        /// <param name="triggerTransform"> Transform объекта-триггера (кровати). </param>
        /// <param name="onCompleteCallback"> Колбэк, вызываемый после завершения процесса. </param>
        public async UniTaskVoid RequestEndDay(Transform triggerTransform, Action onCompleteCallback)
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            _onCompleteCallback = onCompleteCallback;
            await EndDayRoutine();
            await ResetPlayer(triggerTransform);
        }

        /// <summary> Принудительный сон при истощении игрока (автоматическое завершение дня). </summary>
        private async UniTaskVoid ExhaustedSleep()
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            _hudFader.Hide();
            await EndDayRoutine();
            await ResetPlayer(_sleepTriggerTransform, true);
            _hudFader.Show();
        }

        /// <summary> Основная корутина завершения дня.
        /// Управляет последовательностью: пауза времени → затухание → сводка → продолжение. </summary>
        private async UniTask EndDayRoutine()
        {
            WorldTime.Pause();

            await _fader.Show().AsyncWaitForCompletion();
            _locationManager.EnableLocation(LocationName.RockyIsland); //TODO: включить свет на сцене
            await ResetCamera();
            _summaryView.Show();

            bool continuePressed = false;
            _summaryView.OnContinuePressed = () => continuePressed = true;
            _summaryView.SetSummary(SummaryView.DefaultSummaryText);

            await _fader.Hide().AsyncWaitForCompletion();

            await UniTask.WaitUntil(() => continuePressed);

            await _fader.Show().AsyncWaitForCompletion();
            _summaryView.Hide();
            await ResetCamera();

            _isProcessingSleep = false;
            WorldTime.Unpause();
            _onCompleteCallback?.Invoke();
        }

        /// <summary> Сброс состояния игрока: восстановление здоровья, выносливости и позиции. </summary>
        /// <param name="triggerTransform"> Transform позиции для размещения игрока. </param>
        /// <param name="exhausted"> Флаг истощения (влияет на восстановление выносливости). </param>
        private async UniTask ResetPlayer(Transform triggerTransform, bool exhausted = false)
        {
            var playerStats = _playerController.GetComponent<PlayerStats>();

            var health = playerStats.GetStat<Health>();
            health.SetValue(health.MaxValue);

            var stamina = playerStats.GetStat<Stamina>();
            stamina.SetValue(exhausted ? stamina.MaxValue * StaminaMultiplier : stamina.MaxValue);

            _playerController.UpdatePosition(triggerTransform);
            await UniTask.Yield();
            _locationManager.ActivatePlayerCurrentLocation();

            await _fader.Hide().AsyncWaitForCompletion();
        }

        private async UniTask ResetCamera() //TODO: remake, dont work
        {
            if (_virtualCamera != null)
            {
                _virtualCamera.enabled = false;
                await UniTask.Yield();
                _virtualCamera.enabled = true;
            }
        }
    }
}