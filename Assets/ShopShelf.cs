using System;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory
{
    //TODO: Добавить сохраняемость, в том числе для тех, кто спавнится в рантайме
    /// <summary>
    /// Отображает предметы из инвентаря на 3D-полке, спавня их PickupPrefab в слотах из InventoryTransformPlacer.
    /// </summary>
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(InventoryTransformPlacer))]
    public class ShopShelf : MonoBehaviour
    {
        private Inventory _inventory;
        private InventoryTransformPlacer _placer;

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