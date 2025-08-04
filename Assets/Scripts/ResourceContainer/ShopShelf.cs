using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Отображает предметы из инвентаря на 3D-полке,
    /// спавня их PickupPrefab в слотах из InventoryTransformPlacer. </summary>
    [RequireComponent(typeof(Inventory)), RequireComponent(typeof(InventoryTransformPlacer))]
    public class ShopShelf : MonoBehaviour
    {
        /// <summary> Инвентарь, откуда берутся предметы для отображения. </summary>
        private Inventory _inventory;
        /// <summary> Компонент, отвечающий за размещение предметов в слотах. </summary>
        private InventoryTransformPlacer _placer;

        /// <summary> Инициализирует зависимости и проверяет наличие компонентов. </summary>
        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _placer = GetComponent<InventoryTransformPlacer>();

            if (_inventory == null || _placer == null)
            {
                Debug.LogError("[ShopShelf] Missing required components.");
                enabled = false;
                return;
            }
        }
    }
}