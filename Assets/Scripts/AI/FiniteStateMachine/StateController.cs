using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Контроллер состояний, управляющий переключением между состояниями персонажа. </summary>
    public class StateController
    {
        #region Fields and Properties

        /// <summary> Текущее состояние персонажа. </summary>
        private CharacterState _currentState;

        /// <summary> Словарь, хранящий все возможные состояния персонажа, где ключ — тип состояния,
        /// а значение — экземпляр состояния. </summary>
        private readonly Dictionary<Type, CharacterState> _typeToCharacterStates;

        /// <summary> Расписания, отсортированные по приоритетам. </summary>
        private readonly IEnumerable<ScheduleParams> _sortedScheduleParams;

        /// <summary> Событие изменения текущего расписания. </summary>
        private event Action<ScheduleParams> OnCurrentScheduleParamsChanged;

        private NpcMovementController _npcMovementController;

        #endregion

        /// <summary> Конструктор контроллера состояний. </summary>
        /// <param name="npcSchedule"> Набор расписаний. </param>
        /// <param name="animator"> Компонент Animator. </param>
        /// <param name="npcMovementController"> Контроллер движения НПС. </param>
        public StateController(NpcSchedule npcSchedule, Animator animator, NpcMovementController npcMovementController)
        {
            _typeToCharacterStates = new Dictionary<Type, CharacterState>();
            _sortedScheduleParams = npcSchedule.GetSortedScheduleParams();
            if (_sortedScheduleParams == null) Debug.LogError("SortedScheduleParams is null");
            InitializeStates(animator, npcMovementController);

            OnCurrentScheduleParamsChanged += npcMovementController.SetCurrentScheduleParams;
            WorldTime.OnDayEnded += OnReset;
            OnReset(WorldTime.CurrentGameTime);
        }

        /// <summary> Инициализировать состояния. </summary>
        /// <param name="animator"> Компонент Animator. </param>
        /// <param name="npcMovementController"> Контроллер движений НПС. </param>
        private void InitializeStates(Animator animator, NpcMovementController npcMovementController)
        {
            var states = new CharacterState[]
            {
                new InteractionState(), new MovementState(npcMovementController), new RoutineState(animator),
                new WaitingState()
            };

            foreach (var state in states)
            {
                _typeToCharacterStates.Add(state.GetType(), state);
                state.OnStateChangeRequested += SetState;

                if (state is IScheduleDependable dependable)
                    OnCurrentScheduleParamsChanged += dependable.SetCurrentScheduleParams;
            }
        }

        /// <summary> Обновляет логику текущего состояния. </summary>
        /// <param name="deltaTime"> Время в секундах, прошедшее с последнего кадра. </param>
        public void Update(float deltaTime) => _currentState?.Update(deltaTime);

        /// <summary> Обновление состояния NPC. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        private void OnReset(DateTime currentTime)
        {
            PrioritizeSchedule(currentTime);
            ResetStates();
        }

        /// <summary> Приоритизировать расписание. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        private void PrioritizeSchedule(DateTime currentTime)
        {
            bool isRaining = IsWeatherRainyNow();
            int hearts = GetCurrentRelationshipHearts();
            var suitableSchedule = _sortedScheduleParams
                .FirstOrDefault(param => param.AreConditionsSuitable(currentTime, hearts, isRaining));

            OnCurrentScheduleParamsChanged?.Invoke(suitableSchedule);
        }

        /// <summary> Получить текущее количество "сердец" (уровень отношений с NPC). </summary>
        /// <returns> Текущее количество "сердец" (уровень отношений с NPC). </returns>
        private int GetCurrentRelationshipHearts() => 0; // TODO: поменять на получение текущих отношений с данным нпс

        /// <summary> Сейчас дождливая погода? </summary>
        /// <returns> <c>true</c>, если дождливая, <c>false</c>, если ясная.</returns>
        private bool IsWeatherRainyNow() => false; //TODO: поменять на получение текущей погоды из спец. скрипта

        /// <summary> Обновить состояния. </summary>
        private void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(RoutineState));
        }

        /// <summary> Установить текущее состояние. </summary>
        /// <param name="stateType"> Состояние, которое нужно установить. </param>
        private void SetState(Type stateType)
        {
            if (!_typeToCharacterStates.TryGetValue(stateType, out var newState) || _currentState == newState) return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }
    }
}