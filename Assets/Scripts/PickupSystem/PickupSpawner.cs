using FlavorfulStory.Utils.Factories;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.PickupSystem
{
    /// <summary> Отвечает за создание объектов Pickup, которые появляются при первой загрузке уровня. </summary>
    /// <remarks> Автоматически создает префаб для заданного предмета инвентаря. </remarks>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        /// <summary> Предмет, который будет заспавнен в сцене. </summary>
        [Tooltip("Предмет, который будет заспавнен в сцене."), SerializeField]
        private ItemStack _itemStack;

        /// <summary> Был ли предмет собран? </summary>
        private bool _isPickedUp;

        /// <summary> Фабрика для создания экземпляров предметов, доступных для подбора. </summary>
        private IPrefabFactory<Pickup> _pickupFactory;

        /// <summary> Фабрика для создания экземпляров предметов, доступных для подбора. </summary>
        /// <param name="pickupFactory"> Фабрика для создания экземпляров предметов, доступных для подбора. </param>
        [Inject]
        private void Construct(IPrefabFactory<Pickup> pickupFactory) => _pickupFactory = pickupFactory;

        /// <summary> Создает предмет при загрузке сцены. </summary>
        private void Start()
        {
            if (!_isPickedUp) SpawnPickup();
        }

        /// <summary> Создает объект Pickup на сцене. </summary>
        private void SpawnPickup()
        {
            var instance = _pickupFactory.Create(_itemStack.Item.PickupPrefab, transform.position,
                Quaternion.identity, transform);
            instance.Setup(_itemStack, 0f);
            _isPickedUp = true;
        }

        #region ISaveable

        /// <summary> Сохраняет состояние объекта (собран предмет или нет). </summary>
        /// <returns> <c>true</c>, если предмет был собран, иначе <c>false</c>. </returns>
        public object CaptureState() => _isPickedUp;

        /// <summary> Восстанавливает состояние объекта при загрузке. </summary>
        /// <param name="state"> Сохраненное состояние объекта. </param>
        public void RestoreState(object state) => _isPickedUp = (bool)state;

        #endregion
    }
}