using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary> Флаг, указывающий, что квест завершен (пока всегда true). </summary>
        public bool IsComplete => Quest.Objectives.All(objective => _completedObjectives.Contains(objective.Reference));

        /// <summary> Конструктор для создания нового статуса квеста. </summary>
        /// <param name="quest"> Квест, для которого создается статус. </param>
        public QuestStatus(Quest quest)
        {
            Quest = quest;
            _completedObjectives = new List<string>();
        }

        /// <summary> Восстанавливает статус квеста из сохраненного состояния. </summary>
        /// <param name="objectState"> Сериализованное состояние. </param>
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

        /// <summary> Завершает указанную цель квеста. </summary>
        /// <param name="objective"> Название цели. </param>
        public void CompleteObjective(string objective)
        {
            if (Quest.HasObjective(objective)) _completedObjectives.Add(objective);
        }

        /// <summary> Структура для сериализации состояния статуса квеста. </summary>
        [Serializable]
        private class QuestStatusRecord
        {
            /// <summary> Имя квеста для восстановления ссылки. </summary>
            public string QuestName;

            /// <summary> Список выполненных целей. </summary>
            public List<string> CompletedObjectives;
        }

        /// <summary> Сохраняет текущее состояние квеста для сериализации. </summary>
        /// <returns> Сериализованное состояние квеста. </returns>
        public object CaptureState() => new QuestStatusRecord
        {
            QuestName = Quest.QuestName,
            CompletedObjectives = _completedObjectives
        };
    }
}