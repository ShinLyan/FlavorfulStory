using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Структура, определяющая стадию ремонта здания. </summary>
    [System.Serializable]
    public struct RepairStage
    {
        /// <summary> Название объекта на данной стадии ремонта. </summary>
        [Tooltip("Название объекта на данной стадии ремонта.")]
        public string BuildingName;
        
        /// <summary> Ресурсные требования для выполнения стадии ремонта. </summary>
        [Tooltip("Ресурсные требования для выполнения стадии ремонта.")]
        public List<ResourceRequirement> Requirements;
    }
}