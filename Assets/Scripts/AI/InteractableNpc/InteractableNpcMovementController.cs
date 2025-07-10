using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.InteractableNpc
{
    /// <summary> Контроллер движения для интерактивного NPC с поддержкой расписания. </summary>
    public class InteractableNpcMovementController : NpcMovementController
    {
        /// <summary> Обработчик расписания NPC. </summary>
        private readonly NpcScheduleHandler _scheduleHandler;

        /// <summary> Навигатор для интерактивного NPC с расширенной функциональностью. </summary>
        private readonly InteractableNpcNavigator _interactableNavigator;

        /// <summary> Инициализирует новый экземпляр контроллера движения для интерактивного NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="warpGraph"> Граф варп-точек для навигации. </param>
        /// <param name="transform"> Transform NPC. </param>
        /// <param name="animationController"> Контроллер анимации NPC. </param>
        /// <param name="scheduleHandler"> Обработчик расписания NPC. </param>
        public InteractableNpcMovementController(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform,
            NpcAnimationController animationController,
            NpcScheduleHandler scheduleHandler)
            : base(navMeshAgent, warpGraph, transform, animationController)
        {
            _scheduleHandler = scheduleHandler;
            _interactableNavigator = (InteractableNpcNavigator)_navigator;

            _interactableNavigator.OnDestinationReached += () =>
            {
                Debug.Log("Destination reached!");
                OnDestinationReached?.Invoke();
            };
            _scheduleHandler.OnSchedulePointChanged += _interactableNavigator.OnSchedulePointChanged;
        }

        /// <summary> Создает специализированный навигатор для интерактивного NPC. </summary>
        /// <param name="agent"> NavMeshAgent для навигации. </param>
        /// <param name="graph"> Граф варп-точек. </param>
        /// <param name="transform"> Transform NPC. </param>
        /// <returns> Новый экземпляр InteractableNpcNavigator. </returns>
        protected override INpcNavigator CreateNavigator(NavMeshAgent agent, WarpGraph graph, Transform transform)
        {
            return new InteractableNpcNavigator(agent, graph, transform);
        }

        /// <summary> Перемещает NPC к текущей точке расписания. </summary>
        /// <remarks> Если текущая точка расписания существует, инициирует движение к ней. </remarks>
        public override void MoveToPoint()
        {
            if (_scheduleHandler.CurrentPoint != null) _interactableNavigator.MoveTo(_scheduleHandler.CurrentPoint);
        }
    }
}