using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.FSM;
using FlavorfulStory.AI.FSM.InteractableStates;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using DateTime = FlavorfulStory.TimeManagement.DateTime;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> Контроллер состояний конечного автомата NPC,
    /// управляющий переходами между различными состояниями персонажа. </summary>
    public sealed class InteractableNpcStateController : NpcStateController, ICharacterCollisionHandler
    {
        /// <summary> Отсортированные параметры расписания для быстрого поиска подходящего. </summary>
        private readonly IEnumerable<NpcScheduleParams> _sortedScheduleParams;

        /// <summary> Обработчик расписания NPC </summary>
        private readonly NpcScheduleHandler _scheduleHandler;

        /// <summary> Контроллер движения для интерактивного NPC. </summary>
        private readonly InteractableNpcMovementController _npcMovementController;

        /// <summary> Контроллер игрока для взаимодействия. </summary>
        private readonly IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Событие, вызываемое при изменении текущих параметров расписания. </summary>
        private event Action<NpcScheduleParams> OnCurrentScheduleParamsChanged;

        /// <summary> Инициализирует новый экземпляр контроллера состояний. </summary>
        /// <param name="npcSchedule"> Расписание NPC. </param>
        /// <param name="npcMovementController"> Контроллер движения NPC. </param>
        /// <param name="npcAnimationController"> Контроллер анимации NPC. </param>
        /// <param name="scheduleHandler"> Обработчик расписания NPC. </param>
        /// <param name="npcTransform"> Transform NPC для определения позиции. </param>
        /// <param name="playerPositionProvider"> Контроллер игрока. </param>
        public InteractableNpcStateController(NpcSchedule npcSchedule,
            InteractableNpcMovementController npcMovementController, NpcAnimationController npcAnimationController,
            NpcScheduleHandler scheduleHandler, Transform npcTransform, IPlayerPositionProvider playerPositionProvider)
            : base(npcAnimationController, npcTransform)
        {
            _sortedScheduleParams = npcSchedule.GetSortedScheduleParams();
            _scheduleHandler = scheduleHandler;
            _npcMovementController = npcMovementController;
            _playerPositionProvider = playerPositionProvider;
        }

        /// <summary> Инициализация объекта. </summary>
        public override void Initialize()
        {
            base.Initialize();
            InitializeStates();

            _scheduleHandler.OnSchedulePointChanged += OnSchedulePointChanged;
            OnCurrentScheduleParamsChanged += _scheduleHandler.SetCurrentScheduleParams;
            OnReset(WorldTime.CurrentGameTime);
        }

        /// <summary> Освобождает ресурсы при уничтожении объекта. </summary>
        public override void Dispose()
        {
            base.Dispose();
            _scheduleHandler.OnSchedulePointChanged -= OnSchedulePointChanged;
            OnCurrentScheduleParamsChanged -= _scheduleHandler.SetCurrentScheduleParams;
        }

        /// <summary> Инициализирует все состояния для интерактивного NPC. </summary>
        /// <remarks> Создает состояния взаимодействия, движения, рутины и ожидания, настраивает связи между ними. </remarks>
        private void InitializeStates()
        {
            _nameToCharacterStates.Add(NpcStateName.Movement, new MovementState(_npcMovementController, true));
            _nameToCharacterStates.Add(NpcStateName.Routine, new RoutineState(_animationController));
            _nameToCharacterStates.Add(NpcStateName.Waiting, new WaitingState(_playerPositionProvider, _npcTransform));

            foreach (var state in _nameToCharacterStates.Values)
            {
                state.OnStateChangeRequested += SetState;

                if (state is ICurrentSchedulePointDependable dependable)
                    _scheduleHandler.OnSchedulePointChanged += dependable.SetNewCurrentPoint;
            }
        }

        /// <summary> Действия при сбросе контроллера. </summary>
        /// <param name="currentTime"> Текущее время. </param>
        protected override void OnReset(DateTime currentTime)
        {
            PrioritizeSchedule(currentTime);
            ResetStates();
        }

        /// <summary> Сбрасывает все состояния и устанавливает состояние рутины. </summary>
        private void ResetStates()
        {
            foreach (var state in _nameToCharacterStates.Values) state.Reset();
            SetState(NpcStateName.Routine);
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

        /// <summary> Обрабатывает изменение точки расписания и переключает состояние на движение если необходимо. </summary>
        /// <param name="newPoint"> Новая точка расписания. </param>
        private void OnSchedulePointChanged(NpcSchedulePoint newPoint)
        {
            switch (CurrentState)
            {
                case WaitingState:
                    return;
                case MovementState:
                    CurrentState.Enter();
                    break;
                default:
                    SetState(NpcStateName.Movement);
                    break;
            }

            _animationController.TriggerAnimation(AnimationType.Locomotion);
        }

        #region ICharacterCollisionHandler

        /// <summary> Вызывается при входе игрока в триггер NPC. Переводит NPC в состояние ожидания. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        public void OnTriggerEntered(Collider other)
        {
            SetState(NpcStateName.Waiting);
            _animationController.TriggerAnimation(AnimationType.Idle);
        }

        /// <summary> Вызывается при выходе игрока из триггера NPC. Переводит NPC в состояние движения. </summary>
        /// <param name="other"> Коллайдер, вышедший из триггера. </param>
        public void OnTriggerExited(Collider other)
        {
            SetState(NpcStateName.Movement);
            _animationController.TriggerAnimation(AnimationType.Locomotion);
        }

        #endregion
    }
}