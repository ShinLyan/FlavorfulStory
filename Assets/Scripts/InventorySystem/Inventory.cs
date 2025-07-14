using System;
using System.Linq;
using FlavorfulStory.Core;
using FlavorfulStory.InventorySystem.PickupSystem;
using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Инвентарь игрока с настраиваемым количеством слотов. </summary>
    public class Inventory : MonoBehaviour, IPredicateEvaluator, ISaveable
    {
        /// <summary> Количество слотов в инвентаре. </summary>
        [field: Tooltip("Количество слотов в инвентаре."), SerializeField]
        public int InventorySize { get; private set; }

        /// <summary> Предметы инвентаря. </summary>
        private InventorySlot[] _slots;

        /// <summary> Менеджер уведомлений о подборе предмета. </summary>
        private PickupNotificationManager _notificationManager;

        /// <summary> Событие, вызываемое при изменении инвентаря (добавление, удаление предметов). </summary>
        public event Action InventoryUpdated;

        /// <summary> Событие при сборе предмета. Передает сам предмет. </summary>
        public event Action<InventoryItem> ItemCollected;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="notificationManager"> Менеджер уведомлений о подборе предмета. </param>
        [Inject]
        private void Construct(PickupNotificationManager notificationManager) =>
            _notificationManager = notificationManager;

        /// <summary> Инициализация слотов и ссылки на инвентарь игрока. </summary>
        private void Awake() => _slots = new InventorySlot[InventorySize];

        /// <summary> Есть ли место для предмета в инвентаре? </summary>
        public bool HasSpaceFor(InventoryItem item) => FindSlot(item) >= 0;

        /// <summary> Найти слот, в который можно поместить данный предмет. </summary>
        /// <param name="item"> Предмет, который нужно поместить в слот. </param>
        /// <returns> Возвращает индекс слота предмета. Если предмет не найден, возвращает -1. </returns>
        private int FindSlot(InventoryItem item) =>
            FindStackIndex(item) is var slotIndex && slotIndex >= 0 ? slotIndex : FindEmptySlot();

        /// <summary> Найти индекс существующего стака предметов этого типа. </summary>
        /// <param name="item"> Предмет, для которого нужно найти стак. </param>
        /// <returns> Возвращает индекс стака предмета. Если стак для предмета не найден, возвращает -1. </returns>
        private int FindStackIndex(InventoryItem item)
        {
            if (!item.IsStackable) return -1;
            return Array.FindIndex(
                _slots,
                slot => ReferenceEquals(slot.Item, item) && slot.Number < item.StackSize);
        }

        /// <summary> Найти индекс свободного слота в инвентаре. </summary>
        /// <returns> Возвращает индекс свободного слота в инвентаре.
        /// Если все слоты заполнены, то возвращает -1. </returns>
        private int FindEmptySlot() => Array.FindIndex(_slots, slot => !slot.Item);

        /// <summary> Есть ли экземпляр этого предмета в инвентаре? </summary>
        /// <param name="item"> Предмет. </param>
        /// <returns> Возвращает True - если предмет есть в инвентаре, False - в противном случае. </returns>
        public bool HasItem(InventoryItem item) => _slots.Any(slot => ReferenceEquals(slot.Item, item));

        /// <summary> Получить предмет инвентаря в заданном слоте. </summary>
        /// <param name="slotIndex"> Индекс слота, из которого нужно получить предмет. </param>
        /// <returns> Возвращает предмет инвентаря в заданном слоте. </returns>
        public InventoryItem GetItemInSlot(int slotIndex) => _slots[slotIndex].Item;

        /// <summary> Получить общее количество заданного предмета в инвентаре. </summary>
        /// <param name="item"> Предмет инвентаря. </param>
        /// <returns> Возвращает общее количество заданного предмета в инвентаре. </returns>
        public int GetItemNumber(InventoryItem item) =>
            _slots.Where(slot => slot.Item == item).Sum(slot => slot.Number);

        public void RemoveItem(InventoryItem item, int number)
        {
            if (!HasItem(item))
            {
                Debug.LogError($"No item[{item.ItemName}] present in inventory!");
                return;
            }

            if (GetItemNumber(item) < number)
            {
                Debug.LogError($"You are trying to remove {number} item[{item.ItemName}], " +
                               $"but only {GetItemNumber(item)} present in inventory!");
                return;
            }

            int remainingToRemove = number;
            for (int i = 0; i < _slots.Length && remainingToRemove > 0; i++)
                if (_slots[i].Item == item)
                {
                    int numberToRemove = Math.Min(_slots[i].Number, remainingToRemove);
                    RemoveFromSlot(i, numberToRemove);
                    remainingToRemove -= numberToRemove;
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
            if (_slots[slotIndex].Item) return TryAddToFirstAvailableSlot(item, number);

            while (number > 0)
            {
                int index = FindStackIndex(item);
                if (index < 0) index = FindEmptySlot();
                if (index < 0) return false;

                int addAmount = Math.Min(number, item.StackSize - _slots[slotIndex].Number);
                _slots[slotIndex].Item ??= item;
                _slots[slotIndex].Number += addAmount;
                number -= addAmount;
            }

            InventoryUpdated?.Invoke();
            return true;
        }

        /// <summary> Попробовать добавить предмет в первый доступный слот. </summary>
        /// <param name="item"> Предмет, который нужно добавить. </param>
        /// <param name="number"> Количество предметов, которые нужно добавить. </param>
        /// <returns> Возвращает True, если предмет можно добавить, False - в противном случае. </returns>
        public bool TryAddToFirstAvailableSlot(InventoryItem item, int number)
        {
            int remainingNumber = number;
            while (remainingNumber > 0)
            {
                int index = FindSlot(item);
                if (index < 0) return false;

                int addAmount = Mathf.Min(remainingNumber, item.StackSize - _slots[index].Number);
                _slots[index].Item ??= item;
                _slots[index].Number += addAmount;
                remainingNumber -= addAmount;
            }

            // TODO: ПЕРЕДЕЛАТЬ НА EVENT. Inventory не должен знать о _notificationManager
            _notificationManager.ShowNotification(item.Icon, number, item.ItemName, item.ItemName);

            ItemCollected?.Invoke(item);
            InventoryUpdated?.Invoke();
            return true;
        }

        /// <summary> Извлечь предмет из указанного слота. Удаляет указанное количество или всё содержимое. </summary>
        /// <param name="slotIndex"> Индекс слота в инвентаре. </param>
        /// <param name="number"> Количество предметов. Если больше количества в слоте — удалит всё. </param>
        public void RemoveFromSlot(int slotIndex, int number = int.MaxValue)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length)
            {
                Debug.LogError($"Invalid slot index: {slotIndex}");
                return;
            }

            var slot = _slots[slotIndex];
            if (!slot.Item || slot.Number <= 0)
            {
                Debug.LogError($"Attempted to remove from empty slot {slotIndex}");
                return;
            }

            slot.Number -= number;
            if (slot.Number <= 0) ClearSlot(slotIndex);

            InventoryUpdated?.Invoke();
        }

        /// <summary> Очистить слот инвентаря. </summary>
        /// <param name="slotIndex"> Индекс слота в инвентаре. </param>
        private void ClearSlot(int slotIndex)
        {
            _slots[slotIndex].Item = null;
            _slots[slotIndex].Number = 0;
        }

        #region Saving

        /// <summary> Запись о предмете в слоте. </summary>
        [Serializable]
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
            var slotRecords = new InventorySlotRecord[InventorySize];
            for (int i = 0; i < InventorySize; i++)
            {
                if (!_slots[i].Item) continue;

                slotRecords[i].ItemID = _slots[i].Item.ItemID;
                slotRecords[i].Number = _slots[i].Number;
            }

            return slotRecords;
        }

        /// <summary> Восстановление состояния объекта при загрузке. </summary>
        /// <param name="state"> Объект состояния, который необходимо восстановить. </param>
        public void RestoreState(object state)
        {
            var slotRecords = state as InventorySlotRecord[];
            for (int i = 0; i < InventorySize; i++)
            {
                if (slotRecords == null) continue;

                _slots[i].Item = ItemDatabase.GetItemFromID(slotRecords[i].ItemID);
                _slots[i].Number = slotRecords[i].Number;
            }

            InventoryUpdated?.Invoke();
        }

        #endregion

        #region IPredicateEvaluator

        /// <summary> Оценивает заданный предикат с параметрами. </summary>
        /// <param name="predicate"> Имя предиката для проверки. </param>
        /// <param name="parameters"> Массив параметров для предиката. </param>
        /// <returns> True, если условие выполнено; false, если не выполнено;
        /// null, если предикат не поддерживается. </returns>
        public bool? Evaluate(string predicate, string[] parameters) => predicate switch
        {
            "HasInventoryItem" => HasItem(ItemDatabase.GetItemFromID(parameters[0])),
            _ => null
        };

        #endregion
    }
}