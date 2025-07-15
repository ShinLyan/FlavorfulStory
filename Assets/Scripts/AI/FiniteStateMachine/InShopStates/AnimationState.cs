using UnityEngine;

namespace FlavorfulStory.AI.FiniteStateMachine.InShopStates
{
    /// <summary> Состояние анимации персонажа с таймером для управления завершением. </summary>
    public class AnimationState : CharacterState
    {
        /// <summary> Таймер для отслеживания времени выполнения анимации. </summary>
        private float _timer;

        /// <summary> Флаг, указывающий завершена ли анимация. </summary>
        private bool _isAnimationComplete;

        /// <summary> Инициализирует состояние анимации при входе. </summary>
        public override void Enter()
        {
            base.Enter();
            _timer = 3f;
            _isAnimationComplete = false;
        }

        /// <summary> Обновляет состояние анимации каждый кадр, уменьшая таймер. </summary>
        public override void Update()
        {
            if (!_isAnimationComplete && _timer > 0)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0) _isAnimationComplete = true;
            }
        }

        /// <summary> Возвращает статус завершения анимации. </summary>
        /// <returns> true, если анимация завершена; иначе false. </returns>
        public override bool IsComplete() => _isAnimationComplete;
    }
}