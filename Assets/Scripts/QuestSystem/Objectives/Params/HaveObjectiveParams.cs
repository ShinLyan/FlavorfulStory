using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Параметры цели на то, чтобы иметь в инвентаре определённое количество предметов. </summary>
    [Serializable]
    public class HaveObjectiveParams : ObjectiveParamsBase
    {
        /// <summary> Предмет, который требуется собрать. </summary>
        [field: Tooltip("Предмет, который требуется собрать."), SerializeField]
        public InventoryItem InventoryItem { get; private set; }

        /// <summary> Требуемое количество предметов. </summary>
        [field: Tooltip("Требуемое количество предметов."), SerializeField, Min(1f)]
        public int RequiredAmount { get; private set; } = 1;

        /// <summary> Проверяет, собрано ли нужное количество предметов. </summary>
        /// <param name="context"> Контекст выполнения цели. </param>
        /// <param name="eventData"> Событие, содержащее собранный предмет. </param>
        /// <returns> True, если цель выполнена. </returns>
        protected override bool ShouldComplete(QuestExecutionContext context, object eventData)
        {
            if (eventData is InventoryItem item && item != InventoryItem) return false;
            return context.PlayerInventory.GetItemNumber(InventoryItem) >= RequiredAmount;
        }
    }
}