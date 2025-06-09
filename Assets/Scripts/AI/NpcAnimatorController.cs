using UnityEngine;

namespace FlavorfulStory.AI
{
    public class NpcAnimatorController
    {
        private readonly Animator _animator;
        private readonly int _speedHash = Animator.StringToHash("Speed");

        public NpcAnimatorController(Animator animator) { _animator = animator; }

        public void SetSpeed(float speed, float dampTime = 0.2f)
        {
            _animator.SetFloat(_speedHash, speed, dampTime, Time.deltaTime);
        }
    }
}