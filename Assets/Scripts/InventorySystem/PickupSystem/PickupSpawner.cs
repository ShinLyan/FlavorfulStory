using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Отвечает за создание объектов Pickup, которые появляются при первой загрузке уровня. </summary>
    /// <remarks> Автоматически создает префаб для заданного предмета инвентаря. </remarks>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        /// <summary> Предмет, который будет заспавнен в сцене. </summary>
        [Tooltip("Предмет, который будет заспавнен в сцене."), SerializeField]
        private InventorySlot _inventorySlot;

        /// <summary> Был ли предмет собран? </summary>
        private bool _isPickedUp;

        /// <summary> Создает предмет при загрузке сцены. </summary>
        private void Start()
        {
            if (!_isPickedUp) SpawnPickup();
        }

        /// <summary> Создает объект Pickup на сцене. </summary>
        private void SpawnPickup()
        {
            var spawnedPickup = Spawn(_inventorySlot.Item, transform.position, _inventorySlot.Number, transform);
            spawnedPickup.transform.localRotation = Quaternion.Euler(0, 0, 0);

            _isPickedUp = true;
        }

        /// <summary> Заспавнить предмет Pickup на сцене. </summary>
        /// <param name="item"> Предмет, на основе которого будет создан Pickup. </param>
        /// <param name="position"> Позиция спавна. </param>
        /// <param name="number"> Количество предметов. </param>
        /// <param name="parent"> Родитель, контейнер на сцене. </param>
        public static Pickup Spawn(InventoryItem item, Vector3 position, int number, Transform parent = null)
        {
            if (!item.PickupPrefab)
            {
                Debug.LogError($"PickupPrefab не назначен для предмета {item.name}");
                return null;
            }

            var pickup = Instantiate(item.PickupPrefab, parent);
            pickup.transform.position = position;
            pickup.Setup(item, number);
            return pickup;
        }

        #region Saving

        /// <summary> Сохраняет состояние объекта (собран предмет или нет). </summary>
        /// <returns> true, если предмет был собран, иначе false. </returns>
        public object CaptureState() => _isPickedUp;

        /// <summary> Восстанавливает состояние объекта при загрузке. </summary>
        /// <param name="state"> Сохраненное состояние объекта. </param>
        public void RestoreState(object state) => _isPickedUp = (bool)state;

        #endregion
    }
}