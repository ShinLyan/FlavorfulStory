using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.Movement
{
    /// <summary> ������������ ������.</summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMover : MonoBehaviour, ISaveable
    {
        #region Private Fields
        [Header("��������� ������������")]
        /// <summary> �������� �������� � ������ � �������.</summary>
        [SerializeField, Tooltip("�������� ��������")] private float _moveSpeed;

        /// <summary> �������� �������� � �������� � �������.</summary>
        [SerializeField, Tooltip("�������� ��������")] private float _rotateSpeed;

        /// <summary> �������, ��� ������� ������� ���������� ������������ �� ������ ������ ����.</summary>
        [SerializeField, Tooltip("�������, ��� ������� ������� ���������� ������������ �� ������ ������ ����.")]
        private KeyCode _keyForWalking = KeyCode.LeftShift;

        /// <summary> ������� ����.</summary>
        private Rigidbody _rigidbody;

        /// <summary> Вектор движения игрока. </summary>
        private Vector3 _moveDirection;
        
        /// <summary> Вектор поворота(взгляда) игрока. </summary>
        private Vector3 _lookDirection;

        /// <summary> �������� ������.</summary>
        private Animator _animator;

        /// <summary> Хэшированное значение параметра аниматора (скорость). </summary>
        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        /// <summary> ������������� ����� ������.</summary>
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        /// <summary> Коллбэк UnityAPI. Инициализация вектора движения. </summary>
        private void Start()
        {
            _moveDirection = Vector3.zero;
            _lookDirection = Vector3.zero;
        }

        /// <summary> Коллбэк UnityAPI. Поворот и передвижение игрока. </summary>
        private void FixedUpdate()
        {
            Rotate();
            Move();
        }

        /// <summary> Коллбэк UnityAPI. Анимаиця передвижения игрока. </summary>
        private void Update()
        {
            AnimateMovement(_moveDirection.magnitude);
        }
        
        /// <summary> Передвижение игрока в соответствии с Input'ом. </summary>
        private void Move()
        {
            Vector3 moveForce = _moveSpeed * CountSpeedMultiplier() * _moveDirection;
            moveForce.y = _rigidbody.velocity.y;
            _rigidbody.velocity = moveForce;
        }
        
        /// <summary> Плавный поворот игрока в соответствии с Input'ом. </summary>
        private void Rotate()
        {
            _rigidbody.MoveRotation(
                Quaternion.Slerp(transform.rotation, 
                    Quaternion.LookRotation(_lookDirection),
                    Time.fixedDeltaTime * _rotateSpeed)
            );
        }

        /// <summary> ��������� ��������� ��������.</summary>
        /// <returns> ���������� �������� ��������� ��������.</returns>
        private float CountSpeedMultiplier()
        {
            // �������� ���������� ����������� �� 0 �� 1.
            const float WalkingMultiplier = 0.5f;
            const float RunningMultiplier = 1f;
            return Input.GetKey(_keyForWalking) ? WalkingMultiplier : RunningMultiplier;
        }

        /// <summary> ����������� ������������.</summary>
        /// <param name="directionMagnitude"> �������� ������� �����������.</param>
        private void AnimateMovement(float directionMagnitude)
        {
            float speed = Mathf.Clamp01(directionMagnitude) * CountSpeedMultiplier();
            const float DampTime = 0.2f; // �������� �������� �����������
            _animator.SetFloat(Speed, speed, DampTime, Time.deltaTime);
        }
        
        /// <summary> Задать направление движения игрока. </summary>
        /// <param name="direction"> Вектор направления. </param>
        public void SetMoveDirection(Vector3 direction) => _moveDirection = direction;

        /// <summary> Задать направление взгляда игрока. </summary>
        /// <param name="direction"> Вектор взгляда. </param>
        public void SetLookRotation(Vector3 direction) => _lookDirection = direction;
        
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