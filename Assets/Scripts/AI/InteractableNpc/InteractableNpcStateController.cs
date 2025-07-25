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
    public class InteractableNpcStateController : StateController, ICharacterCollisionHandler
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
        public InteractableNpcStateController(NpcSchedule npcSchedule,
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
            _nameToCharacterStates.Add(StateName.Interaction, new InteractionState());
            _nameToCharacterStates.Add(StateName.Movement, new MovementState(_npcMovementController));
            _nameToCharacterStates.Add(StateName.Routine, new RoutineState(_animationController));
            _nameToCharacterStates.Add(StateName.Waiting, new WaitingState(_playerController, _npcTransform));

            foreach (var state in _nameToCharacterStates.Values)
            {
                state.OnStateChangeRequested += SetState;

                if (state is ICurrentSchedulePointDependable dependable)
                    _scheduleHandler.OnSchedulePointChanged += dependable.SetNewCurrentPoint;
            }

            _scheduleHandler.OnSchedulePointChanged += OnSchedulePointChanged;
        }

        protected override void OnReset(DateTime currentTime)
        {
            PrioritizeSchedule(currentTime);
            base.OnReset(currentTime);
        }

        /// <summary> Сбрасывает все состояния и устанавливает состояние рутины. </summary>
        protected override void ResetStates()
        {
            foreach (var state in _nameToCharacterStates.Values) state.Reset();
            SetState(StateName.Routine);
        }

        /// <summary> Обрабатывает изменение точки расписания и переключает состояние на движение если необходимо. </summary>
        /// <param name="newPoint"> Новая точка расписания. </param>
        private void OnSchedulePointChanged(SchedulePoint newPoint)
        {
            if (_currentState is WaitingState) return;

            if (_currentState is MovementState)
                _currentState.Enter();
            else
                SetState(StateName.Movement);

            _animationController.TriggerAnimation(AnimationType.Locomotion);
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

        /// <summary> Вызывается при входе игрока в триггер NPC. Переводит NPC в состояние ожидания. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        public void OnTriggerEntered(Collider other)
        {
            SetState(StateName.Waiting);
            _animationController.TriggerAnimation(AnimationType.Idle);
        }

        /// <summary> Вызывается при выходе игрока из триггера NPC. Переводит NPC в состояние движения. </summary>
        /// <param name="other"> Коллайдер, вышедший из триггера. </param>
        public void OnTriggerExited(Collider other)
        {
            SetState(StateName.Movement);
            _animationController.TriggerAnimation(AnimationType.Locomotion);
        }
    }
}