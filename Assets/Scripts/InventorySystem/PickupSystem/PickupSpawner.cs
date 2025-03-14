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
        private InventoryItem _item;

        /// <summary> Количество предметов, которые будут заспавнены. </summary>
        [Tooltip("Количество предметов, которые будут заспавнены."), SerializeField, Range(1, 100)]
        private int _number;

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
            var spawnedPickup = _item.SpawnPickup(transform.position, _number);
            spawnedPickup.transform.SetParent(transform);
            spawnedPickup.transform.localRotation = Quaternion.Euler(0, 0, 0);

            _isPickedUp = true;
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