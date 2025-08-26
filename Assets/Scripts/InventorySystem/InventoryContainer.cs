using FlavorfulStory.Actions;
using FlavorfulStory.Infrastructure.Services.WindowService;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.PlacementSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Объект-сундук, с которым можно взаимодействовать для обмена предметами. </summary>
    [RequireComponent(typeof(Inventory), typeof(Collider))]
    public class InventoryContainer : MonoBehaviour, IInteractable, ICanBeDismantled
    {
        /// <summary> Провайдер инвентарей. </summary>
        private IInventoryProvider _inventoryProvider;

        /// <summary> Провайдер инвентарей. </summary>
        private PlayerController _playerController;

        /// <summary> Сервис окон. </summary>
        private IWindowService _windowService;

        /// <summary> Инвентарь сундука. </summary>
        private Inventory _inventory;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="inventoryProvider"> Провайдер для получения других инвентарей (например, игрока). </param>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="windowService"> Сервис окон. </param>
        [Inject]
        private void Construct(IInventoryProvider inventoryProvider, PlayerController playerController,
            IWindowService windowService)
        {
            _inventoryProvider = inventoryProvider;
            _playerController = playerController;
            _windowService = windowService;
        }

        /// <summary> Инициализация сундука. </summary>
        /// <remarks> Получение ссылки на инвентарь сундука. </remarks>
        private void Awake() => _inventory = GetComponent<Inventory>();

        /// <summary>
        /// 
        /// </summary>
        private void Start() =>
            _windowService.GetWindow<InventoryExchangeWindow>().Closed += () => _playerController.SetBusyState(false);
        // TODO: Вынести в `InventoryExchangeWindow`

        #region IInteractable

        /// <summary> Тултип открытия сундука. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Open, "Chest");

        /// <summary> Разрешено ли взаимодействие с объектом. </summary>
        public bool IsInteractionAllowed => true;

        /// <summary> Расстояние до игрока. </summary>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Начать взаимодействие — открыть окно обмена между игроком и сундуком. </summary>
        public void BeginInteraction(PlayerController player)
        {
            player.SetBusyState(true);
            _windowService.GetWindow<InventoryExchangeWindow>()
                .Setup(_inventory, _inventoryProvider.GetPlayerInventory());
            _windowService.OpenWindow<InventoryExchangeWindow>();
        }

        /// <summary> Завершить взаимодействие. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public void EndInteraction(PlayerController player) { }

        #endregion

        #region ICanBeDismantled

        /// <summary> Можно ли в данный момент разрушить объект? </summary>
        public bool CanBeDismantled => _inventory.IsEmpty;

        /// <summary> Причина, по которой объект нельзя разрушить. </summary>
        /// <remarks> Используется для отображения уведомлений игроку. </remarks>
        public string DismantleDeniedReason => "Retrieve items from chest to remove it"; // TODO: Localize

        #endregion
    }
}