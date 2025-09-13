using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using FlavorfulStory.Windows;
using FlavorfulStory.Windows.UI;
using UnityEngine;
using Zenject;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.Shop
{
    /// <summary> Компонент для управления состоянием магазина. </summary>
    public class ShopLock : MonoBehaviour, IInteractable
    {
        /// <summary> Шина сигналов для коммуникации между системами. </summary>
        private SignalBus _signalBus;

        /// <summary> Сервис для управления окнами. </summary>
        private IWindowService _windowService;

        /// <summary> Текущее состояние магазина (открыт/закрыт). </summary>
        public bool IsOpen { get; private set; }

        /// <summary> Время последнего выполненного действия. </summary>
        private float _lastActionTime;

        /// <summary> Следующее доступное время для действия. </summary>
        private DateTime _nextAvailableTime;

        /// <summary> Флаг, указывающий что магазин был закрыт игроком. </summary>
        private bool _closedByPlayer;

        /// <summary> Время кулдауна в минутах. </summary>
        private const float CooldownMinutes = 30f;

        /// <summary> Внедрение зависимостей через Zenject. </summary>
        /// <param name="signalBus"> Шина сигналов. </param>
        /// <param name="windowService"> Сервис окон. </param>
        [Inject]
        private void Construct(SignalBus signalBus, IWindowService windowService)
        {
            _signalBus = signalBus;
            _windowService = windowService;
        }

        /// <summary> Подписка на события при включении объекта. </summary>
        private void OnEnable() => _signalBus.Subscribe<ShopStateChangedSignal>(OnShopStateChanged);

        /// <summary> Отписка от событий при выключении объекта. </summary>
        private void OnDisable() => _signalBus.Unsubscribe<ShopStateChangedSignal>(OnShopStateChanged);

        /// <summary> Обработчик изменения состояния магазина. </summary>
        /// <param name="signal"> Сигнал с новым состоянием магазина. </param>
        private void OnShopStateChanged(ShopStateChangedSignal signal)
        {
            if (_closedByPlayer && !signal.ChangedByPlayer) return;
            IsOpen = signal.IsOpen;
        }

        /// <summary> Подтверждение закрытия магазина. </summary>
        private void ConfirmClose()
        {
            _closedByPlayer = true;
            _signalBus.Fire(new ShopStateChangedSignal(false, true));
            _nextAvailableTime = GetRoundedTimeWithCooldown(CooldownMinutes);
        }

        /// <summary> Подтверждение открытия магазина. </summary>
        private void ConfirmOpen()
        {
            _closedByPlayer = false;
            _signalBus.Fire(new ShopStateChangedSignal(true, true));
            _nextAvailableTime = GetRoundedTimeWithCooldown(CooldownMinutes);
        }

        /// <summary> Получает округленное время кулдауна. </summary>
        /// <param name="cooldownMinutes"> Время кулдауна в минутах. </param>
        /// <returns> Округленное время кулдауна. </returns>
        private static DateTime GetRoundedTimeWithCooldown(float cooldownMinutes)
        {
            var rawTime = WorldTime.CurrentGameTime.AddMinutes(cooldownMinutes);

            int remainder = (int)rawTime.TotalMinutes % 5;

            if (remainder == 0) return rawTime;

            int add = 5 - remainder;
            return rawTime.AddMinutes(add);
        }

        #region IInteractable

        /// <summary> Данные для отображения подсказки взаимодействия. </summary>
        public ActionTooltipData ActionTooltip =>
            new("E", IsOpen ? ActionType.Close : ActionType.Open, "Store");

        /// <summary> Проверка кулдауна для открытия/закрытия. </summary>
        public bool IsInteractionAllowed => true;

        /// <summary> Получить расстояние до указанного объекта. </summary>
        /// <param name="otherTransform"> Трансформ целевого объекта. </param>
        /// <returns> Расстояние между объектами. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        /// <summary> Начать взаимодействие с магазином. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void BeginInteraction(PlayerController player)
        {
            if (WorldTime.CurrentGameTime.TotalMinutes < _nextAvailableTime.TotalMinutes)
            {
                _signalBus.Fire(new UnableToChangeShopStateSignal(IsOpen, _nextAvailableTime));
                EndInteraction(player);
                return;
            }

            var confirmationWindow = _windowService.GetWindow<ConfirmationWindow>();
            if (IsOpen)
                confirmationWindow.Setup(
                    "Close Shop",
                    $"Are you sure you want to close the store? You will be able to reopen it after {CooldownMinutes} minutes.",
                    () =>
                    {
                        ConfirmClose();
                        confirmationWindow.Close();
                        EndInteraction(player);
                    },
                    () => EndInteraction(player)
                );
            else
                confirmationWindow.Setup(
                    "Open Shop",
                    $"Are you sure you want to open the store? You will be able to close it after {CooldownMinutes} minutes.",
                    () =>
                    {
                        ConfirmOpen();
                        confirmationWindow.Close();
                        EndInteraction(player);
                    },
                    () => EndInteraction(player)
                );

            confirmationWindow.Open();
        }

        /// <summary> Завершить взаимодействие с магазином. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player) => player.SetBusyState(false);

        #endregion
    }
}