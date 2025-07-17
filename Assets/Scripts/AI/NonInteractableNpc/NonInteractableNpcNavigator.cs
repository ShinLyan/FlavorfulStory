using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.NonInteractableNpc
{
    /// <summary> Навигатор для неинтерактивного NPC с поддержкой перемещения к точкам расписания. </summary>
    public class NonInteractableNpcNavigator : NpcNavigator, INpcNavigatorMover<SchedulePoint>
    {
        /// <summary> Инициализирует новый экземпляр навигатора неинтерактивного NPC. </summary>
        /// <param name="navMeshAgent"> Агент навигационной сетки для перемещения. </param>
        /// <param name="warpGraph"> Граф варп-порталов для телепортации между локациями. </param>
        /// <param name="transform"> Трансформ объекта NPC. </param>
        public NonInteractableNpcNavigator(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform)
            : base(navMeshAgent, warpGraph, transform)
        {
        }

        /// <summary> Запускает перемещение к указанной точке расписания. </summary>
        /// <param name="point"> Точка расписания для перемещения. </param>
        public void MoveTo(SchedulePoint point) //TODO: реализовать через Vector3 после удаления WarpGraph
        {
            _currentTargetPoint = point;
            _isNotMoving = false;
            ResumeAgent();

            if (_currentLocation != point.LocationName)
                StartWarpTransition(point);
            else
                _navMeshAgent.SetDestination(point.Position);
        }
    }
}