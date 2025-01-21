using System.Linq;
using FlavorfulStory.AI.Scheduling;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine
{
    public class RoutineState : FsmState
    {
        private Fsm _fsm;
        private NpcSchedule _npcSchedule;
        private Animator _animator;
        private SchedulePoint _currentPoint;
        
        public RoutineState(Fsm fsm, NpcSchedule npcSchedule, Animator animator) : base(fsm)
        {
            _fsm = fsm;
            _npcSchedule = npcSchedule;
            _animator = animator;
            _currentPoint = null;
        }
        
        public override void Enter()
        {
            WorldTime.OnDateTimeChanged += CheckNewTime;
            Debug.Log("Enter RoutineState");
        }
        
        public override void Exit()
        {
            WorldTime.OnDateTimeChanged -= CheckNewTime;
            _currentPoint = null;
            Debug.Log("Exit RoutineState");
        }

        public override void Update(float deltaTime)
        {
            if (_currentPoint != null && !IsPlayingAnimation(_currentPoint.AnimationClipName))
                _animator.Play(_currentPoint.AnimationClipName);
        }


        private void CheckNewTime(DateTime currentTime)
        {
            foreach (var pathPoint in _npcSchedule.Schedules[0].Path.Reverse())
            {
                if (currentTime.Hour > pathPoint.Hour && currentTime.Minute > pathPoint.Minutes)
                {
                    _currentPoint = pathPoint;
                    break;
                }
                
                if (currentTime.Hour == pathPoint.Hour && currentTime.Minute == pathPoint.Minutes)
                {
                    _fsm.SetState<MovementState>();
                    break;
                }
            }
        }
        
        private bool IsPlayingAnimation(string stateName)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                return true;
            return false;
        }
    }
}