using FlavorfulStory.Actions;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Отвечает за создание объектов Pickup, которые появляются при первой загрузке уровня. </summary>
    /// <remarks> Автоматически создает префаб для заданного предмета инвентаря. </remarks>
    public class InstrumentSpawner : MonoBehaviour, ISaveable
    {
        /// <summary> Предмет, который будет заспавнен в сцене. </summary>
        [Tooltip("Предмет, который будет заспавнен в сцене."), SerializeField]
        private Tool _tool;

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
            var spawnedPickup = PickupFactory.Spawn(_tool, transform.position, 1, transform);
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