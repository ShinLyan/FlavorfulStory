using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.AI.FSM.ShopStates
{
    /// <summary> Состояние анимации персонажа с таймером для управления завершением. </summary>
    public class AnimationState : CharacterState
    {
        /// <summary> Таймер для отслеживания времени выполнения анимации. </summary>
        private float _timer;

        /// <summary> Флаг, указывающий завершена ли анимация. </summary>
        private bool _isAnimationComplete;

        /// <summary> Флаг, указывающий находится ли игра в режиме паузы. </summary>
        private bool _isPaused;

        /// <summary> Контроллер анимаций NPC, используемый для управления анимациями. </summary>
        private readonly NpcAnimationController _npcAnimationController;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="npcAnimationController"></param>
        public AnimationState(NpcAnimationController npcAnimationController)
        {
            _npcAnimationController = npcAnimationController;

            WorldTime.OnTimePaused += () => _isPaused = true;
            WorldTime.OnTimeUnpaused += () => _isPaused = false;
        }

        /// <summary> Инициализирует состояние анимации при входе. </summary>
        public override void Enter()
        {
            base.Enter();
            _isAnimationComplete = false;

            if (Context == null) return;

            if (Context.TryGet<AnimationType>(ContextType.AnimationType, out var animationType))
                _npcAnimationController.TriggerAnimation(animationType);

            if (Context.TryGet(ContextType.AnimationTime, out float animationTime)) _timer = animationTime;
        }

        /// <summary> Обновляет состояние анимации каждый кадр, уменьшая таймер. </summary>
        public override void Update()
        {
            if (_isPaused) return;

            if (!_isAnimationComplete && _timer > 0)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0) _isAnimationComplete = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Exit() => _npcAnimationController.TriggerAnimation(AnimationType.Locomotion);

        /// <summary> Возвращает статус завершения анимации. </summary>
        /// <returns> true, если анимация завершена; иначе false. </returns>
        public override bool IsComplete() => _isAnimationComplete;
    }
}