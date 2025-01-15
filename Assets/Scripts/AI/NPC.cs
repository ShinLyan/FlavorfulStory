using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.AI.FiniteStateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.AI
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private Transform[] _goals;

        [SerializeField] private Dictionary<int, Transform> _shedule;
        
        private Animator _animator;
        
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
        }

        private void Start()
        {
            _fsm = new Fsm();

            _interactionState = new InteractionState(_fsm, this);
            _movementState = new MovementState(_fsm, this, _navMeshAgent, _goals, _animator);
            _routineState = new RoutineState(_fsm, this);
            _waitingState = new WaitingState(_fsm, this);
            
            _fsm.AddState(_interactionState);
            _fsm.AddState(_movementState);
            _fsm.AddState(_routineState);
            _fsm.AddState(_waitingState);
            
            _fsm.SetState<MovementState>();
        }

        private void Update()
        {
            _fsm.Update(Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player"))
                _fsm.SetState<WaitingState>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                StartCoroutine(SetStateAfterTime(3));
        }

        IEnumerator SetStateAfterTime(int sleepTime)
        {
            yield return new WaitForSeconds(sleepTime);
            _fsm.SetState<MovementState>();
        }
    }
    
}