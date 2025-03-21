using System.Linq;
using FlavorfulStory.Actions;
using FlavorfulStory.InputSystem;
using UnityEngine;

namespace FlavorfulStory
{
    public class ToolHandler : MonoBehaviour
    {
        [SerializeField] private ToolPrefabMapping[] _toolMappings;

        [field: SerializeField] public LayerMask HitableLayers { get; private set; }

        private GameObject _currentTool;

        public void Equip(Tool tool)
        {
            if (tool == null || _currentTool != null) return;

            var mapping = _toolMappings.FirstOrDefault(m => m.ToolType == tool.ToolType);
            if (mapping == null) return;

            _currentTool = mapping.ToolPrefab;
            _currentTool.SetActive(true);

            InputWrapper.BlockInput(InputButton.MouseScroll);
        }

        public void Unequip()
        {
            if (_currentTool == null) return;

            _currentTool.SetActive(false);
            _currentTool = null;

            InputWrapper.UnblockPlayerMovement();
            InputWrapper.UnblockInput(InputButton.MouseScroll);
        }
    }
}