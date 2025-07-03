using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    public class QuestRewardSlotView : MonoBehaviour, IItemHolder
    {
        /// <summary> Отображение стака предмета. </summary>
        [SerializeField] private ItemStackView _itemStackView;

        private InventoryItem _rewardItem;

        public InventoryItem GetItem() => _rewardItem;

        public void UpdateView(InventoryItem rewardItem, int rewardNumber)
        {
            _rewardItem = rewardItem;
            _itemStackView.UpdateView(rewardItem, rewardNumber);
        }
    }
}