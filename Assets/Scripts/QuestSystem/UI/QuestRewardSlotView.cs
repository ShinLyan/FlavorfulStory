using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.TooltipSystem;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Представление слота награды за квест. </summary>
    [RequireComponent(typeof(ItemTooltipSpawner))]
    public class QuestRewardSlotView : MonoBehaviour, IItemHolder
    {
        /// <summary> Отображение стака предмета. </summary>
        [SerializeField] private ItemStackView _itemStackView;

        /// <summary> Хранимый предмет награды. </summary>
        private InventoryItem _rewardItem;

        /// <summary> Получить предмет для отображения в тултипе. </summary>
        /// <returns> Предмет для отображения в тултипе. </returns>
        public InventoryItem GetItem() => _rewardItem;

        /// <summary> Обновляет отображение слота награды с новым предметом и количеством. </summary>
        /// <param name="itemStack"> Предмет и его количество. </param>
        public void UpdateView(ItemStack itemStack)
        {
            _rewardItem = itemStack.Item;
            _itemStackView.UpdateView(itemStack);
        }
    }
}