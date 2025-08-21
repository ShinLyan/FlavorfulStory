using System;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Контроллер движения NPC, который управляет навигацией, анимацией и расписанием персонажа.
    /// Реализует интерфейс IScheduleDependable для работы с системой расписания. </summary>
    public class NpcMovementController : IInitializable, IDisposable
    {
        /// <summary> Unity NavMeshAgent для навигации по NavMesh. </summary>
        private readonly NavMeshAgent _agent;

        /// <summary> Навигатор для управления перемещением NPC. </summary>
        private readonly NpcNavigator _navigator;

        /// <summary> Контроллер анимации NPC. </summary>
        private readonly NpcAnimationController _animationController;

        /// <summary> Событие, вызываемое при достижении пункта назначения. </summary>
        public Action OnDestinationReached;

        /// <summary> Инициализирует новый экземпляр контроллера движения NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="transform"> Transform NPC. </param>
        /// <param name="animationController"> Контроллер анимации NPC. </param>
        public NpcMovementController(NavMeshAgent navMeshAgent, Transform transform,
            NpcAnimationController animationController)
        {
            _agent = navMeshAgent;
            _navigator = new NpcNavigator(_agent, transform);
            _animationController = animationController;
        }

        /// <summary> Инициализация объекта. </summary>
        public void Initialize() => _navigator.OnDestinationReached += HandleDestinationReached;

        /// <summary> Освобождает ресурсы при уничтожении объекта. </summary>
        public void Dispose() => _navigator.OnDestinationReached -= HandleDestinationReached;

        /// <summary> Обновляет движение NPC каждый кадр.
        /// Рассчитывает скорость анимации на основе скорости NavMeshAgent и обновляет навигатор. </summary>
        public void Update()
        {
            float speed = Mathf.Clamp01(_agent.velocity.magnitude) * 0.5f;
            _animationController.SetSpeed(speed);
            _navigator.Update();
        }

        /// <summary> Обработка события, вызываемое при достижении пункта назначения. </summary>
        private void HandleDestinationReached()
        {
            OnDestinationReached?.Invoke();
            OnDestinationReached = null;
        }

        /// <summary> Останавливает движение NPC. </summary>
        /// <param name="warp"> Если true, NPC мгновенно телепортируется в пункт назначения. </param>
        public void Stop(bool warp = false) => _navigator.Stop(warp);

        /// <summary> Перемещает NPC к указанной точке. </summary>
        public void MoveToPoint(NpcDestinationPoint destinationPoint) => _navigator.MoveTo(destinationPoint);
    }
}