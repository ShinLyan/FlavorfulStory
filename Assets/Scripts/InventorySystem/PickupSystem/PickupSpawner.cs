using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Создает объекты Pickup, которые должны быть при первой загрузке на уровне.
    /// При этом автоматически создается правильный префаб для данного предмета инвентаря.</summary>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        /// <summary> Предмет инвентаря.</summary>
        [SerializeField] private InventoryItem _item;
        [SerializeField] private int _number = 1;

        /// <summary> Собран ли предмет?</summary>
        private bool IsItemCollected => GetPickup() == null;

        /// <summary> При старте спавним предмет.</summary>
        private void Awake()
        {
            // Вызывается в Awake, чтобы гарантировать его создание до того,
            // как он может быть уничтожен системой сохранения при загрузке.
            SpawnPickup();
        }

        /// <summary> Получить предмет на сцене, если он существует.</summary>
        /// <returns> Возвращает предмет на сцене, если он существует.</returns>
        private Pickup GetPickup() => GetComponentInChildren<Pickup>();

        /// <summary> Заспавнить предмет на сцену.</summary>
        private void SpawnPickup()
        {
            var spawnedPickup = _item.SpawnPickup(transform.position, _number);
            spawnedPickup.transform.SetParent(transform);
        }

        /// <summary> Удалить предмет со сцены.</summary>
        private void DestroyPickup()
        {
            if (GetPickup()) Destroy(GetPickup().gameObject);
        }

        #region Saving
        /// <summary> Фиксация состояния объекта при сохранении.</summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние.</returns>
        public object CaptureState() => IsItemCollected;

        /// <summary> Восстановление состояния объекта при загрузке.</summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить.</param>
        public void RestoreState(object state)
        {
            bool shouldBeCollected = (bool)state;

            if (shouldBeCollected && !IsItemCollected) DestroyPickup();

            if (!shouldBeCollected && IsItemCollected) SpawnPickup();
        }
        #endregion
    }
}