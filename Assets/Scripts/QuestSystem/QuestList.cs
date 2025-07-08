using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Core;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Хранит список активных квестов игрока. </summary>
    public class QuestList : MonoBehaviour, IPredicateEvaluator, ISaveable
    {
        // TODO: Нужно выполненные квесты отсюда переносить в отдельный список, т.к. при проверках на срабатывание триггеров, проходятся по всем квестам
        /// <summary> Список статусов всех квестов игрока. </summary>
        [SerializeField] private List<QuestStatus> _questStatuses;

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
            if (HasQuest(quest)) return;

            var questStatus = new QuestStatus(quest);
            _questStatuses.Add(questStatus);
            OnQuestListUpdated?.Invoke();

            // TODO: Заменить на нормальную логику NotificationManager
            FindFirstObjectByType<QuestNotificationView>(FindObjectsInactive.Include).Show(quest.QuestName);
        }

        /// <summary> Проверяет, есть ли уже этот квест в списке. </summary>
        /// <param name="quest"> Квест для проверки. </param>
        /// <returns> True, если квест уже есть в списке, иначе false. </returns>
        public bool HasQuest(Quest quest) => GetQuestStatus(quest) != null;

        /// <summary> Отмечает цель квеста как выполненную и выдает награду, если квест завершен. </summary>
        /// <param name="questStatus"> Квест, в котором нужно отметить цель. </param>
        /// <param name="objective"> Ссылка на выполненную цель. </param>
        public void CompleteObjective(QuestStatus questStatus, QuestObjective objective)
        {
            if (questStatus == null || objective == null ||
                questStatus.IsObjectiveComplete(objective.Reference))
                return;

            questStatus.CompleteObjective(objective.Reference);

            if (questStatus.IsComplete && !questStatus.IsRewardGiven) CompleteQuest(questStatus);
        }

        /// <summary> Получает статус указанного квеста из списка. </summary>
        /// <param name="quest"> Квест для поиска. </param>
        /// <returns> Статус найденного квеста или null, если квест не найден. </returns>
        private QuestStatus GetQuestStatus(Quest quest) =>
            _questStatuses.FirstOrDefault(questStatus => questStatus.Quest == quest);

        private void CompleteQuest(QuestStatus questStatus)
        {
            Debug.Log($"Complete Quest {questStatus.Quest.QuestName}");

            questStatus.MarkRewardGiven();
            GiveReward(questStatus.Quest.Rewards);
        }

        /// <summary> Выдает награды за завершение квеста: добавляет в инвентарь или дропает на землю. </summary>
        /// <param name="rewards"> Награды за завершение квеста. </param>
        private void GiveReward(IEnumerable<QuestReward> rewards)
        {
            foreach (var reward in rewards)
            {
                bool isSuccess = _inventory.TryAddToFirstAvailableSlot(reward.Item, reward.Number);
                // TODO: Переделать на новый itemdropper
                if (!isSuccess) _itemDropper.DropItem(reward.Item, reward.Number);
            }
        }

        #region ISaveable

        /// <summary> Сохраняет текущее состояние списка квестов. </summary>
        /// <returns> Сериализованное состояние квестов. </returns>
        public object CaptureState() =>
            _questStatuses.Select(questStatus => questStatus.CaptureState()).ToList();

        /// <summary> Восстанавливает список квестов из сохраненного состояния. </summary>
        /// <param name="state"> Сохраненное состояние. </param>
        public void RestoreState(object state)
        {
            if (state is not List<object> stateList) return;

            _questStatuses.Clear();
            foreach (object objectState in stateList) _questStatuses.Add(new QuestStatus(objectState));
        }

        #endregion

        #region IPredicateEvaluator

        /// <summary> Оценивает заданный предикат с параметрами. </summary>
        /// <param name="predicate"> Имя предиката для проверки. </param>
        /// <param name="parameters"> Массив параметров для предиката. </param>
        /// <returns> True, если условие выполнено; false, если не выполнено;
        /// null, если предикат не поддерживается. </returns>
        public bool? Evaluate(string predicate, string[] parameters) => predicate switch
        {
            "HasQuest" => HasQuest(Quest.GetByName(parameters[0])),
            "CompletedQuest" => GetQuestStatus(Quest.GetByName(parameters[0]))?.IsComplete,
            _ => null
        };

        #endregion
    }
}