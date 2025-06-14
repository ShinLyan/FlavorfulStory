using UnityEngine;
using Zenject;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Отвечает за механику подбора предметов игроком. </summary>
    /// <remarks> Скрипт должен быть размещен на специальном префабе, содержащем данные о предмете. </remarks>
    [RequireComponent(typeof(SphereCollider))]
    public class Pickup : MonoBehaviour
    {
        /// <summary> Радиус подбора предмета. </summary>
        [SerializeField, Range(0f, 5f), Tooltip("Радиус подбора предмета.")]
        private float _pickupRadius;

        /// <summary> Предмет, который можно подобрать. </summary>
        [field: SerializeField]
        public InventoryItem Item { get; private set; }

        /// <summary> Количество предметов, доступных для подбора. </summary>
        public int Number { get; private set; } = 1;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _inventory;

        /// <summary> Указывает, может ли предмет быть подобран в текущий момент. </summary>
        public bool CanBePickedUp => _inventory.HasSpaceFor(Item);

        /// <summary> Внедрение зависимостей. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(Inventory inventory) => _inventory = inventory;

        /// <summary> Устанавливает данные для подбираемого предмета. </summary>
        /// <param name="item"> Тип предмета. </param>
        /// <param name="number"> Количество предметов. </param>
        public void Setup(InventoryItem item, int number)
        {
            Item = item;
            Number = number;
        }

        /// <summary> Добавить предмет в инвентарь и удалить его из мира. </summary>
        public void PickupItem()
        {
            bool foundSlot = _inventory.TryAddToFirstAvailableSlot(Item, Number);
            if (foundSlot) Destroy(gameObject);
        }

        #region Debug

        /// <summary> Обновляет радиус коллайдера при изменении радиуса подбора в инспекторе. </summary>
        private void OnValidate()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = _pickupRadius;
        }

        #endregion
    }
}