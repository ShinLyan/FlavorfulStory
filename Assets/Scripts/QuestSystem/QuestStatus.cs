using System;
using System.Collections.Generic;

namespace FlavorfulStory.QuestSystem
{
    /// <summary> Статус квеста для отслеживания его прогресса игроком. </summary>
    public class QuestStatus
    {
        /// <summary> Ссылка на квест, к которому относится этот статус. </summary>
        public Quest Quest { get; }

        public bool IsComplete => true;

        /// <summary> Список выполненных целей квеста. </summary>
        private readonly List<string> _completedObjectives;

        public QuestStatus(Quest quest)
        {
            Quest = quest;
            _completedObjectives = new List<string>();
        }

        public QuestStatus(object objectState)
        {
            if (objectState is not QuestStatusRecord state) return;

            Quest = Quest.GetByName(state.QuestName);
            _completedObjectives = state.CompletedObjectives;
        }

        /// <summary> Проверяет, завершена ли указанная цель. </summary>
        /// <param name="objective"> Название цели. </param>
        /// <returns> True, если цель завершена; иначе — false. </returns>
        public bool IsObjectiveComplete(string objective) => _completedObjectives.Contains(objective);

        public void CompleteObjective(string objective)
        {
            if (Quest.HasObjective(objective)) _completedObjectives.Add(objective);
        }

        [Serializable]
        private class QuestStatusRecord
        {
            public string QuestName;
            public List<string> CompletedObjectives;
        }

        public object CaptureState() => new QuestStatusRecord
        {
            QuestName = Quest.QuestName,
            CompletedObjectives = _completedObjectives
        };
    }
}