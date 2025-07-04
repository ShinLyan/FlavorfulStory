using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Представление слота награды за квест. </summary>
    public class QuestRewardSlotView : MonoBehaviour, IItemHolder
    {
        /// <summary> Отображение стака предмета. </summary>
        [SerializeField] private ItemStackView _itemStackView;

        /// <summary> Хранимый предмет награды. </summary>
        private InventoryItem _rewardItem;

        /// <summary> Возвращает предмет, находящийся в слоте награды. </summary>
        public InventoryItem GetItem() => _rewardItem;

        /// <summary> Обновляет отображение слота награды с новым предметом и количеством. </summary>
        /// <param name="rewardItem"> Предмет награды. </param>
        /// <param name="rewardNumber"> Количество предметов. </param>
        public void UpdateView(InventoryItem rewardItem, int rewardNumber)
        {
            _rewardItem = rewardItem;
            _itemStackView.UpdateView(rewardItem, rewardNumber);
        }
    }
}