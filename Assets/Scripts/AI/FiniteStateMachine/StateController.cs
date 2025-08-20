using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public class StateController : ICharacterCollisionHandler
    {
        /// <summary> Текущее активное состояние персонажа. </summary>
        private CharacterState _currentState;

        /// <summary> Словарь для быстрого доступа к состояниям по их типу. </summary>
        private readonly Dictionary<Type, CharacterState> _typeToCharacterStates;

        /// <summary> Отсортированные параметры расписания персонажа. </summary>
        private readonly IEnumerable<ScheduleParams> _sortedScheduleParams;

        /// <summary> Событие, вызываемое при изменении текущих параметров расписания. </summary>
        private event Action<ScheduleParams> OnCurrentScheduleParamsChanged;

        /// <summary> Контроллер анимации NPC для управления анимационными состояниями. </summary>
        private readonly NpcAnimationController _animationController;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcSchedule"> Расписание NPC. </param>
        /// <param name="npcMovementController"> Контроллер движения NPC. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        /// <param name="scheduleHandler"> Обработчик расписания NPC. </param>
        /// <param name="playerController"> Контроллер игрока для взаимодействия. </param>
        /// <param name="npcTransform"> Transform NPC для определения позиции. </param>
        public StateController(NpcSchedule npcSchedule, NpcMovementController npcMovementController,
            NpcAnimationController npcAnimationController, NpcScheduleHandler scheduleHandler,
            PlayerController playerController, Transform npcTransform)
        {
            _typeToCharacterStates = new Dictionary<Type, CharacterState>();

            _sortedScheduleParams = npcSchedule.GetSortedScheduleParams();
            _animationController = npcAnimationController;
            InitializeStates(npcMovementController, scheduleHandler, npcAnimationController, playerController,
                npcTransform);

            OnCurrentScheduleParamsChanged += npcMovementController.SetCurrentScheduleParams;
            WorldTime.OnDayEnded += OnReset;
            OnReset(WorldTime.CurrentDateTime);
        }

        /// <summary> Инициализирует все доступные состояния персонажа и настраивает связи между ними. </summary>
        /// <param name="movementController"> Контроллер движения для состояния движения. </param>
        /// <param name="scheduleHandler"> Обработчик расписания для связи с состояниями. </param>
        /// <param name="animationController"> Контроллер анимации для состояний. </param>
        /// <param name="playerController"> Контроллер игрока для состояния ожидания. </param>
        /// <param name="npcTransform"> Transform NPC для передачи в состояния. </param>
        private void InitializeStates(NpcMovementController movementController,
            NpcScheduleHandler scheduleHandler,
            NpcAnimationController animationController,
            PlayerController playerController,
            Transform npcTransform)
        {
            var states = new CharacterState[]
            {
                new InteractionState(), new MovementState(movementController),
                new RoutineState(animationController), new WaitingState(playerController, npcTransform)
            };

            foreach (var state in states)
            {
                _typeToCharacterStates.Add(state.GetType(), state);
                state.OnStateChangeRequested += SetState;

                if (state is ICurrentSchedulePointDependable dependable)
                    scheduleHandler.OnSchedulePointChanged += dependable.SetNewCurrentPoint;
            }

            scheduleHandler.OnSchedulePointChanged += OnSchedulePointChanged;
        }

        /// <summary> Обрабатывает изменение точки расписания и переключает состояние на движение если необходимо. </summary>
        /// <param name="newPoint"> Новая точка расписания. </param>
        private void OnSchedulePointChanged(SchedulePoint newPoint)
        {
            if (_currentState is WaitingState) return;

            if (_currentState is MovementState)
                _currentState.Enter();
            else
                SetState(typeof(MovementState));

            _animationController.TriggerAnimation(AnimationType.Locomotion);
        }

        /// <summary> Обновляет текущее состояние персонажа каждый кадр. </summary>
        public void Update() => _currentState?.Update();

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

        /// <summary> Вызывается при входе игрока в триггер NPC. Переводит NPC в состояние ожидания. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        public void OnTriggerEntered(Collider other)
        {
            SetState(typeof(WaitingState));
            _animationController.TriggerAnimation(AnimationType.Idle);
        }

        /// <summary> Вызывается при выходе игрока из триггера NPC. Переводит NPC в состояние движения. </summary>
        /// <param name="other"> Коллайдер, вышедший из триггера. </param>
        public void OnTriggerExited(Collider other)
        {
            SetState(typeof(MovementState));
            _animationController.TriggerAnimation(AnimationType.Locomotion);
        }
    }
}