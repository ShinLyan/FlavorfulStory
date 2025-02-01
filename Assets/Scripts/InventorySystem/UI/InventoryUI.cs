using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Управление отображением инвентаря в пользовательском интерфейсе. </summary>
    public class InventoryUI : MonoBehaviour
    {
        /// <summary> Префаб слота для отображения предмета. </summary>
        [SerializeField] private InventorySlotUI _inventorySlotPrefab;

        /// <summary> Место, где создаются слоты инвентаря. </summary>
        private Transform _placeToSpawnSlots;

        /// <summary> Ссылка на инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Инициализация инвентаря и подписка на обновление инвентаря. </summary>
        private void Awake()
        {
            _playerInventory = Inventory.PlayerInventory;
            _playerInventory.InventoryUpdated += RedrawInventory;

            _placeToSpawnSlots = transform;
        }

        /// <summary> Первичное обновление отображения инвентаря. </summary>
        private void Start()
        {
            RedrawInventory();
        }

        /// <summary> Обновление отображения инвентаря. </summary>
        private void RedrawInventory()
        {
            DestroyAllSlots();
            SpawnInventorySlots();
        }

        /// <summary> Удаление всех существующих слотов инвентаря. </summary>
        private void DestroyAllSlots()
        {
            foreach (Transform child in _placeToSpawnSlots)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary> Создание новых слотов для отображения предметов инвентаря. </summary>
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