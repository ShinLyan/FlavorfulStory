using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.Movement
{
    /// <summary> Передвижение игрока.</summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMover : MonoBehaviour, ISaveable
    {
        #region Private Fields
        [Header("Параметры передвижения")]
        /// <summary> Скорость движения в метрах в секунду.</summary>
        [SerializeField, Tooltip("Скорость движения")] private float _moveSpeed;

        /// <summary> Скорость поворота в радианах в секунду.</summary>
        [SerializeField, Tooltip("Скорость поворота")] private float _rotateSpeed;

        /// <summary> Клавиша, при зажатии которой происходит переключение на ходьбу вместо бега.</summary>
        [SerializeField, Tooltip("Клавиша, при зажатии которой происходит переключение на ходьбу вместо бега.")]
        private KeyCode _keyForWalking = KeyCode.LeftShift;

        /// <summary> Твердое тело.</summary>
        private Rigidbody _rigidbody;

        /// <summary> Аниматор игрока.</summary>
        private Animator _animator;
        #endregion

        /// <summary> Инициализация полей класса.</summary>
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        /// <summary> Передвижение игрока в заданном направлении.</summary>
        /// <param name="direction"> Направление, в котором движется игрок.</param>
        public void MoveAndRotate(Vector3 direction)
        {
            Move(direction);
            AnimateMovement(direction.magnitude);
            Rotate(direction);
        }

        /// <summary> Передвижение игрока в заданном направлении.</summary>
        /// <param name="direction"> Направление, в котором движется игрок.</param>
        private void Move(Vector3 direction)
        {
            Vector3 offset = _moveSpeed * CountSpeedMultiplier() * Time.deltaTime * direction;
            _rigidbody.MovePosition(_rigidbody.position + offset);
        }

        /// <summary> Расчитать множитель скорости.</summary>
        /// <returns> Возвращает значение множителя скорости.</returns>
        private float CountSpeedMultiplier()
        {
            // Значения множителей варьируются от 0 до 1.
            const float WalkingMultiplier = 0.5f;
            const float RunningMultiplier = 1f;
            return Input.GetKey(_keyForWalking) ? WalkingMultiplier : RunningMultiplier;
        }

        /// <summary> Анимировать передвижение.</summary>
        /// <param name="directionMagnitude"> Величина вектора направления.</param>
        private void AnimateMovement(float directionMagnitude)
        {
            float speed = Mathf.Clamp01(directionMagnitude) * CountSpeedMultiplier();
            const float DampTime = 0.2f; // Значение получено эмпирически
            _animator.SetFloat("Speed", speed, DampTime, Time.deltaTime);
        }

        /// <summary> Поворот игрока в заданном направлении.</summary>
        /// <param name="direction"> Направление, в котором движется игрок.</param>
        public void Rotate(Vector3 direction)
        {
            float singleStep = _rotateSpeed * Time.deltaTime;
            var newDirection = Vector3.RotateTowards(transform.forward, direction, singleStep, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        #region Saving
        [System.Serializable]
        private struct MoverSaveData
        {
            public SerializableVector3 Position;
            public SerializableVector3 Rotation;
        }

        public object CaptureState() => new MoverSaveData()
        {
            Position = new SerializableVector3(transform.position),
            Rotation = new SerializableVector3(transform.eulerAngles)
        };

        public void RestoreState(object state)
        {
            var data = (MoverSaveData)state;
            transform.position = data.Position.ToVector();
            transform.eulerAngles = data.Rotation.ToVector();
        }
        #endregion
    }
}