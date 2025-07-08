using System;
using System.Linq;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Параметры цели на сбор определённого количества предметов. </summary>
    [Serializable]
    public class CollectObjectiveParams : ObjectiveParamsBase
    {
        /// <summary> Предмет, который требуется собрать. </summary>
        [field: Tooltip("Предмет, который требуется собрать."), SerializeField]
        public InventoryItem InventoryItem { get; private set; }

        /// <summary> Требуемое количество предметов. </summary>
        [field: Tooltip("Требуемое количество предметов."), SerializeField, Min(1f)]
        public int RequiredAmount { get; private set; } = 1;

        /// <summary> Проверяет выполнение условия сбора и завершает цель, если условие выполнено. </summary>
        /// <param name="questStatus"> Статус квеста. </param>
        /// <param name="context"> Контекст выполнения цели. </param>
        /// <param name="eventData"> Предмет инвентаря. </param>
        public override void CheckAndComplete(QuestStatus questStatus, ObjectiveExecutionContext context,
            object eventData = null)
        {
            if (eventData is InventoryItem item && item != InventoryItem) return;

            int count = context.Inventory.GetItemNumber(InventoryItem);
            if (count < RequiredAmount) return;

            var objective = questStatus.Quest.Objectives.FirstOrDefault(o => o.Params == this);
            if (objective != null)
                context.QuestList.CompleteObjective(questStatus, objective);
            else
                Debug.LogError($"Objective не найдено для Params {this} в квесте {questStatus.Quest.QuestName}!");
        }
    }
}