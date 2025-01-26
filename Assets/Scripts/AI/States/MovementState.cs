using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.AI.States;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    /// <summary> Состояние движения NPC, в котором персонаж перемещается к заданной точке. </summary>
    public class MovementState : CharacterState
    {
        /// <summary> Минимальное расстояние до точки, при котором считается, что NPC достиг её. </summary>
        private const float DistanceToReachPoint = 0.3f;

        /// <summary> Компонент для навигации NPC по NavMesh. </summary>
        private NavMeshAgent _navMeshAgent;

        /// <summary> Компонент аниматора, управляющий анимациями NPC. </summary>
        private Animator _animator;

        /// <summary> Расписание NPC, определяющее его маршруты и точки перемещения. </summary>
        private NpcSchedule _npcSchedule;

        /// <summary> Контроллер состояний, управляющий переключением между состояниями NPC. </summary>
        private StateController _stateController;

        /// <summary> Контроллер NPC, управляющий его поведением и анимациями. </summary>
        private NpcController _npcController;

        /// <summary> Текущая точка маршрута, к которой движется NPC. </summary>
        private SchedulePoint _currentPoint;

        /// <summary> Текущая скорость NPC, используемая для анимации. </summary>
        private float _speed;

        /// <summary> Инициализирует новое состояние движения. </summary>
        /// <param name="stateController"> Контроллер состояний. </param>
        /// <param name="npcSchedule"> Расписание NPC. </param>
        /// <param name="navMeshAgent"> Компонент для навигации по NavMesh. </param>
        /// <param name="npcController"> Контроллер NPC. </param>
        public MovementState(StateController stateController, NpcSchedule npcSchedule,
            NavMeshAgent navMeshAgent, NpcController npcController) : base(stateController)
        {
            _stateController = stateController;
            _npcSchedule = npcSchedule;
            _navMeshAgent = navMeshAgent;
            _npcController = npcController;
            _currentPoint = null;
        }

        /// <summary> Вызывается при входе в состояние движения. </summary>
        public override void Enter()
        {
            WorldTime.OnDateTimeChanged += FindDestinationPoint;
        }

        /// <summary> Вызывается при выходе из состояния движения. </summary>
        public override void Exit()
        {
            WorldTime.OnDateTimeChanged -= FindDestinationPoint;
            _npcController.PlayMoveAnimation(0f, 0f);
            _currentPoint = null;
        }

        /// <summary> Обновляет логику состояния движения каждый кадр. </summary>
        /// <param name="deltaTime"> Время, прошедшее с последнего кадра. </param>
        public override void Update(float deltaTime)
        {
            _speed = Mathf.Clamp01(_navMeshAgent.velocity.magnitude);
            _npcController.PlayMoveAnimation(_speed);
            SwitchStateIfPointReached();
        }

        /// <summary> Проверяет, достиг ли NPC текущей точки, и переключает состояние, если это так. </summary>
        private void SwitchStateIfPointReached()
        {
            if (_navMeshAgent.hasPath && _navMeshAgent.remainingDistance <= DistanceToReachPoint)
                _stateController.SetState<RoutineState>();
        }

        /// <summary> Находит ближайшую точку маршрута на основе текущего времени
        /// и задаёт её как цель для NPC. </summary>
        /// <param name="currentTime"> Текущее время в игре. </param>
        private void FindDestinationPoint(DateTime currentTime)
        {
            SchedulePoint closestPoint = _npcSchedule.Schedules[0].GetClosestSchedulePointInPath(currentTime);

            if (closestPoint != null && _currentPoint != closestPoint)
            {
                _navMeshAgent.SetDestination(closestPoint.Position);
                _currentPoint = closestPoint;
            }
        }
    }
}