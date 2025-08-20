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
        /// <summary> Квест, к которому относится этот статус. </summary>
        [field: SerializeField] public Quest Quest { get; private set; }

        /// <summary> Список выполненных целей квеста. </summary>
        [SerializeField] private List<QuestObjective> _completedObjectives;

        /// <summary> Список всех этапов квеста. </summary>
        private List<QuestStage> _stages;

        /// <summary> Индекс текущего этапа квеста. </summary>
        public int CurrentStageIndex { get; private set; }

        /// <summary> Текущие цели из активного этапа квеста. </summary>
        public IEnumerable<QuestObjective> CurrentObjectives =>
            CurrentStageIndex >= 0 && CurrentStageIndex < _stages.Count
                ? _stages[CurrentStageIndex].Objectives
                : Enumerable.Empty<QuestObjective>();

        /// <summary> Квест завершен? </summary>
        public bool IsComplete => CurrentStageIndex >= _stages.Count;

        /// <summary> Награды были получены? </summary>
        public bool IsRewardGiven { get; private set; }

        /// <summary> Конструктор для создания нового статуса квеста. </summary>
        /// <param name="quest"> Квест, для которого создается статус. </param>
        public QuestStatus(Quest quest)
        {
            Quest = quest;
            Initialize();
            CurrentStageIndex = 0;
        }

        /// <summary> Восстанавливает статус квеста из сохраненного состояния. </summary>
        /// <param name="objectState"> Сериализованное состояние. </param>
        public QuestStatus(object objectState)
        {
            if (objectState is not QuestStatusRecord state) return;

            RestoreState(state);
        }

        /// <summary> Инициализация состояния статуса квеста. </summary>
        /// <remarks> Нужна для того, чтобы заданные статусы через Inspector были проинициализированы. </remarks>
        public void Initialize()
        {
            _completedObjectives ??= new List<QuestObjective>();
            _stages ??= Quest.Stages.ToList();
        }

        /// <summary> Пометить, что награды получены. </summary>
        public void MarkRewardGiven() => IsRewardGiven = true;

        /// <summary> Проверяет, завершена ли указанная цель. </summary>
        /// <param name="objective"> Название цели. </param>
        /// <returns> <c>true</c>, если цель завершена; иначе — <c>false</c>. </returns>
        public bool IsObjectiveComplete(QuestObjective objective) => _completedObjectives.Contains(objective);

        /// <summary> Завершает указанную цель квеста. </summary>
        /// <param name="objective"> Цель квеста. </param>
        /// <param name="context"> Контекст выполнения квеста. </param>
        public void CompleteObjective(QuestObjective objective, QuestExecutionContext context)
        {
            if (IsObjectiveComplete(objective)) return;

            _completedObjectives.Add(objective);

            if (IsStageComplete(CurrentStageIndex)) CompleteStage(context);
        }

        /// <summary> Завершает текущий этап и запускает его действия. </summary>
        /// <param name="context"> Контекст выполнения квеста. </param>
        private void CompleteStage(QuestExecutionContext context)
        {
            ExecuteStageActions(CurrentStageIndex, context);
            CurrentStageIndex++;
        }

        /// <summary> Выполняет все действия, связанные с завершением этапа. </summary>
        /// <param name="stageIndex"> Индекс этапа. </param>
        /// <param name="context"> Контекст выполнения квеста. </param>
        private void ExecuteStageActions(int stageIndex, QuestExecutionContext context)
        {
            if (stageIndex < 0 || stageIndex >= _stages.Count) return;

            foreach (var action in _stages[stageIndex].OnStageCompleteActions) action.Execute(context);
        }

        /// <summary> Этап квеста по индексу пройден? </summary>
        /// <param name="stageIndex"> Индекс этапа квеста. </param>
        /// <returns> <c>true</c> - этап квеста пройден, <c>false</c> - в противном случае. </returns>
        public bool IsStageComplete(int stageIndex) => _stages[stageIndex].IsComplete(_completedObjectives);

        #region Saving

        /// <summary> Структура для сериализации состояния статуса квеста. </summary>
        [Serializable]
        private class QuestStatusRecord
        {
            /// <summary> Имя квеста для восстановления ссылки. </summary>
            public string QuestName;

            /// <summary> Индекс текущего этапа квеста. </summary>
            public int CurrentStageIndex;

            /// <summary> Список выполненных целей. </summary>
            public List<string> CompletedObjectiveReferences;
        }

        /// <summary> Сохраняет текущее состояние квеста для сериализации. </summary>
        /// <returns> Сериализованное состояние квеста. </returns>
        public object CaptureState() => new QuestStatusRecord
        {
            QuestName = Quest.QuestName,
            CurrentStageIndex = CurrentStageIndex,
            CompletedObjectiveReferences = _completedObjectives.Select(objective => objective.Reference).ToList()
        };

        /// <summary> Восстанавливает данные о статусе квеста из сохраненного состояния. </summary>
        /// <param name="state"> Сохраненные данные. </param>
        private void RestoreState(QuestStatusRecord state)
        {
            Quest = Quest.GetByName(state.QuestName);
            Initialize();
            CurrentStageIndex = state.CurrentStageIndex;
            _completedObjectives = Quest.Stages
                .SelectMany(stage => stage.Objectives)
                .Where(objective => state.CompletedObjectiveReferences.Contains(objective.Reference))
                .ToList();
        }

        #endregion
    }
}