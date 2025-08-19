using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> Контроллер движения для интерактивного NPC с поддержкой расписания. </summary>
    public class InteractableNpcMovementController : NpcMovementController
    {
        /// <summary> Обработчик расписания NPC. </summary>
        private readonly NpcScheduleHandler _scheduleHandler;

        /// <summary> Инициализирует новый экземпляр контроллера движения для интерактивного NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="transform"> Transform NPC. </param>
        /// <param name="animationController"> Контроллер анимации NPC. </param>
        /// <param name="scheduleHandler"> Обработчик расписания NPC. </param>
        public InteractableNpcMovementController(NavMeshAgent navMeshAgent, Transform transform,
            NpcAnimationController animationController, NpcScheduleHandler scheduleHandler)
            : base(navMeshAgent, transform, animationController) => _scheduleHandler = scheduleHandler;

        /// <summary> Инициализация объекта. </summary>
        public override void Initialize()
        {
            _scheduleHandler.OnSchedulePointChanged += OnSchedulePointChanged;
            _navigator.OnDestinationReached += HandleDestinationReached;
        }

        /// <summary> Освобождает ресурсы при уничтожении объекта. </summary>
        public override void Dispose()
        {
            _scheduleHandler.OnSchedulePointChanged -= OnSchedulePointChanged;
            _navigator.OnDestinationReached -= HandleDestinationReached;
        }

        /// <summary> Обработка события, вызываемое при достижении пункта назначения. </summary>
        private void HandleDestinationReached() => OnDestinationReached?.Invoke();

        /// <summary> Перемещает NPC к текущей точке расписания. </summary>
        /// <remarks> Если текущая точка расписания существует, инициирует движение к ней. </remarks>
        public override void MoveToPoint()
        {
            var point = _scheduleHandler.CurrentPoint;
            if (point == null) return;

            var destination = new NpcDestinationPoint(point.Position, Quaternion.Euler(point.Rotation));
            _navigator.MoveTo(destination);
        }

        /// <summary> Обрабатывает изменение точки расписания. </summary>
        /// <param name="point"> Новая точка расписания. </param>
        /// <remarks> Если NPC не находится в состоянии покоя, останавливает текущее движение
        /// и начинает движение к новой точке. </remarks>
        private void OnSchedulePointChanged(NpcSchedulePoint point)
        {
            if (!_navigator.IsMoving) return;

            Stop();
            var destination = new NpcDestinationPoint(point.Position, Quaternion.Euler(point.Rotation));
            _navigator.MoveTo(destination);
        }
    }
}