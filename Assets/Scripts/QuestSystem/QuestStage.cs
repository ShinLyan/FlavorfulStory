using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.QuestSystem.TriggerActions;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Стадия квеста, содержащая набор целей, необходимых для завершения. </summary>
    [Serializable]
    public class QuestStage
    {
        /// <summary> Список целей квеста. </summary>
        [Tooltip("Список целей квеста."), SerializeField]
        private List<QuestObjective> _objectives;

        /// <summary> Действия, выполняемые после завершения этого этапа. </summary>
        [Tooltip("Действия, выполняемые после завершения этого этапа."), SerializeReference]
        private List<QuestTriggerAction> _onStageCompleteActions;

        /// <summary> Коллекция действий, запускаемых после завершения этапа. </summary>
        public IEnumerable<QuestTriggerAction> OnStageCompleteActions => _onStageCompleteActions;

        /// <summary> Цели текущей стадии квеста (только для чтения). </summary>
        public IEnumerable<QuestObjective> Objectives => _objectives;

        /// <summary> Проверяет, выполнены ли все цели этой стадии. </summary>
        /// <param name="completedObjectives"> Коллекция выполненных целей. </param>
        /// <returns> True, если все цели выполнены. </returns>
        public bool IsComplete(IEnumerable<QuestObjective> completedObjectives) =>
            _objectives.All(completedObjectives.Contains);
    }
}