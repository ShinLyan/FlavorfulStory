using UnityEngine;
using Unity.Cinemachine;
using System;
using Cysharp.Threading.Tasks;
using Zenject;
using DG.Tweening;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
using FlavorfulStory.UI.Animation;

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

        /// <summary> Позиция триггера сна (кровати), используется для размещения игрока при принудительном сне. </summary>
        private readonly Vector3 _sleepTriggerPosition;

        /// <summary> Менеджер локаций для активации текущей локации игрока после сна. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Флаг, предотвращающий одновременное выполнение нескольких процессов сна. </summary>
        private bool _isProcessingSleep;

        /// <summary> Затемнение экрана при переходах между сценами. </summary>
        private readonly CanvasGroupFader _fader;

        /// <summary> Затемнение HUD интерфейса при принудительном сне. </summary>
        private readonly CanvasGroupFader _hudFader;

        /// <summary> Виртуальная камера Cinemachine для управления обзором во время переходов дня. </summary>
        private readonly CinemachineCamera _virtualCamera;

        /// <summary> Конструктор DayEndManager. </summary>
        /// <param name="summaryView"> Компонент отображения сводки дня. </param>
        /// <param name="playerController">Контроллер игрока. </param>
        /// <param name="sleepTrigger"> Триггер сна (кровать). </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        /// <param name="fader"> Компонент затемнения экрана. </param>
        /// <param name="hudFader"> Компонент затемнения HUD интерфейса. </param>
        /// <param name="virtualCamera"> Виртуальная камера Cinemachine. </param>
        public DayEndManager(SummaryView summaryView, PlayerController playerController, SleepTrigger sleepTrigger,
            LocationManager locationManager, CanvasGroupFader fader, [Inject(Id = "HUD")] CanvasGroupFader hudFader,
            CinemachineCamera virtualCamera)
        {
            _summaryView = summaryView;
            _playerController = playerController;
            _sleepTriggerPosition = sleepTrigger.transform.position;
            _locationManager = locationManager;
            _fader = fader;
            _hudFader = hudFader;
            _virtualCamera = virtualCamera;
            _isProcessingSleep = false;
        }

        /// <summary> Инициализация компонента (реализация IInitializable). </summary>
        public void Initialize() => WorldTime.OnDayEnded += OnDayEnded;

        /// <summary> Освобождение ресурсов и отписка от событий (реализация IDisposable). </summary>
        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        /// <summary> Обработчик события окончания дня из системы времени. </summary>
        /// <param name="date"> Дата окончившегося дня. </param>
        private void OnDayEnded(DateTime date) => ExhaustedSleep().Forget();

        /// <summary> Принудительный сон при истощении игрока (автоматическое завершение дня). </summary>
        private async UniTaskVoid ExhaustedSleep()
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            _hudFader.Hide();
            await EndDayRoutine();
            await RestorePlayerState(_sleepTriggerPosition, true);
            _hudFader.Show();
        }

        /// <summary> Запрос на завершение дня по инициативе игрока (взаимодействие с кроватью). </summary>
        /// <param name="triggerTransform"> Transform объекта-триггера (кровати). </param>
        /// <param name="onCompleteCallback"> Колбэк, вызываемый после завершения процесса. </param>
        public async UniTaskVoid RequestEndDay(Action onCompleteCallback)
        {
            if (_isProcessingSleep) return;
            _isProcessingSleep = true;

            WorldTime.BeginNewDay(6);

            _onCompleteCallback = onCompleteCallback;
            await EndDayRoutine();
            await RestorePlayerState(_sleepTriggerPosition);
        }

        /// <summary> Основная корутина завершения дня.
        /// Управляет последовательностью: пауза времени → затухание → сводка → продолжение. </summary>
        private async UniTask EndDayRoutine()
        {
            WorldTime.Pause();

            await _fader.Show().AsyncWaitForCompletion();
            _locationManager.EnableLocation(LocationName.RockyIsland);
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
        /// <param name="targetPosition"> Позиция цели для спавна. </param>
        /// <param name="isExhausted"> Флаг истощения (влияет на восстановление выносливости). </param>
        private async UniTask RestorePlayerState(Vector3 targetPosition, bool isExhausted = false)
        {
            _playerController.RestoreStatsAfterSleep(isExhausted);
            _playerController.SetPosition(targetPosition);

            await UniTask.Yield();
            _locationManager.UpdateActiveLocation();

            await _fader.Hide().AsyncWaitForCompletion();
        }

        /// <summary> Сброс состояния виртуальной камеры для правильного позиционирования после смены локации.
        /// Отключает и включает камеру для принудительного обновления её состояния. </summary>
        private async UniTask ResetCamera()
        {
            if (_virtualCamera)
            {
                _virtualCamera.enabled = false;
                await UniTask.Yield();
                _virtualCamera.enabled = true;
            }
        }
    }
}