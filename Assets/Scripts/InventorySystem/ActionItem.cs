using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Предмет инвентаря, который можно использовать.</summary>
    /// <remarks> Этот класс следует использовать в качестве базового. Подклассы должны реализовывать метод `Use`.</remarks>
    [CreateAssetMenu(menuName = ("FlavorfulStory/Inventory/Action Item"))]
    public class ActionItem : InventoryItem
    {
        /// <summary> Расходуется ли предмет при использовании?</summary>
        [field: Tooltip("Расходуется ли предмет при использовании?")]
        [field: SerializeField] public bool IsConsumable { get; private set; }

        /// <summary> Использование предмета.</summary>
        /// <remarks> Переопределите для обеспечения функциональности.</remarks>
        public virtual void Use()
        {
            Debug.Log($"Using action: {this}");
        }
    }
}