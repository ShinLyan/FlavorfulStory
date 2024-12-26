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

        public MovementState(Fsm fsm, NPC npc, UnityEngine.AI.NavMeshAgent navMeshAgent, Transform[] goals) : base(fsm)
        {
            _npc = npc;
            _navMeshAgent = navMeshAgent;
            _goals = goals;
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
                if (_currentGoal == _goals.Length - 1) 
                    _currentGoal = 0;
            }
            Debug.Log(_currentGoal);
            _navMeshAgent.destination = _goals[_currentGoal].position;
        }
    }
}