using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Сервис для спавна Pickup объектов на сцене. </summary>
    public static class PickupFactory
    {
        /// <summary> Заспавнить предмет Pickup на сцене. </summary>
        /// <param name="item"> Предмет, на основе которого будет создан Pickup. </param>
        /// <param name="position"> Позиция спавна. </param>
        /// <param name="quantity"> Количество предметов. </param>
        /// <param name="parent"> Родитель, контейнер на сцене. </param>
        public static Pickup Spawn(InventoryItem item, Vector3 position, int quantity, Transform parent = null)
        {
            if (!item.PickupPrefab)
            {
                Debug.LogError($"PickupPrefab не назначен для предмета {item.name}");
                return null;
            }

            var pickup = Object.Instantiate(item.PickupPrefab, parent);
            pickup.transform.position = position;
            pickup.Setup(item, quantity);
            return pickup;
        }
    }
}