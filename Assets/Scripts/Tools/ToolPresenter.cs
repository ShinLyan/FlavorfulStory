using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.Tools
{
    /// <summary> Презентер инструментов. </summary>
    public class ToolPresenter
    {
        /// <summary> Словарь, сопоставляющий тип инструмента с его префабом. </summary>
        private readonly Dictionary<ToolType, GameObject> _toolPrefabs;

        /// <summary> Текущий отображаемый инструмент, прикреплённый к руке игрока. </summary>
        private GameObject _activeTool;

        /// <summary> Конструктор презентера инструментов. </summary>
        /// <param name="toolMappings"> Массив структур, сопоставляющих <see cref="ToolType"/> с его префабом. </param>
        public ToolPresenter(ToolPrefabMapping[] toolMappings)
        {
            _toolPrefabs = new Dictionary<ToolType, GameObject>();
            InitializeToolPrefabs(toolMappings);
        }

        /// <summary> Заполняет словарь на основе переданных маппингов. </summary>
        /// <param name="toolMappings"> Массив структур, сопоставляющих <see cref="ToolType"/> с его префабом. </param>
        private void InitializeToolPrefabs(ToolPrefabMapping[] toolMappings)
        {
            foreach (var toolPrefabMapping in toolMappings)
            {
                if (toolPrefabMapping == null) continue;

                _toolPrefabs[toolPrefabMapping.ToolType] = toolPrefabMapping.ToolPrefab;
            }
        }

        /// <summary> Активирует отображение инструмента в руке игрока на основе его типа. </summary>
        /// <param name="tool"> Инструмент, который нужно отобразить. </param>
        public void EquipTool(Tool tool)
        {
            if (_activeTool) return;

            if (!_toolPrefabs.TryGetValue(tool.ToolType, out var prefab) || !prefab) return;

            _activeTool = prefab;
            _activeTool.SetActive(true);
        }

        /// <summary> Отключает отображение текущего инструмента и разблокирует управление игроком. </summary>
        public void UnequipTool()
        {
            if (!_activeTool) return;

            _activeTool.SetActive(false);
            _activeTool = null;
        }
    }
}