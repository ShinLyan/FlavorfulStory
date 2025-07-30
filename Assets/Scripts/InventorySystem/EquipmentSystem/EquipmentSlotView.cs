using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.InventorySystem.UI.Dragging;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.EquipmentSystem
{
    /// <summary> Слот для отображения и управления экипировкой. </summary>
    public class EquipmentSlotView : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        /// <summary> Отображение стака предмета. </summary>
        [SerializeField] private ItemStackView _itemStackView;

        /// <summary> Тип экипировки, связанный со слотом. </summary>
        [SerializeField] private EquipmentType _equipmentType;

        /// <summary> Экипировка игрока. </summary>
        private Equipment _equipment;

        /// <summary> Внедрить зависимости Zenject. </summary>
        /// <param name="equipment"> Экипировка игрока. </param>
        [Inject]
        private void Construct(Equipment equipment) => _equipment = equipment;

        /// <summary> Подписка на обновление UI. </summary>
        private void Awake() => _equipment.EquipmentUpdated += UpdateView;

        /// <summary> Первичное обновление UI экипировки. </summary>
        private void Start() => UpdateView();

        /// <summary> Обновление отображения слота экипировки. </summary>
        private void UpdateView() => _itemStackView.UpdateView(new ItemStack { Item = GetItem(), Number = 1 });

        /// <summary> Получение максимально допустимого количества предметов, которые могут быть добавлены в слот. </summary>
        /// <param name="item"> Проверяемый предмет. </param>
        /// <returns> 1, если предмет подходит для экипировки, иначе 0. </returns>
        public int GetMaxAcceptableItemsNumber(InventoryItem item)
        {
            if (item is not EquipableItem equipableItem ||
                equipableItem.AllowedEquipmentLocation != _equipmentType || GetItem())
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
        public void RemoveItems(int number) => _equipment.RemoveItem(_equipmentType);
    }
}