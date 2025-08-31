using FlavorfulStory.InventorySystem;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.PickupSystem
{
    [RequireComponent(typeof(Pickup))]
    public class PickupMagnet : MonoBehaviour
    {
        [SerializeField, Tooltip("Пауза перед началом примагничивания (сек).")]
        private float _magnetDelay = 0.0f;

        [SerializeField, Tooltip("Максимальная дистанция для примагничивания (в тайлах).")]
        private float _magnetRangeTiles = 3f;

        [SerializeField, Tooltip("Скорость движения к игроку (в тайлах/сек).")]
        private float _magnetSpeedTiles = 0.9f;

        [Header("Tuning")]
        [SerializeField, Tooltip("Считать достигнутым игрока при этой дистанции (мировые ед.).")]
        private float _reachDistance = 0.01f;

        private const float TileSize = 1f;

        private Pickup _pickup;
        private Transform _playerTransform;
        private Inventory _playerInventory;

        private bool _magnetScheduled;
        private bool _isFlying;
        private float _delayTimer;

        private Rigidbody _rb;
        private Collider[] _allColliders;
        private SphereCollider _pickupTrigger;

        [Inject]
        private void Construct(Player.PlayerController playerController, Inventory playerInventory)
        {
            _playerTransform = playerController.transform;
            _playerInventory = playerInventory;
        }

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
            _allColliders = GetComponentsInChildren<Collider>(true);
            TryGetComponent(out _rb);
            
            foreach (var col in _allColliders)
                if (col is SphereCollider sc && sc.isTrigger) { _pickupTrigger = sc; break; }

            if (_playerInventory != null)
                _playerInventory.InventoryUpdated += OnInventoryUpdated;
        }

        private void OnDestroy()
        {
            if (_playerInventory != null)
                _playerInventory.InventoryUpdated -= OnInventoryUpdated;
        }

        private void Start() => TryScheduleMagnet();

        private void Update()
        {
            if (WorldTime.IsPaused) return;

            if (_magnetScheduled && !_isFlying)
            {
                _delayTimer -= Time.deltaTime;
                if (_delayTimer <= 0f) BeginFlying();
            }

            if (_isFlying)
            {
                var target = _playerTransform.position;
                float speedWorld = _magnetSpeedTiles * TileSize;
                transform.position = Vector3.MoveTowards(transform.position, target, speedWorld * Time.deltaTime);
                
                if ((transform.position - target).sqrMagnitude <= _reachDistance * _reachDistance)
                {
                    //TODO: Тянуть к центру по Y игрока?
                }
            }
            else
            {
                TryScheduleMagnet();
            }
        }

        private void OnInventoryUpdated() => TryScheduleMagnet();

        private void TryScheduleMagnet()
        {
            if (_isFlying || _magnetScheduled) return;
            
            if (!_pickup.IsReadyForPickup) return;
            
            if (!_playerInventory.HasSpaceFor(_pickup.Item)) return;
            
            float maxDistWorld = _magnetRangeTiles * TileSize;
            if ((transform.position - _playerTransform.position).sqrMagnitude > maxDistWorld * maxDistWorld) return;

            _magnetScheduled = true;
            _delayTimer = _magnetDelay;
        }

        private void BeginFlying()
        {
            _isFlying = true;

            if (_rb)
            {
                if (!_rb.isKinematic)
                {
                    _rb.linearVelocity = Vector3.zero;
                    _rb.angularVelocity = Vector3.zero;
                }
                
                _rb.isKinematic = true;
            }

            foreach (var c in _allColliders)
            {
                if (c == _pickupTrigger) { c.enabled = true; continue; }
                if (!c.isTrigger) c.enabled = false;
            }
        }
    }
}