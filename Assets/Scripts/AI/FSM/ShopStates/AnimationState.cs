using System.Threading;
using Cysharp.Threading.Tasks;
using FlavorfulStory.AI.BaseNpc;
using FlavorfulStory.TimeManagement;
using UnityEngine;

namespace FlavorfulStory.AI.FSM.ShopStates
{
    /// <summary> Состояние анимации персонажа с таймером для управления завершением. </summary>
    public class AnimationState : CharacterState
    {
        /// <summary> Флаг завершения анимации. </summary>
        private bool _isAnimationComplete;

        /// <summary> Контроллер анимаций NPC. </summary>
        private readonly NpcAnimationController _npcAnimationController;

        /// <summary> Токен отмены для асинхронного таймера. </summary>
        private CancellationTokenSource _cts;

        /// <summary> Инициализирует состояние анимации. </summary>
        /// <param name="npcAnimationController"> Контроллер анимаций NPC. </param>
        public AnimationState(NpcAnimationController npcAnimationController) =>
            _npcAnimationController = npcAnimationController;

        /// <summary> Выполняется при входе в состояние. </summary>
        public override void Enter()
        {
            _isAnimationComplete = false;
            _cts = new CancellationTokenSource();

            if (Context == null) return;

            if (Context.TryGet<AnimationType>(FsmContextType.AnimationType, out var animationType))
                _npcAnimationController.TriggerAnimation(animationType);

            if (Context.TryGet(FsmContextType.AnimationTime, out float animationTime))
                RunAnimationTimerAsync(animationTime, _cts.Token).Forget();
        }

        /// <summary> Асинхронный таймер для завершения анимации. </summary>
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
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _npcAnimationController.TriggerAnimation(AnimationType.Locomotion);
        }

        /// <summary> Проверяет завершение состояния. </summary>
        /// <returns> True если анимация завершена. </returns>
        public override bool IsComplete() => _isAnimationComplete;
    }
}