using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> Навигатор для интерактивного NPC с поддержкой перемещения к точкам расписания. </summary>
    public class InteractableNpcNavigator : NpcNavigator, INpcNavigatorMover<SchedulePoint>
    {
        /// <summary> Инициализирует новый экземпляр навигатора для интерактивного NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="warpGraph"> Граф варп-точек для навигации между локациями. </param>
        /// <param name="transform"> Transform NPC. </param>
        public InteractableNpcNavigator(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform)
            : base(navMeshAgent, warpGraph, transform)
        {
        }

        /// <summary> Перемещает NPC к указанной точке расписания. </summary>
        /// <param name="point"> Целевая точка расписания для перемещения. </param>
        /// <remarks> Если точка находится в другой локации, инициирует варп-переход,
        /// иначе использует обычную навигацию. </remarks>
        public void MoveTo(SchedulePoint point)
        {
            _currentTargetPoint = point;
            _isNotMoving = false;
            ResumeAgent();

            if (_currentLocation != point.LocationName)
                StartWarpTransition(point);
            else
                _navMeshAgent.SetDestination(point.Position);
        }

        /// <summary> Обрабатывает изменение точки расписания. </summary>
        /// <param name="point"> Новая точка расписания. </param>
        /// <remarks> Если NPC не находится в состоянии покоя, останавливает текущее движение
        /// и начинает движение к новой точке. </remarks>
        public void OnSchedulePointChanged(SchedulePoint point)
        {
            if (_isNotMoving) return;

            Stop();
            MoveTo(point);
        }
    }
}