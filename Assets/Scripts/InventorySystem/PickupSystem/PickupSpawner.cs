using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Отвечает за создание объектов Pickup, которые появляются при первой загрузке уровня. </summary>
    /// <remarks> Автоматически создает префаб для заданного предмета инвентаря. </remarks>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        /// <summary> Предмет, который будет создан в сцене. </summary>
        [SerializeField] private InventoryItem _item;

        /// <summary> Количество предметов, которые будут созданы. </summary>
        [SerializeField] private int _number = 1;

        /// <summary> Был ли предмет собран? </summary>
        private bool IsItemCollected => GetPickup() == null;

        /// <summary> Возвращает объект Pickup, если он существует на сцене. </summary>
        /// <returns> Экземпляр объекта Pickup или null, если его нет. </returns>
        private Pickup GetPickup() => GetComponentInChildren<Pickup>();
        
        /// <summary> Создает предмет при загрузке сцены. </summary>
        private void Awake()
        {
            SpawnPickup();
        }
        
        /// <summary> Создает объект Pickup на сцене. </summary>
        private void SpawnPickup()
        {
            var spawnedPickup = _item.SpawnPickup(transform.position, _number);
            spawnedPickup.transform.SetParent(transform);
        }

        /// <summary> Удаляет объект Pickup со сцены. </summary>
        private void DestroyPickup()
        {
            if (GetPickup()) Destroy(GetPickup().gameObject);
        }

        #region Saving

        /// <summary> Сохраняет состояние объекта (собран предмет или нет). </summary>
        /// <returns> true, если предмет был собран, иначе false. </returns>
        public object CaptureState() => IsItemCollected;

        /// <summary> Восстанавливает состояние объекта при загрузке. </summary>
        /// <param name="state"> Сохраненное состояние объекта. </param>
        public void RestoreState(object state)
        {
            bool shouldBeCollected = (bool)state;

            if (shouldBeCollected && !IsItemCollected) DestroyPickup();
            if (!shouldBeCollected && IsItemCollected) SpawnPickup();
        }
    
        #endregion
    }
}