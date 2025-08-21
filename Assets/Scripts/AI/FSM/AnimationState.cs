using System.Threading;
using Cysharp.Threading.Tasks;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.AI.FSM
{
    /// <summary> Универсальное состояние анимации NPC с поддержкой таймера. </summary>
    public class AnimationState : CharacterState
    {
        /// <summary> Контроллер анимаций NPC. </summary>
        private readonly NpcAnimationController _animationController;

        /// <summary> Источник токена отмены. </summary>
        private CancellationTokenSource _cts;

        /// <summary> Флаг завершения анимации. </summary>
        private bool _isAnimationComplete;

        /// <summary> Инициализирует состояние анимации. </summary>
        /// <param name="animationController"> Контроллер анимаций NPC. </param>
        public AnimationState(NpcAnimationController animationController) => _animationController = animationController;

        /// <summary> Выполняется при входе в состояние. </summary>
        public override void Enter()
        {
            base.Enter();

            _isAnimationComplete = false;
            _cts = new CancellationTokenSource();

            if (Context == null) return;

            if (Context.TryGet<AnimationType>(FsmContextType.AnimationType, out var animationType))
                _animationController.TriggerAnimation(animationType);

            if (Context.TryGet(FsmContextType.AnimationTime, out float animationTime))
                RunAnimationTimerAsync(animationTime, _cts.Token).Forget();
        }

        /// <summary> Запускает таймер анимации. </summary>
        /// <param name="animationTime"> Время анимации </param>
        /// <param name="token"> Токен отмены. </param>
        private async UniTaskVoid RunAnimationTimerAsync(float animationTime, CancellationToken token)
        {
            float timeLeft = animationTime;

            while (timeLeft > 0 && !token.IsCancellationRequested)
                if (!WorldTime.IsPaused)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    timeLeft -= Time.deltaTime;
                }
                else
                {
                    await UniTask.WaitUntil(() => !WorldTime.IsPaused, cancellationToken: token);
                }

            if (!token.IsCancellationRequested) _isAnimationComplete = true;
        }

        /// <summary> Выполняется при выходе из состояния. </summary>
        public override void Exit()
        {
            base.Exit();

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        /// <summary> Сбрасывает состояние. </summary>
        public override void Reset()
        {
            base.Reset();

            _animationController.Reset();
        }

        /// <summary> Проверяет завершение анимации. </summary>
        /// <returns> <c>true</c> - если анимация завершена. </returns>
        public override bool IsComplete() => _isAnimationComplete;
    }
}