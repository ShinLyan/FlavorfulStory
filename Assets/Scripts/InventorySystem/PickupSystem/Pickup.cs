using UnityEngine;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> Подбор предмета. </summary>
    /// <remarks> Размещается в специальном префабе. Содержит данные о подобранном предмете. </remarks>
    [RequireComponent(typeof(SphereCollider))]
    public class Pickup : MonoBehaviour
    {
        [SerializeField, Range(0f, 5f), Tooltip("Радиус подбора предмета.")]
        private float _pickupRadius;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _inventory;

        /// <summary> Предмет инвентаря. </summary>
        [field: SerializeField] public InventoryItem Item { get; private set; }

        /// <summary> Количество предметов. </summary>
        public int Number { get; private set; } = 1;

        /// <summary> Может быть подобран?</summary>
        public bool CanBePickedUp => _inventory.HasSpaceFor(Item);

        /// <summary> Инициализация компонента. </summary>
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _inventory = player.GetComponent<Inventory>();
        }

        /// <summary> Установить необходимые данные после создания префаба. </summary>
        /// <param name="item"> Предмет, который нужно установить. </param>
        /// <param name="number"> Количество предметов. </param>
        public void Setup(InventoryItem item, int number)
        {
            Item = item;
            Number = number;
        }

        /// <summary> Подобрать предмет. </summary>
        public void PickupItem()
        {
            bool foundSlot = _inventory.TryAddToFirstEmptySlot(Item, Number);
            if (foundSlot) Destroy(gameObject);
        }

        #region Debug
        private void OnValidate()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = _pickupRadius;
        }
        #endregion
    }
}