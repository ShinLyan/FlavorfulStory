using System;
using System.Linq;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Класс обеспечивает хранение инвентаря игрока. Доступно настраиваемое количество слотов.
    /// Этот компонент должен быть размещен на игровом объекте с тегом "Игрок". </summary>
    public class Inventory : MonoBehaviour, ISaveable
    {
        /// <summary> Количество слотов в инвентаре. </summary>
        [Tooltip("Количество слотов в инвентаре.")] [SerializeField]
        private int _inventorySize = 16;

        /// <summary> Предметы инвентаря. </summary>
        private InventorySlot[] _slots;

        /// <summary> Количество слотов в инвентаре. </summary>
        public int InventorySize => _inventorySize;

        /// <summary> Событие, которое срабатывает, когда предметы в инвентаре добавляются / удаляются. </summary>
        public event System.Action InventoryUpdated;

        /// <summary> Инициализация полей. </summary>
        private void Awake()
        {
            _slots = new InventorySlot[_inventorySize];
            _playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        }

        private static Inventory _playerInventory;

        /// <summary> Получить инвентарь игрока. </summary>
        public static Inventory PlayerInventory
        {
            get
            {
                if (!_playerInventory)
                {
                    return GameObject.FindWithTag("Player").GetComponent<Inventory>();
                }

                return _playerInventory;
            }
            private set => _playerInventory = value;
        }

        /// <summary> Может ли этот предмет поместиться где-нибудь в инвентаре?</summary>
        public bool HasSpaceFor(InventoryItem item) => FindSlot(item) >= 0;

        /// <summary> Найти слот, в который можно поместить данный предмет. </summary>
        /// <param name="item"> Предмет, который нужно поместить в слот. </param>
        /// <returns> Возвращает индекс слота предмета. Если предмет не найден, возвращает -1. </returns>
        private int FindSlot(InventoryItem item) => FindStackIndex(item) is var slotIndex
                                                    && slotIndex < 0
            ? FindEmptySlot()
            : slotIndex;

        /// <summary> Найти индекс существующего стака предметов этого типа. </summary>
        /// <param name="item"> Предмет, для которого нужно найти стак. </param>
        /// <returns> Возвращает индекс стака предмета. Если стак для предмета не найден, возвращает -1. </returns>
        private int FindStackIndex(InventoryItem item)
        {
            if (!item.IsStackable) return -1;

            for (int i = 0; i < _slots.Length; i++)
            {
                if (ReferenceEquals(_slots[i].Item, item)) return i;
            }

            return -1;
        }

        /// <summary> Найти индекс свободного слота в инвентаре. </summary>
        /// <returns> Возвращает индекс свободного слота в инвентаре.
        /// Если все слоты заполнены, то возвращает -1. </returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].Item == null) return i;
            }

            return -1;
        }

        /// <summary> Есть ли экземпляр этого предмета в инвентаре?</summary>
        /// <param name="item"> Предмет. </param>
        /// <returns> Возвращает True - если предмет есть в инвентаре, False - в противном случае. </returns>
        public bool HasItem(InventoryItem item) => _slots.Count((slot) => slot.Item == item) > 0;

        /// <summary> Получить предмет инвентаря в заданном слоте. </summary>
        /// <param name="slotIndex"> Индекс слота, из которого нужно получить предмет. </param>
        /// <returns> Возвращает предмет инвентаря в заданном слоте. </returns>
        public InventoryItem GetItemInSlot(int slotIndex) => _slots[slotIndex].Item;

        /// <summary> Получить общее количество заданного предмета в инвентаре. </summary>
        /// <param name="item"> Предмет инвентаря. </param>
        /// <returns> Возвращает общее количество заданного предмета в инвентаре. </returns>
        public int GetItemNumber(InventoryItem item)
        {
            return _slots.Where(slot => slot.Item == item).Sum(slot => slot.Number);
        }

        //TODO: Добавить коммы
        public void RemoveItem(InventoryItem item, int number)
        {
            if (!HasItem(item))
            {
                Debug.LogError($"No item[{item.ItemName}] present in inventory!");
                return;
            } 
            if (GetItemNumber(item) < number)
            {
                Debug.LogError($"You are trying to remove {number} item[{item.ItemName}], but only {GetItemNumber(item)} present in inventory!");
                return;
            }

            int remainingToRemove = number;
            for (int i = 0; i < _slots.Length && remainingToRemove > 0; i++)
            {
                if (_slots[i].Item == item)
                {
                    int numberToRemove = Math.Min(_slots[i].Number, remainingToRemove);
                    RemoveFromSlot(i, numberToRemove);
                    remainingToRemove -= numberToRemove;
                }
            }
        }
        
        /// <summary> Получить количество предметов инвентаря в заданном слоте. </summary>
        /// <param name="slotIndex"> Индекс слота, из которого нужно получить количество. </param>
        /// <returns> Возвращает количество предметов инвентаря в заданном слоте. </returns>
        public int GetNumberInSlot(int slotIndex) => _slots[slotIndex].Number;

        /// <summary> Попробовать добавить предмет в указанный слот. </summary>
        /// <remarks> Если такой стак уже существует, он будет добавлен в существующий стак.
        /// В противном случае он будет добавлен в первый пустой слот. </remarks>
        /// <param name="slotIndex"> Индекс слота, в который нужно попытаться добавить предмет. </param>
        /// <param name="item"> Предмет, который нужно добавить. </param>
        /// <param name="number"> Количество предметов. </param>
        /// <returns> Возвращает True, если предмет был добавлен в любое место инвентаря. </returns>
        public bool TryAddItemToSlot(int slotIndex, InventoryItem item, int number)
        {
            if (_slots[slotIndex].Item != null) return TryAddToFirstEmptySlot(item, number);

            int index = FindStackIndex(item);
            if (index >= 0) slotIndex = index;

            _slots[slotIndex].Item = item;
            _slots[slotIndex].Number += number;

            InventoryUpdated?.Invoke();
            return true;
        }

        /// <summary> Попробовать добавить предмет в первый доступный слот. </summary>
        /// <param name="item"> Предмет, который нужно добавить. </param>
        /// <param name="number"> Количество предметов, которые нужно добавить. </param>
        /// <returns> Возвращает True, если предмет можно добавить, False - в противном случае. </returns>
        public bool TryAddToFirstEmptySlot(InventoryItem item, int number)
        {
            int slotIndex = FindSlot(item);

            if (slotIndex < 0) return false;

            _slots[slotIndex].Item = item;
            _slots[slotIndex].Number += number;

            InventoryUpdated?.Invoke();
            return true;
        }

        /// <summary> Извлечь предмет из указанного слота. </summary>
        /// <param name="slotIndex"> Индекс слота в инвентаре. </param>
        /// <param name="number"> Количество предметов. </param>
        public void RemoveFromSlot(int slotIndex, int number)
        {
            _slots[slotIndex].Number -= number;
            if (_slots[slotIndex].Number <= 0)
            {
                _slots[slotIndex].Item = null;
                _slots[slotIndex].Number = 0;
            }

            InventoryUpdated?.Invoke();
        }

        #region Saving

        /// <summary> Запись о предмете в слоте. </summary>
        [System.Serializable]
        private struct InventorySlotRecord
        {
            /// <summary> ID предмета в слоте. </summary>
            public string ItemID;

            /// <summary> Количество предметов в слоте. </summary>
            public int Number;
        }

        /// <summary> Фиксация состояния объекта при сохранении. </summary>
        /// <returns> Возвращает объект, в котором фиксируется состояние. </returns>
        public object CaptureState()
        {
            var slotRecords = new InventorySlotRecord[_inventorySize];
            for (int i = 0; i < _inventorySize; i++)
            {
                if (_slots[i].Item != null)
                {
                    slotRecords[i].ItemID = _slots[i].Item.ItemID;
                    slotRecords[i].Number = _slots[i].Number;
                }
            }

            return slotRecords;
        }

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public void RestoreState(object state)
        {
            var slotRecords = state as InventorySlotRecord[];
            for (int i = 0; i < _inventorySize; i++)
            {
                _slots[i].Item = InventoryItem.GetItemFromID(slotRecords[i].ItemID);
                _slots[i].Number = slotRecords[i].Number;
            }

            InventoryUpdated?.Invoke();
        }

        #endregion
    }
}