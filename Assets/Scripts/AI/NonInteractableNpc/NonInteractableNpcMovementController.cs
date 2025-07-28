using FlavorfulStory.AI.BaseNpc;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Контроллер движения для неинтерактивного NPC с поддержкой точек расписания. </summary>
    public class NonInteractableNpcMovementController : NpcMovementController
    {
        /// <summary> Текущая точка расписания для перемещения. </summary>
        private Vector3 _currentPoint;

        /// <summary> Инициализирует новый экземпляр контроллера движения неинтерактивного NPC. </summary>
        /// <param name="navMeshAgent"> Агент навигационной сетки для перемещения. </param>
        /// <param name="transform"> Трансформ объекта NPC. </param>
        /// <param name="animationController"> Контроллер анимации для управления анимациями движения. </param>
        public NonInteractableNpcMovementController(NavMeshAgent navMeshAgent, Transform transform,
            NpcAnimationController animationController) : base(navMeshAgent, transform, animationController)
        {
            _currentPoint = Vector3.zero;

            _navigator.OnDestinationReached += () =>
            {
                OnDestinationReached?.Invoke();
                OnDestinationReached = null;
            };
        }

        /// <summary> Запускает перемещение к текущей установленной точке. </summary>
        public override void MoveToPoint() => _navigator.MoveTo(_currentPoint);

        /// <summary> Устанавливает новую целевую точку для перемещения агента. </summary>
        /// <param name="newPoint"> Новая позиция в мировых координатах. </param>
        public void SetPoint(Vector3 newPoint) => _currentPoint = newPoint;
    }
}