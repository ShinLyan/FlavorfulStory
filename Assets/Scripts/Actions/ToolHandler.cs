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
        /// <summary> Сопоставления типов инструментов с их префабами для визуализации в руке игрока. </summary>
        [Tooltip("Сопоставления типов инструментов с их префабами для визуализации в руке игрока."), SerializeField]
        private ToolPrefabMapping[] _toolMappings;

        /// <summary> Слои, по которым производится удар с помощью инструмента. </summary>
        [field: Tooltip("Слои, по которым производится удар с помощью инструмента. " +
                        "Выбирать Default, Obstacle, Terrain"), SerializeField]
        public LayerMask HitableLayers { get; private set; }

        /// <summary> Текущий отображаемый инструмент, прикреплённый к руке игрока. </summary>
        private GameObject _currentTool;

        /// <summary> Действие, что должно выполниться при UnEquip'е инструмента. </summary>
        public Action UnequipAction;

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
        /// <remarks> Вызывается при окончании анимации. Также разблокирует ввод (прокрутку мыши и движение). </remarks>
        public void Unequip()
        {
            if (!_currentTool) return;

            _currentTool.SetActive(false);
            _currentTool = null;

            InputWrapper.UnblockPlayerInput();
            UnequipAction?.Invoke();
        }
    }
}