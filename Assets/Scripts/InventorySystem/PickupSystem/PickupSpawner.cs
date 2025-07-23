using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Отвечает за создание объектов Pickup, которые появляются при первой загрузке уровня. </summary>
    /// <remarks> Автоматически создает префаб для заданного предмета инвентаря. </remarks>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        /// <summary> Предмет, который будет заспавнен в сцене. </summary>
        [Tooltip("Предмет, который будет заспавнен в сцене."), SerializeField]
        private ItemStack _inventorySlot;

        /// <summary> Был ли предмет собран? </summary>
        private bool _isPickedUp;

        /// <summary> Фабрика для создания экземпляров предметов, доступных для подбора. </summary>
        private PickupFactory _pickupFactory;

        /// <summary> Фабрика для создания экземпляров предметов, доступных для подбора. </summary>
        /// <param name="pickupFactory"> Фабрика для создания экземпляров предметов, доступных для подбора. </param>
        [Inject]
        private void Construct(PickupFactory pickupFactory) => _pickupFactory = pickupFactory;

        /// <summary> Создает предмет при загрузке сцены. </summary>
        private void Start()
        {
            if (!_isPickedUp) SpawnPickup();
        }

        /// <summary> Создает объект Pickup на сцене. </summary>
        private void SpawnPickup()
        {
            _pickupFactory.Create(_inventorySlot, transform.position, 0f, transform);
            _isPickedUp = true;
        }

        #region Saving

        /// <summary> Сохраняет состояние объекта (собран предмет или нет). </summary>
        /// <returns> true, если предмет был собран, иначе false. </returns>
        public object CaptureState() => _isPickedUp;

        /// <summary> Восстанавливает состояние объекта при загрузке. </summary>
        /// <param name="state"> Сохраненное состояние объекта. </param>
        public void RestoreState(object state) => _isPickedUp = state is true;

        #endregion
    }
}