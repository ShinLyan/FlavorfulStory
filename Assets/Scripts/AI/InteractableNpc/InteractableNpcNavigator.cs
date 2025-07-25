using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.Scheduling;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> Навигатор для интерактивного NPC с поддержкой перемещения к точкам расписания. </summary>
    public class InteractableNpcNavigator : NpcNavigator
    {
        /// <summary> Инициализирует новый экземпляр навигатора для интерактивного NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="transform"> Transform NPC. </param>
        public InteractableNpcNavigator(NavMeshAgent navMeshAgent, Transform transform)
            : base(navMeshAgent, transform)
        {
        }

        /// <summary> Обрабатывает изменение точки расписания. </summary>
        /// <param name="point"> Новая точка расписания. </param>
        /// <remarks> Если NPC не находится в состоянии покоя, останавливает текущее движение
        /// и начинает движение к новой точке. </remarks>
        public void OnSchedulePointChanged(SchedulePoint point)
        {
            if (_isNotMoving) return;

            Stop();
            MoveTo(point.Position);
        }
    }
}