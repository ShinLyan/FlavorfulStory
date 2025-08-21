using FlavorfulStory.AI.BaseNpc;
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
        public override void Initialize() => _navigator.OnDestinationReached += HandleDestinationReached;

        /// <summary> Освобождает ресурсы при уничтожении объекта. </summary>
        public override void Dispose() => _navigator.OnDestinationReached -= HandleDestinationReached;

        /// <summary> Обработка события, вызываемое при достижении пункта назначения. </summary>
        private void HandleDestinationReached() => OnDestinationReached?.Invoke();

        /// <summary> Перемещает NPC к текущей точке расписания. </summary>
        /// <remarks> Если текущая точка расписания существует, инициирует движение к ней. </remarks>
        public override void MoveToPoint(NpcDestinationPoint destinationPoint) => _navigator.MoveTo(destinationPoint);
    }
}