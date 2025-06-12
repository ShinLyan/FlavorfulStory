using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.TimeManagement;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public class StateController
    {
        /// <summary> Текущее активное состояние персонажа. </summary>
        private CharacterState _currentState;

        /// <summary> Словарь для быстрого доступа к состояниям по их типу. </summary>
        private readonly Dictionary<Type, CharacterState> _typeToCharacterStates;

        /// <summary> Отсортированные параметры расписания персонажа. </summary>
        private readonly IEnumerable<ScheduleParams> _sortedScheduleParams;

        /// <summary> Событие, вызываемое при изменении текущих параметров расписания. </summary>
        private event Action<ScheduleParams> OnCurrentScheduleParamsChanged;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcSchedule"> Расписание NPC. </param>
        /// <param name="npcMovementController"> Контроллер движения NPC. </param>
        /// <param name="npcAnimatorController"> Контроллер анимации NPC. </param>
        /// <param name="scheduleHandler"> Обработчик расписания NPC. </param>
        public StateController(NpcSchedule npcSchedule, NpcMovementController npcMovementController,
            NpcAnimatorController npcAnimatorController, NpcScheduleHandler scheduleHandler)
        {
            _typeToCharacterStates = new Dictionary<Type, CharacterState>();

            _sortedScheduleParams = npcSchedule.GetSortedScheduleParams();
            InitializeStates(npcMovementController, scheduleHandler, npcAnimatorController);

            OnCurrentScheduleParamsChanged += npcMovementController.SetCurrentScheduleParams;
            WorldTime.OnDayEnded += OnReset;
            OnReset(WorldTime.CurrentGameTime);
        }

        /// <summary> Инициализирует все доступные состояния персонажа и настраивает связи между ними. </summary>
        /// <param name="movementController"> Контроллер движения для состояния движения. </param>
        /// <param name="scheduleHandler"> Обработчик расписания для связи с состояниями. </param>
        /// <param name="animatorController"> Контроллер анимации для состояний. </param>
        private void InitializeStates(NpcMovementController movementController,
            NpcScheduleHandler scheduleHandler,
            NpcAnimatorController animatorController)
        {
            var states = new CharacterState[]
            {
                new InteractionState(), new MovementState(movementController),
                new RoutineState(scheduleHandler, animatorController), new WaitingState()
            };

            foreach (var state in states)
            {
                _typeToCharacterStates.Add(state.GetType(), state);
                state.OnStateChangeRequested += SetState;

                if (state is ICurrentSchedulePointDependable dependable)
                    scheduleHandler.OnSchedulePointChanged += dependable.SetNewCurrentPont;
            }
        }

        /// <summary> Обновляет текущее состояние персонажа каждый кадр. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public void Update(float deltaTime) => _currentState?.Update(deltaTime);

        /// <summary> Сбрасывает систему состояний при смене дня или инициализации. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        private void OnReset(DateTime currentTime)
        {
            PrioritizeSchedule(currentTime);
            ResetStates();
        }

        /// <summary> Определяет приоритетное расписание на основе текущих условий игры. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        private void PrioritizeSchedule(DateTime currentTime)
        {
            bool isRaining = false; // TODO
            int hearts = 0; // TODO

            var suitable = _sortedScheduleParams.FirstOrDefault(param =>
                param.AreConditionsSuitable(currentTime, hearts, isRaining));

            OnCurrentScheduleParamsChanged?.Invoke(suitable);
        }

        /// <summary> Сбрасывает все состояния к начальному и устанавливает состояние рутины. </summary>
        private void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(RoutineState));
        }

        /// <summary> Устанавливает новое состояние персонажа по типу. </summary>
        /// <param name="type"> Тип состояния для установки. </param>
        private void SetState(Type type)
        {
            if (!_typeToCharacterStates.TryGetValue(type, out var next) || _currentState == next) return;

            _currentState?.Exit();
            _currentState = next;
            _currentState?.Enter();
        }
    }
}