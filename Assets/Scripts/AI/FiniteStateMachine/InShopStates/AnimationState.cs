using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    public class AnimationState : CharacterState
    {
        private float _timer;
        private bool _isAnimationComplete;

        public override void Enter()
        {
            base.Enter();
            _timer = 3f;
            _isAnimationComplete = false;
        }

        public override void Update()
        {
            if (!_isAnimationComplete && _timer > 0)
            {
                _timer -= Time.deltaTime; // Уменьшаем таймер каждый кадр

                if (_timer <= 0) _isAnimationComplete = true; // По истечении времени отмечаем завершение
            }
        }

        public override bool IsComplete() => _isAnimationComplete;
    }
}