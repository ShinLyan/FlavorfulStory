using System;
using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI
{
    /// <summary> Контроллер движения NPC, который управляет навигацией, анимацией и расписанием персонажа.
    /// Реализует интерфейс IScheduleDependable для работы с системой расписания. </summary>
    public class NpcMovementController : IScheduleDependable
    {
        /// <summary> Навигатор для обработки движения NPC по warp-графу. </summary>
        private readonly NpcNavigator _navigator;

        /// <summary> Обработчик расписания NPC.  </summary>
        private readonly NpcScheduleHandler _scheduleHandler;

        /// <summary> Контроллер анимации NPC. </summary>
        private readonly NpcAnimatorController _animatorController;

        /// <summary> Unity NavMeshAgent для навигации по NavMesh. </summary>
        private readonly NavMeshAgent _agent;

        /// <summary> Событие, вызываемое при достижении пункта назначения. </summary>
        public Action OnDestinationReached;

        /// <summary> Инициализирует новый экземпляр контроллера движения NPC. </summary>
        /// <param name="navMeshAgent"> NavMeshAgent для навигации. </param>
        /// <param name="warpGraph"> Граф варп-точек для навигации. </param>
        /// <param name="transform"> Transform NPC. </param>
        /// <param name="coroutineRunner"> MonoBehaviour для запуска корутин. </param>
        /// <param name="animatorController"> Контроллер анимации NPC. </param>
        /// <param name="scheduleHandler"> Обработчик расписания NPC. </param>
        public NpcMovementController(NavMeshAgent navMeshAgent,
            WarpGraph warpGraph,
            Transform transform,
            MonoBehaviour coroutineRunner,
            NpcAnimatorController animatorController,
            NpcScheduleHandler scheduleHandler)
        {
            _agent = navMeshAgent;
            _animatorController = animatorController;
            _scheduleHandler = scheduleHandler;

            _navigator = new NpcNavigator(_agent, warpGraph, transform, coroutineRunner);
            _navigator.OnDestinationReached += () => OnDestinationReached?.Invoke();

            _scheduleHandler.OnSchedulePointChanged += _navigator.OnSchedulePointChanged;

            WorldTime.OnTimePaused += () => Stop();
            WorldTime.OnTimeUnpaused += MoveToCurrentPoint;
        }

        /// <summary> Обновляет движение NPC каждый кадр.
        /// Рассчитывает скорость анимации на основе скорости NavMeshAgent и обновляет навигатор. </summary>
        public void UpdateMovement()
        {
            float speed = Mathf.Clamp01(_agent.velocity.magnitude) * 0.5f;
            _animatorController.SetSpeed(speed);
            _navigator.Update();
        }

        /// <summary> Перемещает NPC к текущей точке расписания.
        /// Если текущая точка расписания не задана, движение не выполняется. </summary>
        public void MoveToCurrentPoint()
        {
            if (_scheduleHandler.CurrentPoint != null) _navigator.MoveTo(_scheduleHandler.CurrentPoint);
        }

        /// <summary> Останавливает движение NPC. </summary>
        /// <param name="warp"> Если true, NPC мгновенно телепортируется в пункт назначения. </param>
        public void Stop(bool warp = false) => _navigator.Stop(warp);

        /// <summary> Устанавливает параметры текущего расписания для NPC. </summary>
        /// <param name="scheduleParams"> Параметры расписания для установки. </param>
        public void SetCurrentScheduleParams(ScheduleParams scheduleParams) =>
            _scheduleHandler.SetCurrentScheduleParams(scheduleParams);
    }
}