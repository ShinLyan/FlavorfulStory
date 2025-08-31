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
        private float _magnetRangeTiles;

        [SerializeField, Tooltip("Скорость движения к игроку (в тайлах/сек).")]
        private float _magnetSpeedTiles;

        private Transform _playerTransform;
        private Inventory _playerInventory;

        private Pickup _pickup;
        private Rigidbody _rigidbody;
        private Collider[] _allColliders;
        private SphereCollider _pickupTrigger;
        
        private bool _magnetScheduled;
        private bool _isFlying;
        private float _delayTimer;
        
        //TODO: Обновить playerInventory на IInventoryProvider
        [Inject]
        private void Construct(PlayerController playerController, Inventory playerInventory)
        {
            _playerTransform = playerController.transform;
            _playerInventory = playerInventory;
        }

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
            _rigidbody = GetComponent<Rigidbody>();
            _allColliders = GetComponentsInChildren<Collider>(true);
            
            foreach (var col in _allColliders)
                if (col is SphereCollider { isTrigger: true } sphereCollider)
                {
                    _pickupTrigger = sphereCollider; 
                    break;
                }

            _playerInventory.InventoryUpdated += OnInventoryUpdated;
        }

        private void OnDestroy() => _playerInventory.InventoryUpdated -= OnInventoryUpdated;

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
                float speedWorld = _magnetSpeedTiles * GridPositionProvider.CellSize;
                transform.position = Vector3.MoveTowards(transform.position, target, speedWorld * Time.deltaTime);
                float magnetRangeWorld = _magnetRangeTiles * GridPositionProvider.CellSize;
                if (Vector3.Distance(transform.position, _playerTransform.position) > magnetRangeWorld)
                {
                    CancelFlying();
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
            
            if (!_pickup.CanBePickedUp) return;
            
            float magnetRangeWorld = _magnetRangeTiles * GridPositionProvider.CellSize;
            if (Vector3.Distance(transform.position, _playerTransform.position) > magnetRangeWorld) return;

            _magnetScheduled = true;
            _delayTimer = 0f;
        }

        private void BeginFlying()
        {
            _isFlying = true;

            if (_rigidbody)
            {
                if (!_rigidbody.isKinematic)
                {
                    _rigidbody.linearVelocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                }
                _rigidbody.isKinematic = true;
            }

            foreach (var c in _allColliders)
            {
                if (c == _pickupTrigger)
                {
                    c.enabled = true;
                    continue;
                }
                
                if (!c.isTrigger)
                    c.enabled = false;
            }
        }
        
        private void CancelFlying()
        {
            _isFlying = false;
            _magnetScheduled = false;
            
            _rigidbody.detectCollisions = true;

            foreach (var c in _allColliders)
            {
                if (!c.isTrigger) c.enabled = true;
            }
        }
    }
}