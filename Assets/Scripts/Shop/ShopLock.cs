using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using FlavorfulStory.Windows;
using FlavorfulStory.Windows.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Shop
{
    /// <summary> Компонент для управления состоянием магазина. </summary>
    public class ShopLock : MonoBehaviour, IInteractable
    {
        /// <summary> Шина сигналов для коммуникации между системами. </summary>
        private SignalBus _signalBus;

        /// <summary> Сервис для управления окнами. </summary>
        private IWindowService _windowService;

        /// <summary> Флаг, указывающий что магазин был закрыт игроком. </summary>
        private bool _closedByPlayer = true;

        /// <summary> Текущее состояние магазина (открыт/закрыт). </summary>
        public bool IsOpen { get; private set; }

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
            if (_closedByPlayer) return;
            IsOpen = signal.IsOpen;
        }

        /// <summary> Подтверждение закрытия магазина. </summary>
        private void ConfirmClose()
        {
            _closedByPlayer = true;
            _signalBus.Fire(new ShopStateChangedSignal(false));
        }

        /// <summary> Подтверждение открытия магазина. </summary>
        private void ConfirmOpen()
        {
            _closedByPlayer = false;
            _signalBus.Fire(new ShopStateChangedSignal(true));
        }

        #region IInteractable

        /// <summary> Данные для отображения подсказки взаимодействия. </summary>
        public ActionTooltipData ActionTooltip =>
            new("E", IsOpen ? ActionType.Close : ActionType.Open, "Store");

        /// <summary> Флаг, разрешено ли взаимодействие. </summary>
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
            var confirmationWindow = _windowService.GetWindow<ConfirmationWindow>();
            if (IsOpen)
                confirmationWindow.Setup(
                    "Close Shop",
                    "Are you sure you want to close the store? You will be able to reopen it after 30 minutes.",
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
                    "Are you sure you want to open the store? You will be able to close it after 5 minutes.",
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