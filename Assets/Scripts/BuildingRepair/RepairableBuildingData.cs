using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Данные для ремонтируемого здания. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Repairable Building Data")]
    public class RepairableBuildingData : ScriptableObject
    {
        /// <summary> Стадии ремонта здания. </summary>
        [field: Tooltip("Стадии ремонта здания."), SerializeField]
        public List<RepairStage> Stages { get; private set; }
    }
}