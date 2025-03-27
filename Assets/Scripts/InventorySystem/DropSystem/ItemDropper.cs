using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InventorySystem.PickupSystem;
using FlavorfulStory.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Выбрасыватель предметов. </summary>
    /// <remarks> Может быть размещен на объекте сцены, которому необходимо выбрасывать pickup в мир.
    /// Также отслеживается системой сохранения. </remarks>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        /// <summary> Выброшенные предметы. </summary>
        private List<Pickup> _droppedItems = new();

        /// <summary> Список прочих заспавненных предметов инвентаря. </summary>
        private readonly List<DropRecord> _otherSceneDroppedItems = new();

        /// <summary> Создание pickup в определенной позиции. </summary>
        /// <param name="item"> Предмет, который необходимо заспавнить. </param>
        /// <param name="number"> Количество предметов. </param>
        public void DropItem(InventoryItem item, int number) => SpawnPickup(item, number, GetDropPosition());

        /// <summary> Получить позицию для спавна предмета. </summary>
        /// <remarks> Можете переопределить для задания кастомной позиции спавна предмета. </remarks>
        /// <returns> Возвращает позицию, где должен быть заспавнен предмет. </returns>
        private Vector3 GetDropPosition()
        {
            const float DropOffsetRange = 2f; // Диапазон случайного смещения по осям X и Z
            float randomOffsetX = Random.Range(-DropOffsetRange, DropOffsetRange);
            float randomOffsetZ = Random.Range(-DropOffsetRange, DropOffsetRange);
            return transform.position + new Vector3(randomOffsetX, 1, randomOffsetZ);
        }

        /// <summary> Заспавнить предмет Pickup на сцене. </summary>
        /// <param name="item"> Предмет, который необходимо заспавнить. </param>
        /// <param name="number"> Количество предметов. </param>
        /// <param name="spawnPosition"> Позиция спавна предмета. </param>
        private void SpawnPickup(InventoryItem item, int number, Vector3 spawnPosition)
        {
            var pickup = PickupSpawner.Spawn(item, spawnPosition, number);
            _droppedItems.Add(pickup);
        }

        #region Saving

        /// <summary> Запись о выпавших предметах. </summary>
        [Serializable]
        private struct DropRecord
        {
            /// <summary> ID выпавшего предмета. </summary>
            public string ItemID;

            /// <summary> Позиция выпадшего предмета. </summary>
            public SerializableVector3 Position;

            /// <summary> Количество выпавших предметов. </summary>
            public int Quantity;

            /// <summary> Индекс сцены. </summary>
            public int SceneIndex;
        }

        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public object CaptureState()
        {
            RemovePickedUpItems();

            var droppedItemsList = _droppedItems.Select(pickup => new DropRecord
            {
                ItemID = pickup.Item.ItemID,
                Position = new SerializableVector3(pickup.transform.position),
                Quantity = pickup.Number,
                SceneIndex = SceneManager.GetActiveScene().buildIndex
            }).ToList();

            droppedItemsList.AddRange(_otherSceneDroppedItems);
            return droppedItemsList;
        }

        /// <summary> Удалить из списка все предметы, которые были подобраны в мире. </summary>
        private void RemovePickedUpItems() => _droppedItems = _droppedItems.Where(item => item).ToList();

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public void RestoreState(object state)
        {
            var droppedItemsList = state as List<DropRecord>;
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            _otherSceneDroppedItems.Clear();
            foreach (var item in droppedItemsList)
            {
                if (item.SceneIndex != buildIndex)
                {
                    _otherSceneDroppedItems.Add(item);
                    continue;
                }

                var pickupItem = ItemDatabase.GetItemFromID(item.ItemID);
                int number = item.Quantity;
                var position = item.Position.ToVector();
                SpawnPickup(pickupItem, number, position);
            }
        }

        #endregion
    }
}