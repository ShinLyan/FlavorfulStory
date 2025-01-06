using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    public class MovementState : FsmState
    {
        private NPC _npc;
        private UnityEngine.AI.NavMeshAgent _navMeshAgent;
        private Transform[] _goals;
        private readonly float _distanceToChangeGoal = (float)0.1;
        private int _currentGoal;
        private Animator _animator;

        public MovementState(Fsm fsm, NPC npc, UnityEngine.AI.NavMeshAgent navMeshAgent, Transform[] goals, Animator animator) : base(fsm)
        {
            _npc = npc;
            _navMeshAgent = navMeshAgent;
            _goals = goals;
            _animator = animator;
        }

        public override void Enter()
        {
            _currentGoal = 0;
        }

        public override void Exit()
        {
            _navMeshAgent.ResetPath();
        }

        public override void Update(float deltaTime)
        {
            if (_navMeshAgent.remainingDistance < _distanceToChangeGoal)
            {
                _currentGoal++;
                if (_currentGoal == _goals.Length) 
                    _currentGoal = 0;
            }
            _navMeshAgent.destination = _goals[_currentGoal].position;
            DoAnimation();
        }

        private void DoAnimation()
        {
            float speed = Mathf.Clamp01(_navMeshAgent.velocity.magnitude);
            const float DampTime = 0.2f;
            _animator.SetFloat("Speed", speed, DampTime, Time.deltaTime);
        }
    }
}