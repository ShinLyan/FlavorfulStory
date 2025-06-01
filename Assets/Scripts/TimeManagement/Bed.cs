using System.Collections;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI;
using UnityEngine;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Представляет кровать, с которой игрок может взаимодействовать для завершения дня. </summary>
    public class Bed : MonoBehaviour, IInteractable
    {
        /// <summary> Заголовок для отображения во всплывающей подсказке при наведении. </summary>
        [field: SerializeField]
        public string TooltipTitle { get; private set; }

        /// <summary> Описание для отображения во всплывающей подсказке при наведении. </summary>
        [field: SerializeField]
        public string TooltipDescription { get; private set; }

        /// <summary> View для подтверждения действия "лечь спать". </summary>
        [SerializeField] private ConfirmationView _confirmationView;

        /// <summary> View для отображения сводки дня. </summary>
        [SerializeField] private SummaryView _summaryView;

        /// <summary> Позиция объекта в мировых координатах. </summary>
        public Vector3 WorldPosition => transform.position;

        private GameObject _currentModal;
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        /// <summary> Показывает View подтверждения перед сном. </summary>
        private void ShowConfirmationView()
        {
            _confirmationView.Setup(
                "Лечь спать?",
                "Вы уверены, что хотите закончить день?",
                OnSleepConfirmed,
                () => EndInteraction(_playerController)
            );
            _confirmationView.Show();
        }

        /// <summary> Обрабатывает подтверждение сна. </summary>
        private void OnSleepConfirmed()
        {
            StartCoroutine(SleepRoutine());
            _confirmationView.Hide();
        }

        /// <summary> Корутина, обрабатывающая процесс сна и завершения дня. </summary>
        private IEnumerator SleepRoutine()
        {
            yield return PersistentObject.Instance.Fader.FadeOut(Fader.FadeOutTime);

            WorldTime.ForceEndDay();
            WorldTime.Pause();

            _summaryView.Show();

            var continuePressed = false;
            _summaryView.OnContinuePressed = () => continuePressed = true;
            _summaryView.SetSummary("BEST SUMMARY EVER!!!!!!");
            yield return new WaitUntil(() => continuePressed);

            _summaryView.Hide();
            WorldTime.Unpause();

            yield return PersistentObject.Instance.Fader.FadeIn(Fader.FadeInTime);

            EndInteraction(_playerController);
        }

        #region IInteractable Implementation

        /// <summary> Возвращает возможность взаимодействия с объектом. </summary>
        public bool IsInteractionAllowed => true;

        /// <summary> Вычисляет расстояние до указанного трансформа. </summary>
        /// <param name="otherTransform"> Трансформ, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние до объекта. </returns>
        public float GetDistanceTo(Transform otherTransform)
        {
            return Vector3.Distance(transform.position, otherTransform.position);
        }

        /// <summary> Начинает взаимодействие с кроватью. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void BeginInteraction(PlayerController player) => ShowConfirmationView();

        /// <summary> Завершает взаимодействие с кроватью. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player)
        {
            player.SetBusyState(false);
            _confirmationView.Hide();
        }

        #endregion
    }
}