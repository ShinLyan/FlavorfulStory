using FlavorfulStory.AI.FiniteStateMachine;
using FlavorfulStory.AI.Scheduling;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private NpcSchedule _npcSchedule;
        
        private Animator _animator;
        private Animation _animation;
        
        private NavMeshAgent _navMeshAgent;
        
        private InteractionState _interactionState;
        private MovementState _movementState;
        private RoutineState _routineState;
        private WaitingState _waitingState;

        private Fsm _fsm;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _animation = GetComponent<Animation>();
        }

        private void Start()
        {
            _fsm = new Fsm();

            _interactionState = new InteractionState(_fsm, this);
            _movementState = new MovementState(_fsm, _navMeshAgent, _npcSchedule, _animator);
            _routineState = new RoutineState(_fsm, _npcSchedule, _animator);
            _waitingState = new WaitingState(_fsm, this);
            
            _fsm.AddState(_interactionState);
            _fsm.AddState(_movementState);
            _fsm.AddState(_routineState);
            _fsm.AddState(_waitingState);
            
            _fsm.SetState<RoutineState>();
        }

        private void Update()
        {
            _fsm.Update(Time.deltaTime);
        }
        
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player"))
            {
                _navMeshAgent.isStopped = true;
                _fsm.SetState<WaitingState>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _navMeshAgent.isStopped = false;
                _fsm.SetState<MovementState>();
            }
        }
    }
}