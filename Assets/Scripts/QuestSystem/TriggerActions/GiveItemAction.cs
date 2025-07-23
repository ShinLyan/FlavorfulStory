using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.TriggerActions
{
    /// <summary> Действие, выдающее игроку указанный предмет. </summary>
    [Serializable]
    public class GiveItemAction : QuestTriggerAction
    {
        /// <summary> Предмет и его количество, которые нужно выдать. </summary>
        [SerializeField] private ItemStack _itemToGive;

        /// <summary> Выдает игроку указанный предмет в контексте квеста. </summary>
        /// <param name="context"> Контекст выполнения квеста. </param>
        public override void Execute(QuestExecutionContext context)
        {
            bool isSuccess = context.Inventory.TryAddToFirstAvailableSlot(_itemToGive.Item, _itemToGive.Number);
            if (!isSuccess) context.ItemDropService.Drop(_itemToGive, context.PlayerSpeaker.transform.position);
        }
    }
}