using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Триггер сна - кровать, с которой игрок может взаимодействовать для завершения дня. </summary>
    public class SleepTrigger : MonoBehaviour, IInteractable
    {
        /// <summary> Окно подтверждения действия. </summary>
        private ConfirmationWindowView _confirmationWindowView;

        /// <summary> Сводка дня. </summary>
        private SummaryView _summaryView;

        /// <summary> Контроллер игрока. </summary>
        private PlayerController _playerController;

        /// <summary> Затемнение экрана при переходах между сценами. </summary>
        private Fader _fader;

        private DayEndManager _dayEndManager;

        /// <summary> Заголовок окна подтверждения сна. </summary>
        private const string SleepConfirmationTitle = "Bed"; // TODO: заменить на генератор/локализацию

        /// <summary> Заголовок окна подтверждения сна. </summary>
        private const string SleepConfirmationDescription = "Go to sleep?"; // TODO: заменить на генератор/локализацию

        /// <summary> Заголовок окна подтверждения сна. </summary>
        private const string DefaultSummaryText = "BEST SUMMARY EVER"; // TODO: заменить на генератор/локализацию

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="confirmationWindowView"> Окно подтверждения. </param>
        /// <param name="playerController"> Контроллер игрока. </param>
        [Inject]
        private void Construct(ConfirmationWindowView confirmationWindowView,
            DayEndManager dayEndManager,
            PlayerController playerController)
        {
            _confirmationWindowView = confirmationWindowView;
            _playerController = playerController;
            _dayEndManager = dayEndManager;
        }

        /// <summary> Показывает View подтверждения перед сном. </summary>
        private void ShowConfirmationView()
        {
            _confirmationWindowView.Setup(SleepConfirmationTitle, SleepConfirmationDescription,
                OnSleepConfirmed, () => EndInteraction(_playerController));
            _confirmationWindowView.Show();
            _playerController.SetBusyState(true);
        }

        /// <summary> Обрабатывает подтверждение сна. </summary>
        private void OnSleepConfirmed()
        {
            WorldTime.ForceEndDay();
            _dayEndManager.RequestEndDay(() => EndInteraction(_playerController));
        }


        #region IInteractable

        /// <summary> Действие игрока по отношению к объекту. </summary>
        [field: SerializeField]
        public ActionDescription ActionDescription { get; private set; }

        /// <summary> Возвращает возможность взаимодействия с объектом. </summary>
        public bool IsInteractionAllowed => true;

        /// <summary> Вычисляет расстояние до указанного трансформа. </summary>
        /// <param name="otherTransform"> Трансформ, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние до объекта. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Начинает взаимодействие с кроватью. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void BeginInteraction(PlayerController player) => ShowConfirmationView();

        /// <summary> Завершает взаимодействие с кроватью. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player)
        {
            player.SetBusyState(false);
            _confirmationWindowView.Hide();
        }

        #endregion
    }
}