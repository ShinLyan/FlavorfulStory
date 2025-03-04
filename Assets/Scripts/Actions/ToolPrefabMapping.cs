using System;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Сопоставление типа инструмента с его префабом. </summary>
    [Serializable]
    public class ToolPrefabMapping
    {
        /// <summary> Тип инструмента. </summary>
        [field: Tooltip("Тип инструмента."), SerializeField]
        public ToolType ToolType { get; private set; }

        /// <summary> Префаб инструмента. </summary>
        [field: Tooltip("Префаб инструмента."), SerializeField]
        public GameObject ToolPrefab { get; private set; }
    }
}