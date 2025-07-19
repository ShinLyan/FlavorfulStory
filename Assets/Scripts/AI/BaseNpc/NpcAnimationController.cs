using UnityEngine;

namespace FlavorfulStory.AI.BaseNpc
{
    /// <summary> Контроллер анимации NPC, управляющий всеми анимационными состояниями персонажа. </summary>
    public class NpcAnimationController
    {
        /// <summary> Компонент Animator для управления анимациями. </summary>
        private readonly Animator _animator;

        /// <summary> Хэш параметра скорости для оптимизации обращения к Animator. </summary>
        private readonly int _speedHash = Animator.StringToHash("Speed");

        /// <summary> Инициализирует контроллер анимации с заданным компонентом Animator. </summary>
        /// <param name="animator"> Компонент Animator для управления анимациями NPC. </param>
        public NpcAnimationController(Animator animator) => _animator = animator;

        /// <summary> Устанавливает скорость анимации с плавным переходом. </summary>
        /// <param name="speed"> Значение скорости для установки. </param>
        /// <param name="dampTime"> Время сглаживания перехода (по умолчанию 0.2 секунды). </param>
        public void SetSpeed(float speed, float dampTime = 0.2f)
        {
            if (_animator.speed == 0) return;

            _animator.SetFloat(_speedHash, speed, dampTime, Time.deltaTime);
        }

        /// <summary> Запустить анимацию. </summary>
        /// <param name="animationType"> Тип проигрываемой анимации.</param>
        public void TriggerAnimation(AnimationType animationType)
        {
            string animationName = animationType.ToString();
            _animator.SetTrigger(Animator.StringToHash(animationName));
        }

        /// <summary> Остановить анимацию. </summary>
        public void PauseAnimation() => _animator.speed = 0;

        /// <summary> Продолжить анимацию. </summary>
        public void ContinueAnimation() => _animator.speed = 1;

        /// <summary> Сбрасывает все параметры Animator к значениям по умолчанию. </summary>
        public void Reset() => _animator.Rebind();
    }
}