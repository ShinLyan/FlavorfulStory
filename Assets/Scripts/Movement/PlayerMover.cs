using System;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.Movement
{
    /// <summary> Передвижение игрока. </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMover : MonoBehaviour, ISaveable
    {
        #region Private Fields

        [Header("Параметры движения")]
        /// <summary> Скорость передвижения игрока. </summary>
        [SerializeField, Tooltip("Скорость передвижения игрока.")]
        private float _moveSpeed;

        /// <summary> Скорость поворота игрока. </summary>
        [SerializeField, Tooltip("Скорость поворота игрока.")]
        private float _rotateSpeed;

        /// <summary> Компонент Rigidbody, отвечающий за физику движения игрока. </summary>
        private Rigidbody _rigidbody;

        /// <summary> Текущий вектор направления движения игрока. </summary>
        private Vector3 _moveDirection;

        /// <summary> Текущее направление взгляда игрока. </summary>
        private Quaternion _lookRotation = Quaternion.identity;

        /// <summary> Компонент Animator для управления анимациями игрока. </summary>
        private Animator _animator;

        /// <summary> Хэшированное значение параметра "скорость" для анимации. </summary>
        private static readonly int _speedParameterHash = Animator.StringToHash("Speed");

        #endregion

        /// <summary> Инициализация компонентов (Rigidbody и Animator). </summary>
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        /// <summary> Обновление движения и поворота игрока через FixedUpdate. </summary>
        private void FixedUpdate()
        {
            Rotate();
            Move();
        }

        /// <summary> Обновляет анимацию в зависимости от направления движения. </summary>
        private void Update()
        {
            AnimateMovement(_moveDirection.magnitude);
        }

        /// <summary> Выполняет передвижение игрока в соответствии с направлением движения. </summary>
        private void Move()
        {
            var moveForce = _moveSpeed * CountSpeedMultiplier() * _moveDirection;
            moveForce.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = moveForce;
        }

        /// <summary> Плавно поворачивает игрока в указанном направлении. </summary>
        private void Rotate()
        {
            _rigidbody.MoveRotation(
                Quaternion.Slerp(transform.rotation,
                    _lookRotation,
                    Time.fixedDeltaTime * _rotateSpeed)
            );
        }

        /// <summary> Вычисляет множитель скорости в зависимости от режима (ходьба или бег). </summary>
        /// <returns> Множитель скорости (0.5 для ходьбы, 1 для бега). </returns>
        private static float CountSpeedMultiplier()
        {
            const float WalkingMultiplier = 0.5f;
            const float RunningMultiplier = 1f;
            return InputWrapper.GetButton(InputButton.Walking) ? WalkingMultiplier : RunningMultiplier;
        }

        /// <summary> Обновляет анимацию движения игрока в зависимости от скорости. </summary>
        /// <param name="directionMagnitude"> Величина направления движения. </param>
        private void AnimateMovement(float directionMagnitude)
        {
            float speed = Mathf.Clamp01(directionMagnitude) * CountSpeedMultiplier();
            const float DampTime = 0.2f; // Время сглаживания перехода анимации
            _animator.SetFloat(_speedParameterHash, speed, DampTime, Time.deltaTime);
        }

        /// <summary> Устанавливает направление движения игрока. </summary>
        /// <param name="direction"> Вектор направления движения. </param>
        public void SetMoveDirection(Vector3 direction) => _moveDirection = direction;

        /// <summary> Устанавливает направление взгяда игрока. </summary>
        /// <param name="rotation"> Кватернион направления взгляда. </param>
        public void SetLookRotation(Quaternion rotation) => _lookRotation = rotation;

        /// <summary> Установить позицию и мгновенно перенести к ней игрока. </summary>
        /// <param name="position"> Позиция, к которой необходимо перенести игрока. </param>
        public void SetPosition(Vector3 position) => _rigidbody.MovePosition(position);

        #region Saving

        /// <summary> Структура для сохранения позиции и поворота игрока. </summary>
        [Serializable]
        private struct MoverSaveData
        {
            /// <summary> Позиция игрока. </summary>
            public SerializableVector3 Position;

            /// <summary> Поворот игрока. </summary>
            public SerializableVector3 Rotation;
        }

        /// <summary> Сохраняет текущее состояние игрока (позиция и поворот). </summary>
        /// <returns> Объект с данными позиции и поворота. </returns>
        public object CaptureState() => new MoverSaveData
        {
            Position = new SerializableVector3(transform.position),
            Rotation = new SerializableVector3(transform.eulerAngles)
        };

        /// <summary> Восстанавливает состояние игрока из сохранения. </summary>
        /// <param name="state"> Объект сохраненного состояния. </param>
        public void RestoreState(object state)
        {
            var data = (MoverSaveData)state;
            transform.position = data.Position.ToVector();
            transform.eulerAngles = data.Rotation.ToVector();
        }

        #endregion
    }
}