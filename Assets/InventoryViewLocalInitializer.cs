using UnityEngine;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;

namespace FlavorfulStory
{
    
    public class InventoryViewLocalInitializer : MonoBehaviour
    {
        [SerializeField] private InventoryView _view;
        [SerializeField] private Inventory _inventory;

        private void Awake()
        {
            if (_view && _inventory) _view.Initialize(_inventory);
            else Debug.LogError("InventoryViewLocalInitializer: assign View and Inventory.");
        }
    }
}