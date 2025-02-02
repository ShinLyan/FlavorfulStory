using UnityEngine;

namespace FlavorfulStory.InventorySystem.EquipmentSystem
{
    /// <summary> Экипируемый предмет, предназначенный для использования в определенном слоте экипировки. </summary>
    [CreateAssetMenu(menuName = ("FlavorfulStory/Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        /// <summary> Слот экипировки, в который может быть помещен предмет. </summary>
        [field: Tooltip("Слот экипировки, в который может быть помещен предмет.")]
        [field: SerializeField] public EquipmentType AllowedEquipmentLocation { get; private set; }
    }
}