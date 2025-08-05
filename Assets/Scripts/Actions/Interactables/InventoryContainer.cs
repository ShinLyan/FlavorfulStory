using UnityEngine;
using Zenject;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Player;
using FlavorfulStory.TooltipSystem.ActionTooltips;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Объект-сундук, с которым можно взаимодействовать для обмена предметами. </summary>
    [RequireComponent(typeof(Inventory))]
    public class InventoryContainer : MonoBehaviour, IInteractable
    {
        /// <summary> Данные для отображения тултипа действия (открыть сундук). </summary>
        [Inject] private InventoryExchangeWindow _exchangeWindow;
        /// <summary> Провайдер инвентарей. </summary>
        [Inject] private IInventoryProvider _inventoryProvider;
        
        /// <summary> Инвентарь сундука. </summary>
        private Inventory _inventory;
        
        /// <summary> Тултип открытия сундука. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Open, "Chest");
        /// <summary> Разрешено ли взаимодействие с объектом. </summary>
        public bool IsInteractionAllowed => true;

        /// <summary> Инициализация сундука. </summary>
        /// <remarks> Получение ссылки на инввентарь сундука. </remarks>
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