using System;
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

        /// <summary> �������� ������.</summary>
        private Animator _animator;
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
        }

        /// <summary> ������������ ������ � �������� �����������.</summary>
        /// <param name="direction"> �����������, � ������� �������� �����.</param>
        private void MoveAndRotate()
        {
            // Rotate(_moveDirection);
            // AnimateMovement(_moveDirection.magnitude);
            // Move(_moveDirection);
        }

        private void FixedUpdate()
        {
            Rotate(_moveDirection);
            Move(_moveDirection);
        }

        private void Update()
        {
            AnimateMovement(_moveDirection.magnitude);
        }

        public void SetDirection(Vector3 direction) => _moveDirection = direction;
        
        /// <summary> ������������ ������ � �������� �����������.</summary>
        /// <param name="direction"> �����������, � ������� �������� �����.</param>
        private void Move(Vector3 direction)
        {
            Vector3 offset = _moveSpeed * CountSpeedMultiplier() * Time.fixedDeltaTime * direction;
            //_rigidbody.MovePosition(_rigidbody.position + offset);
            Vector3 moveForce = _moveSpeed * CountSpeedMultiplier() * direction;
            moveForce.y = _rigidbody.velocity.y;
            // _rigidbody.AddForce(moveForce, ForceMode.Impulse);
            // if (_rigidbody.velocity.magnitude > _moveSpeed)
            // {
            //     _rigidbody.velocity = direction * _moveSpeed;
            // }
            _rigidbody.velocity = moveForce;
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
            _animator.SetFloat("Speed", speed, DampTime, Time.deltaTime);
        }

        /// <summary> ������� ������ � �������� �����������.</summary>
        /// <param name="direction"> �����������, � ������� �������� �����.</param>
        public void Rotate(Vector3 direction)
        {
            float singleStep = _rotateSpeed * Time.fixedDeltaTime;
            var newDirection = Vector3.RotateTowards(transform.forward, direction, singleStep, 0f);
            _rigidbody.MoveRotation(Quaternion.LookRotation(newDirection));
            //_rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(newDirection), _rotateSpeed));
            //_rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_moveDirection), Time.fixedDeltaTime * _rotateSpeed));
            
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