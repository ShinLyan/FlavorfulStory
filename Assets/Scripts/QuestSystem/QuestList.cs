using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Хранит список активных квестов игрока. </summary>
    public class QuestList : MonoBehaviour, ISaveable
    {
        /// <summary> Список статусов всех квестов игрока. </summary>
        private readonly List<QuestStatus> _questStatuses = new();

        public event Action OnQuestListUpdated;

        /// <summary> Перечисление всех статусов квестов. </summary>
        public IEnumerable<QuestStatus> QuestStatuses => _questStatuses;

        private Inventory _inventory;

        private ItemDropper _itemDropper;

        [Inject]
        private void Construct(Inventory inventory, ItemDropper itemDropper)
        {
            _inventory = inventory;
            _itemDropper = itemDropper;
        }

        public void AddQuest(Quest quest)
        {
            var questStatus = new QuestStatus(quest);
            if (HasQuest(questStatus)) return;

            _questStatuses.Add(questStatus);
            OnQuestListUpdated?.Invoke();
        }

        public bool HasQuest(QuestStatus questStatus) => _questStatuses.Contains(questStatus);

        public void CompleteObjective(Quest quest, string objective)
        {
            var questStatus = GetQuestStatus(quest);
            questStatus?.CompleteObjective(objective);
            if (questStatus.IsComplete) GiveReward(quest);
        }

        private QuestStatus GetQuestStatus(Quest quest) =>
            _questStatuses.FirstOrDefault(questStatus => questStatus.Quest == quest);

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.Rewards)
            {
                bool isSuccess = _inventory.TryAddToFirstAvailableSlot(reward.Item, reward.Number);
                // TODO
                if (!isSuccess) _itemDropper.DropItem(reward.Item, reward.Number);
            }
        }

        #region ISaveable

        public object CaptureState() => _questStatuses.Select(questStatus => questStatus.CaptureState()).ToList();

        public void RestoreState(object state)
        {
            if (state is not List<object> stateList) return;

            _questStatuses.Clear();
            foreach (object objectState in stateList) _questStatuses.Add(new QuestStatus(objectState));
        }

        #endregion
    }
}