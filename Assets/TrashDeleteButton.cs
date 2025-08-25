using UnityEngine;
using UnityEngine.UI;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory
{
    public class TrashDeleteButton : MonoBehaviour
    {
        [SerializeField] private Button _deleteButton;

        [SerializeField] private Inventory _trashInventory;

        private void Awake() => _deleteButton.onClick.AddListener(OnDeleteClicked);

        private void OnDestroy() => _deleteButton.onClick.RemoveListener(OnDeleteClicked);

        private void OnDeleteClicked()
        {
            var item = _trashInventory.GetItemInSlot(0);
            if (item != null)
                _trashInventory.RemoveFromSlot(0);
        }
    }
}