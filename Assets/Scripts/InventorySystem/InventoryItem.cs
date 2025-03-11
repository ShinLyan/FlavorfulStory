using System;
using System.Collections.Generic;
using FlavorfulStory.InventorySystem.PickupSystem;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary>
    ///     ScriptableObject, представляющий предмет,
    ///     который может быть помещен в инвентарь.
    /// </summary>
    // TODO: сделать абстрактным, когда все типы предметов будут реализованы
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Item")]
    public class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary> Заспавнить предмет Pickup на сцене. </summary>
        /// <param name="spawnPosition"> Позиция спавна предмета. </param>
        /// <param name="number"> Количество предметов. </param>
        /// <returns> Возвращает ссылку на заспавненный предмет Pickup. </returns>
        public Pickup SpawnPickup(Vector3 spawnPosition, int number)
        {
            var pickup = Instantiate(_pickup);
            pickup.transform.position = spawnPosition;
            pickup.Setup(this, number);
            return pickup;
        }

        /// <summary> Получить экземпляр предмета инвентаря по его ID. </summary>
        /// <param name="itemID"> ID предмета инвентаря. </param>
        /// <returns> Экземпляр Inventoryitem, соответствующий ID. </returns>
        public static InventoryItem GetItemFromID(string itemID)
        {
            if (_itemDatabase == null) CreateItemDatabase();

            if (itemID == null || !_itemDatabase.ContainsKey(itemID)) return null;
            return _itemDatabase[itemID];
        }

        /// <summary> Создать базу данных предметов. </summary>
        private static void CreateItemDatabase()
        {
            _itemDatabase = new Dictionary<string, InventoryItem>();

            // Загрузка все ресурсов с типом InventoryItem по всему проекту.
            var items = Resources.LoadAll<InventoryItem>(string.Empty);
            foreach (var item in items)
            {
                if (_itemDatabase.TryGetValue(item.ItemID, out var value))
                {
                    Debug.LogError("Присутствует дубликат ID InventoryItem для объектов: " +
                                   $"{value} и {item}. Замените ID у данного объекта.");
                    continue;
                }

                _itemDatabase[item.ItemID] = item;
            }
        }

        #region Fields and Properties

        /// <summary> Автоматически сгенерированный ID для сохранения/загрузки. </summary>
        /// <remarks> Очистите это поле, если вы хотите создать новое. </remarks>
        [field: Tooltip("Автоматически сгенерированный ID для сохранения/загрузки. " +
                        "Очистите это поле, если вы хотите создать новое.")]
        [field: SerializeField]
        public string ItemID { get; private set; }

        /// <summary> Название предмета, которое будет отображаться в UI. </summary>
        [field: Tooltip("Название предмета, которое будет отображаться в UI.")]
        [field: SerializeField]
        public string ItemName { get; private set; }

        /// <summary> Описание предмета, которое будет отображаться в UI. </summary>
        [field: Tooltip("Описание предмета, которое будет отображаться в UI.")]
        [field: SerializeField]
        [field: TextArea]
        public string Description { get; private set; }

        /// <summary> Иконка предмета, которая будет отображаться в UI. </summary>
        [field: Tooltip("Иконка предмета, которая будет отображаться в UI.")]
        [field: SerializeField]
        public Sprite Icon { get; private set; }

        /// <summary> Префаб, который должен появиться при выпадении этого предмета. </summary>
        [Tooltip("Префаб, который должен появиться при выпадении этого предмета.")] [SerializeField]
        private Pickup _pickup;

        /// <summary> Можно ли поместить несколько предметов одного типа в один слот инвентаря? </summary>
        [field: Tooltip("Можно ли поместить несколько предметов одного типа в один слот инвентаря?")]
        [field: SerializeField]
        public bool IsStackable { get; private set; }

        /// <summary> Вместимость одного стака. </summary>
        [field: Tooltip("Вместимость одного стака. ")]
        [field: SerializeField]
        public int StackSize { get; private set; } = 99;

        /// <summary> База данных всех предметов игры. </summary>
        private static Dictionary<string, InventoryItem> _itemDatabase;

        #endregion

        #region ISerializationCallbackReceiver

        /// <summary> Генерация и сохранение нового GUID, если он пустой. </summary>
        public void OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(ItemID)) ItemID = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Требуется для ISerializationCallbackReceiver,
        ///     но нам не нужно ничего с ним делать.
        /// </summary>
        public void OnAfterDeserialize()
        {
        }

        #endregion
    }
}