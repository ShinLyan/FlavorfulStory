using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    public class StateController
    {
        private CharacterState _currentState;
        private readonly Dictionary<Type, CharacterState> _typeToCharacterStates;
        private readonly IEnumerable<ScheduleParams> _sortedScheduleParams;
        private event Action<ScheduleParams> OnCurrentScheduleParamsChanged;

        public StateController(NpcSchedule npcSchedule, Animator animator, NpcMovementController npcMovementController)
        {
            _typeToCharacterStates = new Dictionary<Type, CharacterState>();
            _sortedScheduleParams = npcSchedule.GetSortedScheduleParams();
            InitializeStates(animator, npcMovementController);

            OnCurrentScheduleParamsChanged += npcMovementController.SetCurrentScheduleParams;
            WorldTime.OnDayEnded += OnReset;
            OnReset(WorldTime.CurrentGameTime);
        }

        private void InitializeStates(Animator animator, NpcMovementController movementController)
        {
            var states = new CharacterState[]
            {
                new InteractionState(), new MovementState(movementController), new RoutineState(animator),
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

        public void Update(float deltaTime) => _currentState?.Update(deltaTime);

        private void OnReset(DateTime currentTime)
        {
            PrioritizeSchedule(currentTime);
            ResetStates();
        }

        private void PrioritizeSchedule(DateTime currentTime)
        {
            bool isRaining = false; // TODO
            int hearts = 0; // TODO

            var suitable = _sortedScheduleParams.FirstOrDefault(param =>
                param.AreConditionsSuitable(currentTime, hearts, isRaining));

            OnCurrentScheduleParamsChanged?.Invoke(suitable);
        }

        private void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(RoutineState));
        }

        private void SetState(Type type)
        {
            if (!_typeToCharacterStates.TryGetValue(type, out var next) || _currentState == next) return;

            _currentState?.Exit();
            _currentState = next;
            _currentState?.Enter();
        }
    }
}