using System;
using FlavorfulStory.BuildingRepair;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.Objectives.Params
{
    /// <summary> Параметры цели на ремонт здания определённого типа. </summary>
    [Serializable]
    public class RepairObjectiveParams : ObjectiveParamsBase
    {
        /// <summary> Тип ремонтируемого здания. </summary>
        [field: Tooltip("Тип ремонтируемого здания."), SerializeField]
        public RepairableBuildingName Name { get; private set; }

        /// <summary> Проверяет, соответствует ли отремонтированное здание заданному типу. </summary>
        /// <param name="context"> Контекст выполнения цели. </param>
        /// <param name="eventData"> Тип отремонтированного здания. </param>
        /// <returns> True, если тип здания совпадает с требуемым. </returns>
        protected override bool ShouldComplete(QuestExecutionContext context, object eventData)
        {
            if (eventData is not RepairableBuildingName repairableBuildingType) return false;
            return repairableBuildingType == Name;
        }
    }
}