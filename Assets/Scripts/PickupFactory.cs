using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.PickupSystem;
using UnityEngine;

namespace FlavorfulStory
{
    /// <summary> Сервис для спавна Pickup объектов на сцене. </summary>
    public static class PickupFactory
    {
        /// <summary> Заспавнить предмет Pickup на сцене. </summary>
        /// <param name="item"> Предмет, на основе которого будет создан Pickup. </param>
        /// <param name="position"> Позиция спавна. </param>
        /// <param name="quantity"> Количество предметов. </param>
        public static Pickup Spawn(InventoryItem item, Vector3 position, int quantity)
        {
            if (!item.PickupPrefab)
            {
                Debug.LogError($"PickupPrefab не назначен для предмета {item.name}");
                return null;
            }

            var pickup = Object.Instantiate(item.PickupPrefab);
            pickup.transform.position = position;
            pickup.Setup(item, quantity);
            return pickup;
        }
    }
}