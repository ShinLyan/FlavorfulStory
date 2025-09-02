using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using FlavorfulStory.Windows;
using FlavorfulStory.Windows.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Триггер сна — позволяет игроку завершить день через взаимодействие. </summary>
    public class SleepTrigger : MonoBehaviour, IInteractable
    {
        /// <summary> Менеджер завершения дня, управляющий процессом сна и переходом между днями. </summary>
        private DayEndManager _dayEndManager;

        /// <summary> Сервис окон. </summary>
        private IWindowService _windowService;

        /// <summary> Сервис для управления точками возрождения игрока и регистрации триггеров сна. </summary>
        private PlayerSpawnService _playerSpawnService;

        /// <summary> Заголовок окна подтверждения сна. </summary>
        private const string SleepConfirmationTitle = "Bed"; // TODO: локализация

        /// <summary> Описание окна подтверждения сна. </summary>
        private const string SleepConfirmationDescription = "Go to sleep?";

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="dayEndManager"> Менеджер завершения дня. </param>
        /// <param name="windowService"> Сервис окон. </param>
        /// <param name="playerSpawnService"> Сервис спавна игрока. </param>
        [Inject]
        private void Construct(DayEndManager dayEndManager, IWindowService windowService,
            PlayerSpawnService playerSpawnService)
        {
            _dayEndManager = dayEndManager;
            _windowService = windowService;
            _playerSpawnService = playerSpawnService;
        }

        /// <summary> Описание действия с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Sleep, "to Bed");

        /// <summary> Можно ли взаимодействовать с объектом? </summary>
        public bool IsInteractionAllowed => true;

        /// <summary> Вычисляет расстояние до указанного трансформа. </summary>
        /// <param name="otherTransform"> Трансформ, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние до объекта. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Начинает взаимодействие с кроватью. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void BeginInteraction(PlayerController player)
        {
            var window = _windowService.GetWindow<ConfirmationWindow>();
            window.Setup(SleepConfirmationTitle, SleepConfirmationDescription,
                () => OnSleepConfirmed(player), () => { });
            window.Open();
        }

        /// <summary> Обрабатывает подтверждение сна. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        private void OnSleepConfirmed(PlayerController player)
        {
            _playerSpawnService.RegisterLastUsedBed(this);
            _dayEndManager.RequestEndDay(() => EndInteraction(player));
        }

        /// <summary> Завершает взаимодействие с кроватью. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player) => player.SetBusyState(false);
    }
}