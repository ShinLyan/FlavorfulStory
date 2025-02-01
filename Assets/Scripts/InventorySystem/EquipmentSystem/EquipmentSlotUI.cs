using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.EquipmentSystem
{
    /// <summary> Слот для отображения и управления экипировкой. </summary>
    public class EquipmentSlotUI : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        /// <summary> Отображение иконки предмета экипировки. </summary>
        [SerializeField] private InventoryItemIcon _inventoryItemIcon;

        /// <summary> Тип экипировки, связанный со слотом. </summary>
        [SerializeField] private EquipmentType _equipmentType;

        /// <summary> Экипированный предмет. </summary>
        private EquipableItem _item;

        /// <summary> Ссылка на систему экипировки игрока. </summary>
        private Equipment _equipment;

        /// <summary> Инициализация ссылки на систему экипировки и подписка на обновление UI. </summary>
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _equipment = player.GetComponent<Equipment>();
            _equipment.EquipmentUpdated += RedrawUI;
        }

        /// <summary> Первичное обновление UI экипировки. </summary>
        private void Start()
        {
            RedrawUI();
        }

        /// <summary> Обновление отображения слота экипировки. </summary>
        private void RedrawUI()
        {
            _inventoryItemIcon.SetItem(GetItem(), 1);
        }

        /// <summary> Получение максимально допустимого количества предметов, которые могут быть добавлены в слот. </summary>
        /// <param name="item"> Проверяемый предмет. </param>
        /// <returns> 1, если предмет подходит для экипировки, иначе 0. </returns>
        public int GetMaxAcceptableItemsNumber(InventoryItem item)
        {
            if (item is not EquipableItem equipableItem ||
                equipableItem.AllowedEquipmentLocation != _equipmentType ||
                GetItem() != null)
                return 0;

            return 1;
        }

        /// <summary> Добавление предмета в слот экипировки. </summary>
        /// <param name="item"> Предмет, который нужно экипировать. </param>
        /// <param name="number"> Количество добавляемых предметов (всегда 1). </param>
        public void AddItems(InventoryItem item, int number)
        {
            _equipment.AddItem(_equipmentType, item as EquipableItem);
        }

        /// <summary> Получение предмета, находящегося в текущем слоте экипировки. </summary>
        /// <returns> Экипированный предмет, если он есть, иначе null. </returns>
        public InventoryItem GetItem() => _equipment.GetEquipmentFromType(_equipmentType);

        /// <summary> Получение количества предметов в текущем слоте. </summary>
        /// <returns> 1, если предмет есть, иначе 0. </returns>
        public int GetNumber() => GetItem() ? 1 : 0;

        /// <summary> Удаление предмета из слота экипировки. </summary>
        /// <param name="number"> Количество удаляемых предметов (всегда 1). </param>
        public void RemoveItems(int number)
        {
            _equipment.RemoveItem(_equipmentType);
        }
    }
}