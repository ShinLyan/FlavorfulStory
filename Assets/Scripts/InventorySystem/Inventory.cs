using System;
using System.Linq;
using FlavorfulStory.Core;
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
        private ItemStack[] _inventorySlots;

        /// <summary> Событие, вызываемое при изменении инвентаря (добавление, удаление предметов). </summary>
        public event Action InventoryUpdated;

        /// <summary> Сигнальная шина Zenject для отправки и получения событий. </summary>
        private SignalBus _signalBus;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="signalBus"> Сигнальная шина Zenject для отправки событий. </param>
        [Inject]
        private void Construct(SignalBus signalBus) => _signalBus = signalBus;

        /// <summary> Инициализация слотов и ссылки на инвентарь игрока. </summary>
        private void Awake() => _inventorySlots = new ItemStack[InventorySize];

        /// <summary> При старте вызываем событие обновление инвентаря. </summary>
        /// <remarks> После восстановления состояния нужно разослать событие. </remarks>
        private void Start() => InventoryUpdated?.Invoke();

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
        private int FindStackIndex(InventoryItem item) => item.IsStackable
            ? Array.FindIndex(_inventorySlots,
                itemStack => ReferenceEquals(itemStack.Item, item) && itemStack.Number < item.StackSize)
            : -1;

        /// <summary> Найти индекс свободного слота в инвентаре. </summary>
        /// <returns> Возвращает индекс свободного слота в инвентаре.
        /// Если все слоты заполнены, то возвращает -1. </returns>
        private int FindEmptySlot() => Array.FindIndex(_inventorySlots, itemStack => !itemStack.Item);

        /// <summary> Есть ли экземпляр этого предмета в инвентаре? </summary>
        /// <param name="item"> Предмет. </param>
        /// <returns> Возвращает True - если предмет есть в инвентаре, False - в противном случае. </returns>
        public bool HasItem(InventoryItem item) =>
            _inventorySlots.Any(itemStack => ReferenceEquals(itemStack.Item, item));

        /// <summary> Получить предмет инвентаря и его количество в заданном слоте. </summary>
        /// <param name="slotIndex"> Индекс слота, из которого нужно получить предмет. </param>
        /// <returns> Возвращает предмет инвентаря и его количество в заданном слоте. </returns>
        public ItemStack GetItemStackInSlot(int slotIndex) => _inventorySlots[slotIndex];

        /// <summary> Получить предмет инвентаря в заданном слоте. </summary>
        /// <param name="slotIndex"> Индекс слота, из которого нужно получить предмет. </param>
        /// <returns> Возвращает предмет инвентаря в заданном слоте. </returns>
        public InventoryItem GetItemInSlot(int slotIndex) => _inventorySlots[slotIndex].Item;

        /// <summary> Получить общее количество заданного предмета в инвентаре. </summary>
        /// <param name="item"> Предмет инвентаря. </param>
        /// <returns> Возвращает общее количество заданного предмета в инвентаре. </returns>
        public int GetItemNumber(InventoryItem item) =>
            _inventorySlots.Where(itemStack => itemStack.Item == item).Sum(itemStack => itemStack.Number);

        /// <summary> Удаляет указанное количество предметов из инвентаря. </summary>
        /// <param name="item"> Предмет, который нужно удалить. </param>
        /// <param name="number"> Количество предметов для удаления. </param>
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
            for (int i = 0; i < _inventorySlots.Length && remainingToRemove > 0; i++)
                if (_inventorySlots[i].Item == item)
                {
                    int numberToRemove = Math.Min(_inventorySlots[i].Number, remainingToRemove);
                    RemoveFromSlot(i, numberToRemove);
                    remainingToRemove -= numberToRemove;
                }
        }

        /// <summary> Получить количество предметов инвентаря в заданном слоте. </summary>
        /// <param name="slotIndex"> Индекс слота, из которого нужно получить количество. </param>
        /// <returns> Возвращает количество предметов инвентаря в заданном слоте. </returns>
        public int GetNumberInSlot(int slotIndex) => _inventorySlots[slotIndex].Number;

        /// <summary> Попробовать добавить предмет в указанный слот. </summary>
        /// <remarks> Если такой стак уже существует, он будет добавлен в существующий стак.
        /// В противном случае он будет добавлен в первый пустой слот. </remarks>
        /// <param name="slotIndex"> Индекс слота, в который нужно попытаться добавить предмет. </param>
        /// <param name="item"> Предмет, который нужно добавить. </param>
        /// <param name="number"> Количество предметов. </param>
        /// <returns> Возвращает True, если предмет был добавлен в любое место инвентаря. </returns>
        public bool TryAddItemToSlot(int slotIndex, InventoryItem item, int number)
        {
            if (_inventorySlots[slotIndex].Item) return TryAddToFirstAvailableSlot(item, number);

            while (number > 0)
            {
                int index = FindStackIndex(item);
                if (index < 0) index = FindEmptySlot();
                if (index < 0) return false;

                int addAmount = Math.Min(number, item.StackSize - _inventorySlots[slotIndex].Number);
                _inventorySlots[slotIndex].Item ??= item;
                _inventorySlots[slotIndex].Number += addAmount;
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

                int addAmount = Mathf.Min(remainingNumber, item.StackSize - _inventorySlots[index].Number);
                _inventorySlots[index].Item ??= item;
                _inventorySlots[index].Number += addAmount;
                remainingNumber -= addAmount;
            }

            _signalBus.Fire(new ItemCollectedSignal(new ItemStack(item, number)));
            InventoryUpdated?.Invoke();
            return true;
        }

        /// <summary> Извлечь предмет из указанного слота. Удаляет указанное количество или всё содержимое. </summary>
        /// <param name="slotIndex"> Индекс слота в инвентаре. </param>
        /// <param name="number"> Количество предметов. Если больше количества в слоте — удалит всё. </param>
        public void RemoveFromSlot(int slotIndex, int number = int.MaxValue)
        {
            if (slotIndex < 0 || slotIndex >= _inventorySlots.Length)
            {
                Debug.LogError($"Invalid slot index: {slotIndex}");
                return;
            }

            var slot = _inventorySlots[slotIndex];
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
            _inventorySlots[slotIndex].Item = null;
            _inventorySlots[slotIndex].Number = 0;
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
                if (!_inventorySlots[i].Item) continue;

                slotRecords[i].ItemID = _inventorySlots[i].Item.ItemID;
                slotRecords[i].Number = _inventorySlots[i].Number;
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

                _inventorySlots[i].Item = ItemDatabase.GetItemFromID(slotRecords[i].ItemID);
                _inventorySlots[i].Number = slotRecords[i].Number;
            }
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