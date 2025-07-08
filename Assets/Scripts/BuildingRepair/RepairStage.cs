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
        [field: Tooltip("Название объекта на данной стадии ремонта."), SerializeField]
        public string BuildingName { get; private set; }

        /// <summary> Ресурсные требования для выполнения стадии ремонта. </summary>
        [field: Tooltip("Ресурсные требования для выполнения стадии ремонта."), SerializeField]
        public List<ItemRequirement> Requirements { get; private set; }
    }
}