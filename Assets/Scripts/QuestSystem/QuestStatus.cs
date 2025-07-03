using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Статус квеста для отслеживания его прогресса игроком. </summary>
    [Serializable]
    public class QuestStatus
    {
        /// <summary> Ссылка на квест, к которому относится этот статус. </summary>
        [field: SerializeField] public Quest Quest { get; private set; }

        /// <summary> Список выполненных целей квеста. </summary>
        [SerializeField] private List<string> _completedObjectives;

        /// <summary> Проверяет, завершена ли указанная цель. </summary>
        /// <param name="objective"> Название цели. </param>
        /// <returns> True, если цель завершена; иначе — false. </returns>
        public bool IsObjectiveComplete(string objective) => _completedObjectives.Contains(objective);
    }
}