using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Предмет инвентаря, который можно разместить в игровом мире (мебель, здание и т.д.) </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Placeable Item")]
    public class PlaceableItem : InventoryItem
    {
        /// <summary> Префаб объекта, который будет размещён в мире. </summary>
        [field: Header("Placeable Properties")]
        [field: Tooltip("Префаб объекта, который будет размещён в мире."), SerializeField]
        public PlaceableObject Prefab { get; private set; }
    }
}