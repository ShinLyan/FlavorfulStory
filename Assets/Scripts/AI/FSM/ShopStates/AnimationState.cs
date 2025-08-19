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

        /// <summary> Флаг завершения анимации. </summary>
        private bool _isAnimationComplete;

        /// <summary> Флаг паузы игры. </summary>
        private bool _isPaused;

        /// <summary> Контроллер анимаций NPC. </summary>
        private readonly NpcAnimationController _npcAnimationController;

        /// <summary> Инициализирует состояние анимации. </summary>
        /// <param name="npcAnimationController"> Контроллер анимаций NPC. </param>
        public AnimationState(NpcAnimationController npcAnimationController) =>
            _npcAnimationController = npcAnimationController;

        /// <summary> Выполняется при входе в состояние. </summary>
        public override void Enter()
        {
            WorldTime.OnTimePaused += HandlePause;
            _isAnimationComplete = false;

            if (Context == null) return;

            if (Context.TryGet<AnimationType>(FsmContextType.AnimationType, out var animationType))
                _npcAnimationController.TriggerAnimation(animationType);

            if (Context.TryGet(FsmContextType.AnimationTime, out float animationTime)) _timer = animationTime;
        }

        /// <summary> Обновляет состояние каждый кадр. </summary>
        public override void Update()
        {
            if (_isPaused || _isAnimationComplete || !(_timer > 0)) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0) _isAnimationComplete = true;
        }

        /// <summary> Выполняется при выходе из состояния. </summary>
        public override void Exit()
        {
            WorldTime.OnTimeUnpaused += HandleUnpause;
            _npcAnimationController.TriggerAnimation(AnimationType.Locomotion);
        }

        /// <summary> Проверяет завершение состояния. </summary>
        /// <returns> True если анимация завершена. </returns>
        public override bool IsComplete() => _isAnimationComplete;

        private void HandlePause() => _isPaused = true;

        private void HandleUnpause() => _isPaused = false;
    }
}