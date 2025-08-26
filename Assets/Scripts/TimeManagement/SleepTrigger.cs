using FlavorfulStory.Actions;
using FlavorfulStory.Infrastructure.Services.WindowService;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement.UI;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using FlavorfulStory.UI.Windows;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Триггер сна — позволяет игроку завершить день через взаимодействие. </summary>
    public class SleepTrigger : MonoBehaviour, IInteractable
    {
        private DayEndManager _dayEndManager;
        private IWindowService _windowService;
        private PlayerSpawnService _playerSpawnService;

        private const string SleepConfirmationTitle = "Bed"; // TODO: локализация
        private const string SleepConfirmationDescription = "Go to sleep?";

        [Inject]
        private void Construct(
            DayEndManager dayEndManager,
            IWindowService windowService,
            PlayerSpawnService playerSpawnService)
        {
            _dayEndManager = dayEndManager;
            _windowService = windowService;
            _playerSpawnService = playerSpawnService;
        }

        public ActionTooltipData ActionTooltip => new("E", ActionType.Sleep, "to Bed");
        public bool IsInteractionAllowed => true;

        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        public void BeginInteraction(PlayerController player)
        {
            var window = _windowService.GetWindow<ConfirmationWindow>();
            window.Setup(SleepConfirmationTitle, SleepConfirmationDescription,
                () => OnSleepConfirmed(player), OnSleepRejected);
            window.Open();
        }

        private void OnSleepConfirmed(PlayerController player)
        {
            _playerSpawnService.RegisterLastUsedBed(this);
            _dayEndManager.RequestEndDay(() => EndInteraction(player));
        }

        private void OnSleepRejected() { }

        public void EndInteraction(PlayerController player) => player.SetBusyState(false);
    }
}
