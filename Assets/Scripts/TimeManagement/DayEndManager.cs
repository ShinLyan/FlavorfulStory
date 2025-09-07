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
        /// <summary> Контроллер игрока. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Менеджер локаций. </summary>
        private readonly LocationManager _locationManager;

        /// <summary> Сервис окон. </summary>
        private readonly IWindowService _windowService;

        /// <summary> Сервис, отвечающий за управление точками появления игрока. </summary>
        private readonly PlayerSpawnService _playerSpawnService;

        /// <summary> Сигнальная шина. </summary>
        private readonly SignalBus _signalBus;

        /// <summary> Флаг выполнения процесса сна. </summary>
        private bool _isProcessingSleep;

        /// <summary> Инициализирует менеджер окончания дня. </summary>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="locationManager"> Менеджер управления локациями. </param>
        /// <param name="windowService"> Сервис окон. </param>
        /// <param name="playerSpawnService"> Сервис для спавна игрока. </param>
        /// <param name="signalBus"> Сигнальная шина. </param>
        public DayEndManager(PlayerController playerController, LocationManager locationManager,
            IWindowService windowService, PlayerSpawnService playerSpawnService, SignalBus signalBus)
        {
            _playerController = playerController;
            _locationManager = locationManager;
            _windowService = windowService;
            _playerSpawnService = playerSpawnService;
            _signalBus = signalBus;
        }

        /// <summary> Подписывается на события. </summary>
        public void Initialize() => WorldTime.OnDayEnded += OnDayEnded;

        /// <summary> Отписывается от событий. </summary>
        public void Dispose() => WorldTime.OnDayEnded -= OnDayEnded;

        /// <summary> Обрабатывает завершение дня, инициируя процесс сна. </summary>
        /// <param name="_"> Время окончания дня. </param>
        private void OnDayEnded(DateTime _) => ProcessSleepAsync(true).Forget();

        /// <summary> Запрашивает завершение дня вручную, с последующим колбэком. </summary>
        /// <param name="onComplete"> Метод, вызываемый после завершения всех процедур сна. </param>
        public void RequestEndDay(Action onComplete) =>
            ProcessSleepAsync(false).ContinueWith(() => onComplete?.Invoke()).Forget();

        /// <summary> Выполняет процесс сна: смена дня или сигнал об истощении,
        /// показ окна, восстановление игрока. </summary>
        /// <param name="isExhausted"> Является ли сон результатом истощения. </param>
        private async UniTask ProcessSleepAsync(bool isExhausted)
        {
            if (_isProcessingSleep) return;

            _isProcessingSleep = true;

            if (!isExhausted)
                WorldTime.Instance.BeginNewDay();
            else
                _signalBus.Fire(new ExhaustedSleepSignal());

            await ExecuteSleepSequence(isExhausted);
            _isProcessingSleep = false;
        }

        /// <summary> Выполняет полную последовательность сна: активация локации,
        /// окно итога, восстановление игрока. </summary>
        /// <param name="isExhausted"> Истощён ли игрок. </param>
        private async UniTask ExecuteSleepSequence(bool isExhausted)
        {
            _locationManager.EnableLocation(LocationName.RockyIsland);

            await ShowSummaryWindowAsync();

            _windowService.CloseWindow<SummaryWindow>();

            RestorePlayer(isExhausted);
        }

        /// <summary> Восстанавливает игрока после сна и перемещает его на точку спавна. </summary>
        /// <param name="isExhausted"> Был ли сон принудительным из-за истощения. </param>
        private void RestorePlayer(bool isExhausted)
        {
            var spawnPosition = _playerSpawnService.GetSpawnPosition();

            _playerController.RestoreStatsAfterSleep(isExhausted);
            _playerController.SetPosition(spawnPosition);
            _locationManager.UpdateActiveLocation();
        }

        /// <summary> Отображает окно итогов дня и ожидает его закрытия. </summary>
        private async UniTask ShowSummaryWindowAsync()
        {
            var window = _windowService.GetWindow<SummaryWindow>();
            window.Setup(SummaryWindow.DefaultSummaryText);
            _windowService.OpenWindow<SummaryWindow>();

            await WaitForWindowCloseAsync(window);
        }

        /// <summary> Ожидает, пока окно SummaryWindow будет закрыто пользователем. </summary>
        /// <param name="window"> Ссылка на окно итогов дня. </param>
        /// <returns> Задача, завершающаяся после закрытия окна. </returns>
        private static UniTask WaitForWindowCloseAsync(SummaryWindow window)
        {
            var tcs = new UniTaskCompletionSource();

            window.Closed += OnClosed;
            return tcs.Task;

            void OnClosed()
            {
                window.Closed -= OnClosed;
                tcs.TrySetResult();
            }
        }
    }
}