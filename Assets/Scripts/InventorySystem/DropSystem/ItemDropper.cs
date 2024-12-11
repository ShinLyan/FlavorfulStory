using FlavorfulStory.InventorySystem.PickupSystem;
using FlavorfulStory.Saving;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.DropSystem
{
    /// <summary> Выбрасыватель предметов.</summary>
    /// <remarks> Может быть размещен на объекте сцены, которому необходимо выбрасывать pickup в мир.
    /// Также отслеживается системой сохранения.</remarks>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        /// <summary> Выброшенные предметы.</summary>
        private List<Pickup> _droppedItems = new();

        /// <summary> Создание pickup в определенной позиции.</summary>
        /// <param name="item"> Предмет, который необходимо заспавнить.</param>
        /// <param name="number"> Количество предметов.</param>
        public void DropItem(InventoryItem item , int number)
        {
            SpawnPickup(item, number, GetDropPosition());
        }

        /// <summary> Получить позицию для спавна предмета.</summary>
        /// <remarks> Можете переопределить для задания кастомной позиции спавна предмета.</remarks>
        /// <returns> Возвращает позицию, где должен быть заспавнен предмет.</returns>
        protected virtual Vector3 GetDropPosition()
        {
            // По умолчанию спавнится в самом объекте.
            return transform.position;
        }

        /// <summary> Заспавнить предмет Pickup на сцене.</summary>
        /// <param name="item"> Предмет, который необходимо заспавнить.</param>
        /// <param name="number"> Количество предметов.</param>
        /// <param name="spawnPosition"> Позиция спавна предмета.</param>
        public void SpawnPickup(InventoryItem item, int number, Vector3 spawnPosition)
        {
            var pickup = item.SpawnPickup(spawnPosition, number);
            _droppedItems.Add(pickup);
        }

        #region Saving
        /// <summary> Запись о выпавших предметах.</summary>
        [System.Serializable]
        private struct DropRecord
        {
            /// <summary> ID выпавшего предмета.</summary>
            public string ItemID;

            /// <summary> Позиция выпадшего предмета.</summary>
            public SerializableVector3 Position;

            /// <summary> Количество выпавших предметов.</summary>
            public int Number;
        }

        /// <summary> Фиксация состояния объекта при сохранении.</summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние.</returns>
        public object CaptureState()
        {
            RemovePickedUpItems();
            var droppedItemsList = new DropRecord[_droppedItems.Count];
            for (int i = 0; i < droppedItemsList.Length; i++)
            {
                droppedItemsList[i].ItemID = _droppedItems[i].Item.ItemID;
                droppedItemsList[i].Position = new SerializableVector3(_droppedItems[i].transform.position);
                droppedItemsList[i].Number = _droppedItems[i].Number;
            }
            return droppedItemsList;
        }

        /// <summary> Удалить из списка все предметы, которые были подобраны в мире.</summary>
        private void RemovePickedUpItems()
        {
            var newList = new List<Pickup>();
            foreach (var item in _droppedItems)
            {
                if (item != null) newList.Add(item);
            }
            _droppedItems = newList;
        }

        /// <summary> Восстановление состояния объекта при загрузке.</summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить.</param>
        public void RestoreState(object state)
        {
            var droppedItemsList = state as DropRecord[];
            foreach (var item in droppedItemsList)
            {
                var pickupItem = InventoryItem.GetItemFromID(item.ItemID);
                Vector3 position = item.Position.ToVector();
                int number = item.Number;
                SpawnPickup(pickupItem, number, position);
            }
        }
        #endregion
    }
}