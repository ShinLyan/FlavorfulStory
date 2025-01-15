using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    public class RoutineState : FsmState
    {
        private NPC _npc;
        public RoutineState(Fsm fsm, NPC npc) : base(fsm)
        {
            _npc = npc;
        }
        
        public override void Enter()
        {
            Debug.Log($"[{GetType().Name}] Enter.");
        }
        
        public override void Exit()
        {
            Debug.Log($"[{GetType().Name}] Exit.");
        }

        public override void Update(float deltaTime)
        {
            Debug.Log($"[{GetType().Name}] Update.");
        }
    }
}