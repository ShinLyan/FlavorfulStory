using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Конфигурация для системы ремонта зданий. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Building/RepairConfig", fileName = "RepairConfig")]
    public class BuildingRepairConfig : ScriptableObject
    {
        /// <summary> Префаб объекта в неотремонтированном состоянии. </summary>
        [Tooltip("Префаб ремонтируемого объекта по умолчанию")]
        public GameObject DefaultGameObject;

        /// <summary> Список стадий ремонта здания. </summary>
        [Tooltip("Стадии строительства")] public List<RepairStage> Stages;
    }
}