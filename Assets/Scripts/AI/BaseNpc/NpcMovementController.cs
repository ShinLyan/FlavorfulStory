using System;
using FlavorfulStory.AI.WarpGraphSystem;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Контроллер движения NPC, который управляет навигацией, анимацией и расписанием персонажа.
    /// Реализует интерфейс IScheduleDependable для работы с системой расписания. </summary>
    public abstract class NpcMovementController
    {
        /// <summary> Контроллер анимации NPC. </summary>
        private readonly NpcAnimationController _animationController;

        /// <summary> Unity NavMeshAgent для навигации по NavMesh. </summary>
        protected readonly NavMeshAgent _agent;

        /// <summary> Событие, вызываемое при достижении пункта назначения. </summary>
        public Action OnDestinationReached;

        protected readonly INpcNavigator _navigator;

        /// <summary> Инициализирует новый экземпляр контроллера движения NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="warpGraph"> Граф варп-точек для навигации. </param>
        /// <param name="transform"> Transform NPC. </param>
        /// <param name="animationController"> Контроллер анимации NPC. </param>
        public NpcMovementController(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform,
            NpcAnimationController animationController)
        {
            _agent = navMeshAgent;
            _animationController = animationController;
            _navigator = CreateNavigator(_agent, warpGraph, transform);
        }

        /// <summary>
        /// Фабричный метод для создания навигатора — может быть переопределён в потомке.
        /// </summary>
        protected virtual INpcNavigator CreateNavigator(NavMeshAgent agent, WarpGraph graph, Transform transform)
        {
            return new NpcNavigator(agent, graph, transform);
        }

        /// <summary> Обновляет движение NPC каждый кадр.
        /// Рассчитывает скорость анимации на основе скорости NavMeshAgent и обновляет навигатор. </summary>
        public void UpdateMovement()
        {
            float speed = Mathf.Clamp01(_agent.velocity.magnitude) * 0.5f;
            _animationController.SetSpeed(speed);
            _navigator.Update();
        }

        public abstract void MoveToPoint();


        /// <summary> Останавливает движение NPC. </summary>
        /// <param name="warp"> Если true, NPC мгновенно телепортируется в пункт назначения. </param>
        public void Stop(bool warp = false) => _navigator.Stop(warp);
    }
}