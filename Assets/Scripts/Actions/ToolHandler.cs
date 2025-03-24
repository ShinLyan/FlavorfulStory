using System;
using System.Linq;
using FlavorfulStory.InputSystem;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Отвечает за отображение и управление визуальным отображением инструментов в руке игрока. </summary>
    /// <remarks> Также блокирует/разблокирует ввод, связанный с прокруткой тулбара. </remarks>
    public class ToolHandler : MonoBehaviour
    {
        /// <summary> Текущий отображаемый инструмент, прикреплённый к руке игрока. </summary>
        private GameObject _currentTool;

        /// <summary> Действие, что должно выполниться при UnEquip'е инструмента. </summary>
        private Action _unequipAction;

        /// <summary> Сопоставления типов инструментов с их префабами для визуализации в руке игрока. </summary>
        [SerializeField] private ToolPrefabMapping[] _toolMappings;

        /// <summary> Слои, по которым производится удар с помощью инструмента. </summary>
        [field: SerializeField]
        public LayerMask HitableLayers { get; private set; }

        /// <summary> Задать действие анэквипа. </summary>
        /// <param name="action"> Действие анэквипа. </param>
        public void SetUnequipAction(Action action) => _unequipAction = action;

        /// <summary> Активирует визуальное отображение инструмента на основе переданного объекта Tool. </summary>
        /// <param name="tool">Инструмент, который нужно отобразить.</param>
        /// <remarks> Также блокирует прокрутку мыши, чтобы исключить случайную смену предмета. </remarks>
        public void Equip(Tool tool)
        {
            if (!tool || _currentTool) return;

            var mapping = _toolMappings.FirstOrDefault(m => m.ToolType == tool.ToolType);
            if (mapping == null) return;

            _currentTool = mapping.ToolPrefab;
            _currentTool.SetActive(true);
            InputWrapper.BlockInput(InputButton.MouseScroll);
        }

        /// <summary> Удаляет текущий отображаемый инструмент из руки игрока. </summary>
        /// <remarks> Также разблокирует ввод (прокрутку мыши и движение). </remarks>
        public void Unequip()
        {
            if (!_currentTool) return;

            _currentTool.SetActive(false);
            _currentTool = null;

            InputWrapper.UnblockPlayerMovement();
            InputWrapper.UnblockInput(InputButton.MouseScroll);

            _unequipAction?.Invoke();
        }
    }
}