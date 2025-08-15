using System;
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
        private readonly NavMeshAgent _agent;

        /// <summary> Навигатор для управления перемещением NPC. </summary>
        protected readonly NpcNavigator _navigator;

        /// <summary> Событие, вызываемое при достижении пункта назначения. </summary>
        public Action OnDestinationReached;

        /// <summary> Инициализирует новый экземпляр контроллера движения NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="transform"> Transform NPC. </param>
        /// <param name="animationController"> Контроллер анимации NPC. </param>
        protected NpcMovementController(NavMeshAgent navMeshAgent, Transform transform,
            NpcAnimationController animationController)
        {
            _agent = navMeshAgent;
            _animationController = animationController;
            _navigator = new NpcNavigator(_agent, transform);
            _navigator.OnDestinationReached += () => OnDestinationReached?.Invoke();
        }

        /// <summary> Обновляет движение NPC каждый кадр.
        /// Рассчитывает скорость анимации на основе скорости NavMeshAgent и обновляет навигатор. </summary>
        public void Update()
        {
            float speed = Mathf.Clamp01(_agent.velocity.magnitude) * 0.5f;
            _animationController.SetSpeed(speed);
            _navigator.Update();
        }

        /// <summary> Останавливает движение NPC. </summary>
        /// <param name="warp"> Если true, NPC мгновенно телепортируется в пункт назначения. </param>
        public void Stop(bool warp = false) => _navigator.Stop(warp);

        /// <summary> Перемещает NPC к указанной точке. </summary>
        /// <remarks> Должен быть реализован в наследниках. </remarks>
        public abstract void MoveToPoint();
    }
}