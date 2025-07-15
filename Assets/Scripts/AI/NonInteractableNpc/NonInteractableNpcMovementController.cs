using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Контроллер движения для неинтерактивного NPC с поддержкой точек расписания. </summary>
    public class NonInteractableNpcMovementController : NpcMovementController
    {
        /// <summary> Навигатор для неинтерактивного NPC. </summary>
        private readonly NonInteractableNpcNavigator _nonInteractableNavigator;

        /// <summary> Текущая точка расписания для перемещения. </summary>
        private SchedulePoint _currentPoint;

        /// <summary> Инициализирует новый экземпляр контроллера движения неинтерактивного NPC. </summary>
        /// <param name="navMeshAgent"> Агент навигационной сетки для перемещения. </param>
        /// <param name="warpGraph"> Граф варп-порталов для телепортации между локациями. </param>
        /// <param name="transform"> Трансформ объекта NPC. </param>
        /// <param name="animationController"> Контроллер анимации для управления анимациями движения. </param>
        public NonInteractableNpcMovementController(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform,
            NpcAnimationController animationController)
            : base(navMeshAgent, warpGraph, transform, animationController)
        {
            _nonInteractableNavigator = (NonInteractableNpcNavigator)_navigator;

            _nonInteractableNavigator.OnDestinationReached += () =>
            {
                OnDestinationReached?.Invoke();
                OnDestinationReached = null;
            };
        }

        /// <summary> Создает навигатор для неинтерактивного NPC. </summary>
        /// <param name="agent"> Агент навигационной сетки. </param>
        /// <param name="graph"> Граф варп-порталов. </param>
        /// <param name="transform"> Трансформ объекта. </param>
        /// <returns> Экземпляр навигатора для неинтерактивного NPC. </returns>
        protected override INpcNavigator CreateNavigator(NavMeshAgent agent, WarpGraph graph, Transform transform)
        {
            return new NonInteractableNpcNavigator(agent, graph, transform);
        }

        /// <summary> Запускает перемещение к текущей установленной точке. </summary>
        public override void MoveToPoint() { _nonInteractableNavigator.MoveTo(_currentPoint); }

        // public void SetPoint(Vector3 newPoint) => _currentPoint = newPoint; //TODO: подумать
        /// <summary> Устанавливает новую точку расписания для перемещения. </summary>
        /// <param name="newPoint"> Новая точка расписания. </param>
        public void SetPoint(SchedulePoint newPoint) => _currentPoint = newPoint;
    }
}