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

        /// <summary> Событие, вызываемое при обновлении списка квестов. </summary>
        public event Action OnQuestListUpdated;

        /// <summary> Перечисление всех статусов квестов. </summary>
        public IEnumerable<QuestStatus> QuestStatuses => _questStatuses;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _inventory;

        /// <summary> Компонент для выбрасывания предметов, если нет места в инвентаре. </summary>
        private ItemDropper _itemDropper;

        /// <summary> Внедряет зависимости через Zenject. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="itemDropper"> Компонент для дропа предметов. </param>
        [Inject]
        private void Construct(Inventory inventory, ItemDropper itemDropper)
        {
            _inventory = inventory;
            _itemDropper = itemDropper;
        }

        /// <summary> Добавляет квест в активный список, если он еще не был добавлен. </summary>
        /// <param name="quest"> Квест для добавления. </param>
        public void AddQuest(Quest quest)
        {
            var questStatus = new QuestStatus(quest);
            if (HasQuest(questStatus)) return;

            _questStatuses.Add(questStatus);
            OnQuestListUpdated?.Invoke();
        }

        /// <summary> Проверяет, есть ли уже этот квест в списке. </summary>
        /// <param name="questStatus"> Статус квеста для проверки. </param>
        /// <returns> True, если квест уже есть в списке, иначе false. </returns>
        public bool HasQuest(QuestStatus questStatus) => _questStatuses.Contains(questStatus);

        /// <summary> Отмечает цель квеста как выполненную и выдает награду, если квест завершен. </summary>
        /// <param name="quest"> Квест, в котором нужно отметить цель. </param>
        /// <param name="objective"> Ссылка на выполненную цель. </param>
        public void CompleteObjective(Quest quest, string objective)
        {
            var questStatus = GetQuestStatus(quest);
            questStatus?.CompleteObjective(objective);
            if (questStatus != null && questStatus.IsComplete) GiveReward(quest);
        }

        /// <summary> Получает статус указанного квеста из списка. </summary>
        /// <param name="quest"> Квест для поиска. </param>
        /// <returns> Статус найденного квеста или null, если квест не найден. </returns>
        private QuestStatus GetQuestStatus(Quest quest) =>
            _questStatuses.FirstOrDefault(questStatus => questStatus.Quest == quest);

        /// <summary> Выдает награды за завершение квеста: добавляет в инвентарь или дропает на землю. </summary>
        /// <param name="quest"> Завершенный квест. </param>
        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.Rewards)
            {
                bool isSuccess = _inventory.TryAddToFirstAvailableSlot(reward.Item, reward.Number);
                // TODO: Переделать на новый itemdropper
                if (!isSuccess) _itemDropper.DropItem(reward.Item, reward.Number);
            }
        }

        #region ISaveable

        /// <summary> Сохраняет текущее состояние списка квестов. </summary>
        /// <returns> Сериализованное состояние квестов. </returns>
        public object CaptureState() => _questStatuses.Select(questStatus => questStatus.CaptureState()).ToList();

        /// <summary> Восстанавливает список квестов из сохраненного состояния. </summary>
        /// <param name="state"> Сохраненное состояние. </param>
        public void RestoreState(object state)
        {
            if (state is not List<object> stateList) return;

            _questStatuses.Clear();
            foreach (object objectState in stateList) _questStatuses.Add(new QuestStatus(objectState));
        }

        #endregion
    }
}