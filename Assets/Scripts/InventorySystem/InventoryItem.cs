using System;
using FlavorfulStory.InventorySystem.PickupSystem;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> ScriptableObject, представляющий предмет, который может быть помещен в инвентарь. </summary>
    // TODO: сделать абстрактным, когда все типы предметов будут реализованы
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Item")]
    public class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Fields and Properties

        /// <summary> Автоматически сгенерированный ID для сохранения/загрузки. </summary>
        /// <remarks> Очистите это поле, если вы хотите создать новое. </remarks>
        [field: Tooltip("Автоматически сгенерированный ID для сохранения/загрузки. " +
                        "Очистите это поле, если вы хотите создать новое."), SerializeField]
        public string ItemID { get; private set; }

        /// <summary> Название предмета, которое будет отображаться в UI. </summary>
        [field: Tooltip("Название предмета, которое будет отображаться в UI."), SerializeField]
        public string ItemName { get; private set; }

        /// <summary> Описание предмета, которое будет отображаться в UI. </summary>
        [field: Tooltip("Описание предмета, которое будет отображаться в UI."), SerializeField, TextArea]
        public string Description { get; private set; }

        /// <summary> Иконка предмета, которая будет отображаться в UI. </summary>
        [field: Tooltip("Иконка предмета, которая будет отображаться в UI."), SerializeField]
        public Sprite Icon { get; private set; }

        /// <summary> Префаб, который должен появиться при выпадении этого предмета. </summary>
        [Tooltip("Префаб, который должен появиться при выпадении этого предмета."), SerializeField]
        private Pickup _pickup;

        /// <summary> Можно ли поместить несколько предметов одного типа в один слот инвентаря? </summary>
        [field: Tooltip("Можно ли поместить несколько предметов одного типа в один слот инвентаря?"), SerializeField]
        public bool IsStackable { get; private set; }

        /// <summary> Вместимость одного стака. </summary>
        [field: Tooltip("Вместимость одного стака. "), SerializeField]
        public int StackSize { get; private set; } = 99;

        #endregion

        /// <summary> Заспавнить предмет Pickup на сцене. </summary>
        /// <param name="spawnPosition"> Позиция спавна предмета. </param>
        /// <param name="number"> Количество предметов. </param>
        /// <returns> Возвращает ссылку на заспавненный предмет Pickup. </returns>
        //TODO: Вынести куда-то. Item не должен сам себя спавнить!
        public Pickup SpawnPickup(Vector3 spawnPosition, int number)
        {
            var pickup = Instantiate(_pickup);
            pickup.transform.position = spawnPosition;
            pickup.Setup(this, number);
            return pickup;
        }

        #region ISerializationCallbackReceiver

        /// <summary> Генерация и сохранение нового GUID, если он пустой. </summary>
        public void OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(ItemID)) ItemID = Guid.NewGuid().ToString();
        }

        /// <summary> Требуется для ISerializationCallbackReceiver, но нам не нужно ничего с ним делать. </summary>
        public void OnAfterDeserialize() { }

        #endregion
    }
}