using FlavorfulStory.Actions.ActionItems;
using FlavorfulStory.Control;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Предмет инвентаря, который можно использовать.</summary>
    /// <remarks> Этот класс следует использовать в качестве базового. Подклассы должны реализовывать метод `Use`.</remarks>
    [CreateAssetMenu(menuName = ("FlavorfulStory/Inventory/Action Item"))]
    public class ActionItem : InventoryItem, IUsable
    {
        /// <summary> Расходуется ли предмет при использовании?</summary>
        [field: Tooltip("Расходуется ли предмет при использовании?")]
        [field: SerializeField] public bool IsConsumable { get; private set; }

        /// <summary> Расходуется ли предмет при использовании?</summary>
        [field: Tooltip("Тип действия для использования предмета (ЛКМ или ПКМ).")]
        [field: SerializeField] public UseActionType UseActionType { get; private set; }

        /// <summary> Использование предмета.</summary>
        /// <remarks> Переопределите для обеспечения функциональности.</remarks>
        /// <param name="player"> Контроллер игрока.</param>
        public virtual void Use(PlayerController player)
        {
            Debug.Log($"Using action: {this}");
        }
    }
}