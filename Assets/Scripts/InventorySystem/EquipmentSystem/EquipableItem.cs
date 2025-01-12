using UnityEngine;

namespace FlavorfulStory.InventorySystem.EquipmentSystem
{
    /// <summary> Предмет инвентаря, которым игрок может экипироваться. Оружие может быть его подклассом. </summary>
    [CreateAssetMenu(menuName = ("FlavorfulStory/Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        /// <summary> Куда разрешается класть этот предмет. </summary>
        [field: Tooltip("Куда нам разрешается класть этот предмет.")]
        [field: SerializeField] public EquipmentType AllowedEquipmentLocation { get; private set; }
    }
}