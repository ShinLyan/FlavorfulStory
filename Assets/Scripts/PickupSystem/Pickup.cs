using UnityEngine;
using Zenject;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;

namespace FlavorfulStory.PickupSystem
{
    /// <summary> Отвечает за механику подбора предметов игроком. </summary>
    /// <remarks> Скрипт должен быть размещен на специальном префабе, содержащем данные о предмете. </remarks>
    [RequireComponent(typeof(SphereCollider))]
    public class Pickup : MonoBehaviour
    {
        /// <summary> Радиус подбора предмета. </summary>
        [Tooltip("Радиус подбора предмета."), SerializeField, Range(0f, 5f)]
        private float _pickupRadius;

        /// <summary> Стак предметов для подбора. </summary>
        [Tooltip("Стак предметов для подбора."), SerializeField]
        private ItemStack _itemStack;

        /// <summary> Предмет, который можно подобрать. </summary>
        public InventoryItem Item => _itemStack.Item;

        /// <summary> Количество предметов, доступных для подбора. </summary>
        public int Number => _itemStack.Number;

        /// <summary> Провайдер инвентарей. </summary>
        private IInventoryProvider _inventoryProvider;

        /// <summary> Провайдер позиции игрока. </summary>
        private IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Настройки пикапов. </summary>
        private PickupSettings _pickupSettings;
        
        /// <summary> Магнит. </summary>
        private PickupMagnet _magnet;
        
        /// <summary> Флаг, указывающий, можно ли подбирать предмет. </summary>
        private bool _canBePickedUp;

        /// <summary> Возвращает true, если предмет можно подобрать и в инвентаре есть место. </summary>
        public bool CanBePickedUp => _canBePickedUp && _inventoryProvider.GetPlayerInventory().HasSpaceFor(Item);

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="inventoryProvider"> Провайдер инвентарей. </param>
        /// <param name="playerPositionProvider"> Провайдер позиции игрока. </param>
        /// <param name="pickupSettings"> Настройки пикапа. </param>
        [Inject]
        private void Construct(
            IInventoryProvider inventoryProvider, 
            IPlayerPositionProvider playerPositionProvider, 
            PickupSettings pickupSettings)
        {
            _inventoryProvider = inventoryProvider;
            _pickupSettings = pickupSettings;
            _magnet = new PickupMagnet(
                this,
                transform,
                GetComponent<Rigidbody>(),
                GetComponentsInChildren<Collider>(true),
                playerPositionProvider,
                inventoryProvider,
                _pickupSettings);
        }

        /// <summary> Устанавливает предмет, количество и задержку перед возможностью подбора. </summary>
        /// <param name="itemStack"> Предмет и его количество, которые можно подобрать. </param>
        public void Setup(ItemStack itemStack)
        {
            _itemStack = itemStack;
            _canBePickedUp = false;
            Invoke(nameof(ActivatePickup), _pickupSettings.PickupActivationDelay);
        }

        /// <summary> Пытается подобрать предмет и удалить его из мира. </summary>
        public void TryPickup()
        {
            if (!CanBePickedUp) return;

            if (_inventoryProvider.GetPlayerInventory().TryAddToFirstAvailableSlot(Item, Number)) Destroy(gameObject);
        }

        /// <summary> Делает предмет доступным для подбора. </summary>
        private void ActivatePickup()
        {
            _canBePickedUp = true;
            _magnet?.ScheduleMagnet();
        } 

        /// <summary> Отписка магнита от инвентаря игрока при уничтожении. </summary>
        private void OnDestroy() => _magnet?.Dispose();

#if UNITY_EDITOR

        #region Debug

        /// <summary> Проводит настройку параметров в инспекторе. </summary>
        private void OnValidate()
        {
            UpdatePickupRadius();
            IgnorePlayerOnChildColliders();
        }

        /// <summary> Обновляет радиус триггер-коллайдера. </summary>
        private void UpdatePickupRadius()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider) sphereCollider.radius = _pickupRadius;
        }

        /// <summary> Настраивает игнорирование дочерних коллизий слоя игрока. </summary>
        private void IgnorePlayerOnChildColliders()
        {
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            foreach (var col in GetComponentsInChildren<Collider>(true))
            {
                if (col.gameObject == gameObject) continue;

                col.excludeLayers = playerLayer;
            }
        }

        #endregion

#endif
    }
}