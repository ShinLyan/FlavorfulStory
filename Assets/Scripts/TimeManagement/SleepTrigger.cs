using DG.Tweening;
using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using FlavorfulStory.UI;
using FlavorfulStory.UI.Animation;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.TimeManagement
{
    /// <summary> Триггер сна - кровать, с которой игрок может взаимодействовать для завершения дня. </summary>
    public class SleepTrigger : MonoBehaviour, IInteractable
    {
        /// <summary> Окно подтверждения действия. </summary>
        private ConfirmationWindowView _confirmationWindowView;

        /// <summary> Менеджер завершения дня, управляющий процессом сна и переходом между днями. </summary>
        private DayEndManager _dayEndManager;

        /// <summary> Заголовок окна подтверждения сна. </summary>
        private const string SleepConfirmationTitle = "Bed"; // TODO: заменить на генератор/локализацию

        /// <summary> Заголовок окна подтверждения сна. </summary>
        private const string SleepConfirmationDescription = "Go to sleep?"; // TODO: заменить на генератор/локализацию

        /// <summary> Компонент затемнения HUD интерфейса во время взаимодействия с кроватью. </summary>
        private CanvasGroupFader _hudFader;

        /// <summary> Сервис для управления точками возрождения игрока и регистрации триггеров сна. </summary>
        private PlayerSpawnService _playerSpawnService;

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="confirmationWindowView"> Окно подтверждения. </param>
        /// <param name="dayEndManager"> Менеджер завершения дня. </param>
        /// <param name="hudFader"> Компонент затемнения HUD интерфейса. </param>
        /// <param name="playerSpawnService"> Сервис спавна игрока. </param>
        [Inject]
        private void Construct(ConfirmationWindowView confirmationWindowView, DayEndManager dayEndManager,
            [Inject(Id = "HUD")] CanvasGroupFader hudFader, PlayerSpawnService playerSpawnService)
        {
            _confirmationWindowView = confirmationWindowView;
            _dayEndManager = dayEndManager;
            _hudFader = hudFader;
            _playerSpawnService = playerSpawnService;
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
            _confirmationWindowView.Setup(SleepConfirmationTitle, SleepConfirmationDescription,
                () => OnSleepConfirmed(player), OnSleepRejected);
            _hudFader.Hide().OnComplete(() => { _confirmationWindowView.Show(); });
        }

        /// <summary> Обрабатывает подтверждение сна. </summary>
        private void OnSleepConfirmed(PlayerController player)
        {
            _playerSpawnService.RegisterLastUsedBed(this);
            _confirmationWindowView.Hide();
            _dayEndManager.RequestEndDay(() =>
            {
                _hudFader.Show();
                EndInteraction(player);
            });
        }

        /// <summary> Обрабатывает отклонение сна. </summary>
        private void OnSleepRejected()
        {
            _confirmationWindowView.Hide();
            _hudFader.Show();
        }

        /// <summary> Завершает взаимодействие с кроватью. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player) => player.SetBusyState(false);

        #endregion
    }
}