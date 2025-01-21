using System.Linq;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    public class MovementState : FsmState
    {
        private UnityEngine.AI.NavMeshAgent _navMeshAgent;
        private readonly float _distanceToReachTarget = 0.1f;
        private Animator _animator;
        private NpcSchedule _npcSchedule;
        private Fsm _fsm;
        private SchedulePoint _currentPoint;
        
        /// <summary> Хэшированное значение параметра "скорость" для анимации. </summary>
        private static readonly int _speedParameterHash = Animator.StringToHash("Speed");

        public MovementState(Fsm fsm, UnityEngine.AI.NavMeshAgent navMeshAgent, NpcSchedule npcSchedule, Animator animator) : base(fsm)
        {
            _fsm = fsm;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _npcSchedule = npcSchedule;
            _currentPoint = null;
        }


        public override void Enter()
        {
            WorldTime.OnDateTimeChanged += FindDestinationPoint;
            Debug.Log("Enter MovementState");
        }

        public override void Exit()
        {
            WorldTime.OnDateTimeChanged -= FindDestinationPoint;
            _animator.SetFloat(_speedParameterHash, 0.0f, 0f, Time.deltaTime);
            _currentPoint = null;
            Debug.Log("Exit MovementState");
        }

        public override void Update(float deltaTime)
        {
            DoAnimation();
            
            if (_navMeshAgent.hasPath && _navMeshAgent.remainingDistance <= _distanceToReachTarget)
            {
                _fsm.SetState<RoutineState>();
            }
        }

        private void FindDestinationPoint(DateTime currentTime)
        {
            foreach (var pathPoint in _npcSchedule.Schedules[0].Path.Reverse())
            {
                if (pathPoint == _currentPoint)
                    break;
                
                if (currentTime.Hour >= pathPoint.Hour && currentTime.Minute >= pathPoint.Minutes)
                {
                    _navMeshAgent.SetDestination(pathPoint.Position);
                    _currentPoint = pathPoint;
                    break;
                }
            }
        }

        private void DoAnimation()
        {
            float speed = Mathf.Clamp01(_navMeshAgent.velocity.magnitude);
            const float DampTime = 0.2f;
            _animator.SetFloat(_speedParameterHash, speed, DampTime, Time.deltaTime);
        }
    }
}