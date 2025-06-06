using System.Collections;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
using UnityEngine;
using UnityEngine.Serialization;

// TODO: Актуализировать под Zenject
namespace FlavorfulStory.TimeManagement
{
    /// <summary> Представляет кровать, с которой игрок может взаимодействовать для завершения дня. </summary>
    public class SleepTrigger : MonoBehaviour, IInteractable
    {
        /// <summary> View для подтверждения действия "лечь спать". </summary>
        [FormerlySerializedAs("_confirmationView")] [SerializeField]
        private ConfirmationWindowView _confirmationWindowView;

        /// <summary> View для отображения сводки дня. </summary>
        [SerializeField] private SummaryView _summaryView;

        private PlayerController _playerController;

        private const string SleepConfirmationTitle = "Лечь спать?"; // TODO: заменить на генератор/локализацию

        private const string
            SleepConfirmationDescription =
                "Вы уверены, что хотите закончить день?"; // TODO: заменить на генератор/локализацию

        private const string DefaultSummaryText = "BEST SUMMARY EVER"; // TODO: заменить на генератор/локализацию

        private void Awake()
        {
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        /// <summary> Показывает View подтверждения перед сном. </summary>
        private void ShowConfirmationView()
        {
            _confirmationWindowView.Setup(SleepConfirmationTitle, SleepConfirmationDescription,
                OnSleepConfirmed, () => EndInteraction(_playerController));
            _confirmationWindowView.Show();
        }

        /// <summary> Обрабатывает подтверждение сна. </summary>
        private void OnSleepConfirmed()
        {
            StartCoroutine(SleepRoutine());
            _confirmationWindowView.Hide();
        }

        /// <summary> Корутина, обрабатывающая процесс сна и завершения дня. </summary>
        private IEnumerator SleepRoutine()
        {
            yield return PersistentObject.Instance.Fader.FadeOut(Fader.FadeOutTime);

            WorldTime.ForceEndDay();
            WorldTime.Pause();

            _summaryView.Show();

            bool continuePressed = false;
            _summaryView.OnContinuePressed = () => continuePressed = true;
            _summaryView.SetSummary(DefaultSummaryText);
            yield return new WaitUntil(() => continuePressed);

            _summaryView.Hide();
            WorldTime.Unpause();

            yield return PersistentObject.Instance.Fader.FadeIn(Fader.FadeInTime);

            EndInteraction(_playerController);
        }

        #region IInteractable

        /// <summary> Заголовок для отображения во всплывающей подсказке при наведении. </summary>
        [field: SerializeField] public string TooltipTitle { get; private set; }

        /// <summary> Описание для отображения во всплывающей подсказке при наведении. </summary>
        [field: SerializeField] public string TooltipDescription { get; private set; }

        /// <summary> Позиция объекта в мировых координатах. </summary>
        public Vector3 WorldPosition => transform.position;

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