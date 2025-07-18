﻿using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Отвечает за механику подбора предметов игроком. </summary>
    /// <remarks> Скрипт должен быть размещен на специальном префабе, содержащем данные о предмете. </remarks>
    [RequireComponent(typeof(SphereCollider))]
    public class Pickup : MonoBehaviour
    {
        /// <summary> Радиус подбора предмета. </summary>
        [Tooltip("Радиус подбора предмета."), SerializeField, Range(0f, 5f)]
        private float _pickupRadius = 1f;

        /// <summary> Стак предметов для подбора. </summary>
        [Tooltip("Стак предметов для подбора."), SerializeField]
        private ItemStack _itemStack;

        /// <summary> Предмет, который можно подобрать. </summary>
        public InventoryItem Item => _itemStack.Item;

        /// <summary> Количество предметов, доступных для подбора. </summary>
        public int Number => _itemStack.Number;

        /// <summary> Ссылка на инвентарь игрока. </summary>
        private Inventory _inventory;

        /// <summary> Флаг, указывающий, можно ли подбирать предмет. </summary>
        private bool _canBePickedUp;

        /// <summary> Возвращает true, если предмет можно подобрать и в инвентаре есть место. </summary>
        private bool CanBePickedUp => _canBePickedUp && _inventory.HasSpaceFor(Item);

        /// <summary> Внедрение зависимостей. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(Inventory inventory) => _inventory = inventory;

        /// <summary> Устанавливает предмет, количество и задержку перед возможностью подбора. </summary>
        /// <param name="itemStack"> Предмет и его количество, которые можно подобрать. </param>
        /// <param name="pickupDelay"> Задержка в секундах до возможности подбора. </param>
        public void Setup(ItemStack itemStack, float pickupDelay = 1f)
        {
            _itemStack = itemStack;
            _canBePickedUp = false;
            Invoke(nameof(ActivatePickup), pickupDelay);
        }

        /// <summary> Пытается подобрать предмет и удалить его из мира. </summary>
        public void TryPickup()
        {
            if (!CanBePickedUp) return;

            if (_inventory.TryAddToFirstAvailableSlot(Item, Number)) Destroy(gameObject);
        }

        /// <summary> Делает предмет доступным для подбора. </summary>
        private void ActivatePickup() => _canBePickedUp = true;

#if UNITY_EDITOR

        #region Debug

        /// <summary> Обновляет радиус коллайдера при изменении радиуса подбора в инспекторе. </summary>
        private void OnValidate()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = _pickupRadius;
        }

        #endregion

#endif
    }
}