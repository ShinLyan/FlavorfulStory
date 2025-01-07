using System;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Коллбек из UnityAPI. </summary>
    [RequireComponent(typeof(Toolbar))]
    public class ToolbarScroll : MonoBehaviour
    {
        // Expose GetSlotsCount() int Toolbar.cs or put here const = 9
        [SerializeField] private int _toolbarSize;
        
        /// <summary> Поле тулбара. </summary>
        private Toolbar _toolbar;
        
        /// <summary> Коллбек из UnityAPI. </summary>
        private void Awake()
        {
            _toolbar = GetComponent<Toolbar>();
        }

        /// <summary> Коллбек из UnityAPI. </summary>
        private void Update()
        {
            HandleMouseScrollInput();
        }

        /// <summary> Метод обработки ввода колесика мыши. </summary>
        private void HandleMouseScrollInput()
        {
            var scrollInput = (int) Input.mouseScrollDelta.y;
            var newSelectedItemIndex = Math.Clamp(_toolbar.SelectedItemIndex - scrollInput, 0, _toolbarSize - 1);
            _toolbar.SelectItem(newSelectedItemIndex);
        }
    }
}