using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Инвентарь в UI - управляет созданием всех предметов инвентаря в UI.</summary>
    public class InventoryUI : MonoBehaviour
    {
        /// <summary> Префаб слота инвентаря.</summary>
        [SerializeField] private InventorySlotUI _inventorySlotPrefab;

        /// <summary> Место для спавна слотов инвентаря.</summary>
        private Transform _placeToSpawnSlots;

        /// <summary> Инвентарь игрока.</summary>
        private Inventory _playerInventory;

        /// <summary> Инициализация объекта.</summary>
        private void Awake()
        {
            _playerInventory = Inventory.PlayerInventory;
            _playerInventory.InventoryUpdated += RedrawInventory;

            _placeToSpawnSlots = transform;
        }

        /// <summary> При старте перерисовываем инвентарь.</summary>
        private void Start()
        {
            RedrawInventory();
        }

        /// <summary> Перерисовать инвентарь.</summary>
        private void RedrawInventory()
        {
            DestroyAllSlots();
            SpawnInventorySlots();
        }

        /// <summary> Уничтожить все слоты инвентаря.</summary>
        private void DestroyAllSlots()
        {
            foreach (Transform child in _placeToSpawnSlots)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary> Создание слотов инвентаря.</summary>
        private void SpawnInventorySlots()
        {
            for (int index = 0; index < _playerInventory.InventorySize; index++)
            {
                var itemUI = Instantiate(_inventorySlotPrefab, _placeToSpawnSlots);
                itemUI.Setup(_playerInventory, index);
            }
        }
    }
}