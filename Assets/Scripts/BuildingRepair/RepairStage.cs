using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Структура, определяющая стадию ремонта здания. </summary>
    [Serializable]
    public struct RepairStage
    {
        /// <summary> Название объекта на данной стадии ремонта. </summary>
        [Tooltip("Ресурсные требования для выполнения стадии ремонта.")]
        public string ObjectName;

        /// <summary> Префаб для визуального представления этой стадии ремонта. </summary>
        [Tooltip("Префаб объекта выполненной стадии ремонта.")]
        public GameObject Gameobject;

        /// <summary> Список требуемых ресурсов для завершения этой стадии. </summary>
        [Tooltip("Ресурсные требования для выполнения стадии ремонта.")]
        public List<ResourceRequirement> Requirements;
    }
}