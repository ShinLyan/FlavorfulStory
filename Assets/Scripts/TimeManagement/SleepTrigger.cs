using UnityEngine;
using Zenject;
using DG.Tweening;
using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using FlavorfulStory.UI;
using FlavorfulStory.UI.Animation;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Триггер сна - кровать, с которой игрок может взаимодействовать для завершения дня. </summary>
    public class SleepTrigger : MonoBehaviour, IInteractable
    {
        /// <summary> Менеджер завершения дня, управляющий процессом сна и переходом между днями. </summary>
        private DayEndManager _dayEndManager;
        /// <summary> Оконный сервис. </summary>
        private IWindowService _windowService;
        
        private UIOverlayFadeCoordinator _overlayCoordinator;

        /// <summary> Заголовок окна подтверждения сна. </summary>
        private const string SleepConfirmationTitle = "Bed"; // TODO: заменить на генератор/локализацию

        /// <summary> Заголовок окна подтверждения сна. </summary>
        private const string SleepConfirmationDescription = "Go to sleep?"; // TODO: заменить на генератор/локализацию
        
        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="dayEndManager"> Менеджер завершения дня. </param>
        /// <param name="windowService"> Сервис окон. </param>
        /// <param name="overlayCoordinator"> Координатор оверлея. </param>
        [Inject]
        private void Construct(
            DayEndManager dayEndManager,
            IWindowService windowService,
            UIOverlayFadeCoordinator overlayCoordinator)
        {
            _dayEndManager = dayEndManager;
            _windowService = windowService;
            _overlayCoordinator = overlayCoordinator;
        }

        #region IInteractable

        /// <summary> Описание действия с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Sleep, "to Bed");

        /// <summary> Возвращает возможность взаимодействия с объектом. </summary>
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
            window.Setup(SleepConfirmationTitle, SleepConfirmationDescription, OnSleepConfirmed, OnSleepRejected);
            window.Open();
        }

        /// <summary> Обрабатывает подтверждение сна. </summary>
        private void OnSleepConfirmed()
        {
            // TODO: сходить в _dayEndManager и переписать RequestEndDay
            _dayEndManager.RequestEndDay(null);
        }

        /// <summary> Обрабатывает отклонение сна. </summary>
        private void OnSleepRejected() { }

        /// <summary> Завершает взаимодействие с кроватью. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player) { }

        #endregion
    }
}