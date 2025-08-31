using UnityEngine;
using Zenject;
using FlavorfulStory.GridSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;

namespace FlavorfulStory.PickupSystem
{
    [RequireComponent(typeof(Pickup))]
    public class PickupMagnet : MonoBehaviour
    {
        [SerializeField, Tooltip("Максимальная дистанция для примагничивания (в тайлах).")]
        private float _magnetRangeTiles = 3f;

        [SerializeField, Tooltip("Скорость движения к игроку (в тайлах/сек).")]
        private float _magnetSpeedTiles = 9f;

        private const float TileSize = GridPositionProvider.CellSize;
        private readonly Vector3 _playerPositionOffset = new(0f, 0.5f, 0f);

        private Transform _playerTransform;
        private Inventory _playerInventory;

        private Pickup _pickup;
        private Rigidbody _rigidbody;
        private SphereCollider _pickupTrigger;
        private Collider[] _colliders;

        private bool _isScheduled;
        private bool _isFlying;
        private float _delayTimer;

        [Inject]
        private void Construct(PlayerController player, Inventory playerInventory)
        {
            _playerTransform = player.transform;
            _playerInventory = playerInventory;
        }

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
            _rigidbody = GetComponent<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>(includeInactive: true);

            _pickupTrigger = FindTriggerCollider(_colliders);
            _playerInventory.InventoryUpdated += OnInventoryChanged;
        }

        private void OnDestroy() => _playerInventory.InventoryUpdated -= OnInventoryChanged;

        private void Start()
        {
            TryScheduleMagnet();
        }

        private void Update()
        {
            if (WorldTime.IsPaused) return;

            if (_isScheduled && !_isFlying)
            {
                UpdateDelayTimer();
            }

            if (_isFlying)
            {
                FlyTowardsPlayer();
                if (ShouldCancelFlying())
                {
                    CancelFlying();
                }
            }
            else
            {
                TryScheduleMagnet();
            }
        }

        private void OnInventoryChanged() => TryScheduleMagnet();

        private void TryScheduleMagnet()
        {
            if (_isScheduled || _isFlying || !CanStartMagnet()) return;

            _isScheduled = true;
            _delayTimer = 0f;
        }

        private bool CanStartMagnet()
        {
            if (!_pickup.CanBePickedUp) return false;

            return DistanceToPlayer() <= MaxRangeWorld();
        }

        private float DistanceToPlayer() => Vector3.Distance(transform.position, _playerTransform.position);
        private float MaxRangeWorld() => _magnetRangeTiles * TileSize;

        private void UpdateDelayTimer()
        {
            _delayTimer -= Time.deltaTime;
            if (_delayTimer <= 0f)
                BeginFlying();
        }

        private void BeginFlying()
        {
            _isFlying = true;
            _isScheduled = false;

            if (!_rigidbody.isKinematic)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }

            EnableOnlyTriggerCollider();
        }

        private void FlyTowardsPlayer()
        {
            var speed = _magnetSpeedTiles * TileSize;
            var target = _playerTransform.position + _playerPositionOffset;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        private bool ShouldCancelFlying() => DistanceToPlayer() > MaxRangeWorld();

        private void CancelFlying()
        {
            _isFlying = false;
            _isScheduled = false;
            
            _rigidbody.detectCollisions = true;
            _rigidbody.useGravity = false;

            EnableAllColliders();
        }

        private void EnableOnlyTriggerCollider()
        {
            foreach (var col in _colliders)
            {
                col.enabled = (col == _pickupTrigger) || col.isTrigger;
            }
        }

        private void EnableAllColliders()
        {
            foreach (var col in _colliders)
            {
                if (!col.isTrigger)
                    col.enabled = true;
            }
        }

        private SphereCollider FindTriggerCollider(Collider[] colliders)
        {
            foreach (var col in colliders)
            {
                if (col is SphereCollider { isTrigger: true } trigger) return trigger;
            }

            Debug.LogWarning("[PickupMagnet] No trigger SphereCollider found.");
            return null;
        }
    }
}