using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.EquipmentSystem
{
    /// <summary> Слот для снаряжения игрока. </summary>
    public class EquipmentSlotUI : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        /// <summary> Инонка слота инвентаря. </summary>
        [SerializeField] private InventoryItemIcon _icon;

        /// <summary> Тип снаряжения. </summary>
        [SerializeField] private EquipmentType _equipmentType;

        private EquipableItem _item;
        private Equipment _equipment;

        /// <summary> Инициализация полей. </summary>
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _equipment = player.GetComponent<Equipment>();
            _equipment.EquipmentUpdated += RedrawUI;
        }

        private void Start()
        {
            RedrawUI();
        }

        private void RedrawUI()
        {
            // TODO: FIX 1
            _icon.SetItem(GetItem(), 1);
        }

        public int GetMaxAcceptableItemsNumber(InventoryItem item)
        {
            if (item is not EquipableItem equipableItem ||
                equipableItem.AllowedEquipmentLocation != _equipmentType ||
                GetItem() != null)
                return 0;

            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
            _equipment.AddItem(_equipmentType, item as EquipableItem);
            //_icon.SetItem(item, number);
        }

        public InventoryItem GetItem() => _equipment.GetEquipmentFromType(_equipmentType);

        public int GetNumber() => GetItem() ? 1 : 0;

        public void RemoveItems(int number)
        {
            _equipment.RemoveItem(_equipmentType);
        }
    }
}