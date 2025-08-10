using FlavorfulStory.Actions;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Объект-сундук, с которым можно взаимодействовать для обмена предметами. </summary>
    [RequireComponent(typeof(Inventory))]
    public class InventoryContainer : MonoBehaviour, IInteractable
    {
        /// <summary> Окно для обмена предметами между инвентарями. </summary>
        private InventoryExchangeWindow _exchangeWindow;

        /// <summary> Провайдер инвентарей. </summary>
        private IInventoryProvider _inventoryProvider;

        /// <summary> Инвентарь сундука. </summary>
        private Inventory _inventory;

        /// <summary> Тултип открытия сундука. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Open, "Chest");

        /// <summary> Разрешено ли взаимодействие с объектом. </summary>
        public bool IsInteractionAllowed => true;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="exchangeWindow"> Окно обмена инвентарями. </param>
        /// <param name="inventoryProvider"> Провайдер для получения других инвентарей (например, игрока). </param>
        [Inject]
        private void Construct(InventoryExchangeWindow exchangeWindow, IInventoryProvider inventoryProvider)
        {
            _exchangeWindow = exchangeWindow;
            _inventoryProvider = inventoryProvider;
        }

        /// <summary> Инициализация сундука. </summary>
        /// <remarks> Получение ссылки на инвентарь сундука. </remarks>
        private void Awake() => _inventory = GetComponent<Inventory>();

        /// <summary> Расстояние до игрока. </summary>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Начать взаимодействие — открыть окно обмена между игроком и сундуком. </summary>
        public void BeginInteraction(PlayerController player)
        {
            player.SetBusyState(true);
            _exchangeWindow.Show(_inventoryProvider.GetPlayerInventory(), _inventory, () => player.SetBusyState(false));
        }

        /// <summary> Завершить взаимодействие. </summary>
        public void EndInteraction(PlayerController player) { }
    }
}