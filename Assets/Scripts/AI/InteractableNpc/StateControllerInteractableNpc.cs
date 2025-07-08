using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.Player;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public class StateControllerInteractableNpc : StateController, ICharacterCollisionHandler
    {
        /// <summary> Обработчик расписания NPC </summary>
        private readonly NpcScheduleHandler _scheduleHandler;

        /// <summary> Событие, вызываемое при изменении текущих параметров расписания. </summary>
        private event Action<ScheduleParams> OnCurrentScheduleParamsChanged;

        /// <summary> Отсортированные параметры расписания для быстрого поиска подходящего. </summary>
        private readonly IEnumerable<ScheduleParams> _sortedScheduleParams;

        /// <summary> Контроллер движения для интерактивного NPC. </summary>
        private readonly InteractableNpcMovementController _npcMovementController;

        /// <summary> Контроллер игрока для взаимодействия. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Transform NPC для определения позиции. </summary>
        private readonly Transform _npcTransform;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcSchedule"> Расписание NPC. </param>
        /// <param name="npcMovementController"> Контроллер движения NPC. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        /// <param name="scheduleHandler"> Обработчик расписания NPC. </param>
        /// <param name="playerController"> Контроллер игрока для взаимодействия. </param>
        /// <param name="npcTransform"> Transform NPC для определения позиции. </param>
        public StateControllerInteractableNpc(NpcSchedule npcSchedule,
            InteractableNpcMovementController npcMovementController,
            NpcAnimationController npcAnimationController, NpcScheduleHandler scheduleHandler,
            PlayerController playerController, Transform npcTransform)
            : base(npcAnimationController)
        {
            _sortedScheduleParams = npcSchedule.GetSortedScheduleParams();
            _scheduleHandler = scheduleHandler;
            _npcMovementController = npcMovementController;
            _playerController = playerController;
            _npcTransform = npcTransform;
            Initialize();
        }

        /// <summary> Подписывается на события системы времени и расписания. </summary>
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _scheduleHandler.OnSchedulePointChanged += OnSchedulePointChanged;
            OnCurrentScheduleParamsChanged += _scheduleHandler.SetCurrentScheduleParams;
        }

        /// <summary> Инициализирует все состояния для интерактивного NPC. </summary>
        /// <remarks> Создает состояния взаимодействия, движения, рутины и ожидания, настраивает связи между ними. </remarks>
        protected override void InitializeStates()
        {
            var states = new CharacterState[]
            {
                new InteractionState(), new MovementState(_npcMovementController),
                new RoutineState(_animationController), new WaitingState(_playerController, _npcTransform)
            };

            foreach (var state in states)
            {
                _typeToCharacterStates.Add(state.GetType().ToString(), state);
                state.OnStateChangeRequested += SetState;

                if (state is ICurrentSchedulePointDependable dependable)
                    _scheduleHandler.OnSchedulePointChanged += dependable.SetNewCurrentPoint;
            }

            _scheduleHandler.OnSchedulePointChanged += OnSchedulePointChanged;
        }

        /// <summary> Обрабатывает изменение точки расписания и переключает состояние на движение если необходимо. </summary>
        /// <param name="newPoint"> Новая точка расписания. </param>
        private void OnSchedulePointChanged(SchedulePoint newPoint)
        {
            if (_currentState is WaitingState) return;

            if (_currentState is MovementState)
                _currentState.Enter();
            else
                SetState(typeof(MovementState).ToString());

            _animationController.TriggerAnimation(AnimationType.Locomotion);
        }

        /// <summary> Сбрасывает все состояния и устанавливает состояние рутины. </summary>
        protected override void ResetStates()
        {
            foreach (var state in _typeToCharacterStates.Values) state.Reset();
            SetState(typeof(RoutineState).ToString());
        }

        /// <summary> Выполняет приоритизацию расписания перед сбросом состояний. </summary>
        /// <param name="currentTime"> Текущее игровое время. </param>
        protected override void OnPreReset(DateTime currentTime) => PrioritizeSchedule(currentTime);

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

        /// <summary> Вызывается при входе игрока в триггер NPC. Переводит NPC в состояние ожидания. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        public void OnTriggerEntered(Collider other)
        {
            SetState(typeof(WaitingState).ToString());
            _animationController.TriggerAnimation(AnimationType.Idle);
        }

        /// <summary> Вызывается при выходе игрока из триггера NPC. Переводит NPC в состояние движения. </summary>
        /// <param name="other"> Коллайдер, вышедший из триггера. </param>
        public void OnTriggerExited(Collider other)
        {
            SetState(typeof(MovementState).ToString());
            _animationController.TriggerAnimation(AnimationType.Locomotion);
        }
    }
}