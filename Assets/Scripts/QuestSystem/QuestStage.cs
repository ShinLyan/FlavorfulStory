using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary> Цели квеста (только для чтения). </summary>
        public IEnumerable<QuestObjective> Objectives => _objectives;

        /// <summary> Проверяет, выполнены ли все цели этой стадии. </summary>
        /// <param name="completedObjectives"> Коллекция выполненных целей. </param>
        /// <returns> True, если все цели выполнены. </returns>
        public bool IsComplete(IEnumerable<QuestObjective> completedObjectives) =>
            _objectives.All(completedObjectives.Contains);
    }
}