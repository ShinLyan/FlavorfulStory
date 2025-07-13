using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.TriggerActions
{
    /// <summary> Действие, выдающее игроку указанный предмет. </summary>
    [Serializable]
    public class GiveItemAction : QuestTriggerAction
    {
        /// <summary> Предмет, который нужно выдать. </summary>
        [Tooltip("Предмет, который нужно выдать.")]
        [SerializeField] private InventoryItem _item;

        /// <summary> Количество предметов, которое нужно выдать. </summary>
        [Tooltip("Количество предметов, которое нужно выдать."), Min(1f)]
        [SerializeField] private int _amount = 1;

        /// <summary> Выдает игроку указанный предмет в контексте квеста. </summary>
        /// <param name="context"> Контекст выполнения квеста. </param>
        public override void Execute(QuestExecutionContext context)
        {
            bool isSuccess = context.Inventory.TryAddToFirstAvailableSlot(_item, _amount);
            // TODO: Переделать на новый itemdropper
            // if (!isSuccess) _itemDropper.DropItem(reward.Item, reward.Number);
        }
    }
}