using System;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Сопоставление типа инструмента с его префабом. </summary>
    [Serializable]
    public class ToolPrefabMapping
    {
        /// <summary> Тип инструмента. </summary>
        [Tooltip("Тип инструмента.")] public ToolType ToolType;

        /// <summary> Префаб инструмента. </summary>
        [Tooltip("Префаб инструмента.")] public GameObject ToolPrefab;
    }
}