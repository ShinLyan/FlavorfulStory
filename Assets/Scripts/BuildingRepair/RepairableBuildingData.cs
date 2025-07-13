using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Данные для ремонтируемого здания. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Repairable Building Data")]
    public class RepairableBuildingData : ScriptableObject
    {
        /// <summary> Название ремонтируемого здания. </summary>
        [field: Tooltip("Название ремонтируемого здания."), SerializeField]
        public RepairableBuildingName Name { get; private set; }

        /// <summary> Стадии ремонта здания. </summary>
        [field: Tooltip("Стадии ремонта здания."), SerializeField]
        public List<RepairStage> Stages { get; private set; }
    }
}