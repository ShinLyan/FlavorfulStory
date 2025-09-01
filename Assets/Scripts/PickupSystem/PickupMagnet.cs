using FlavorfulStory.GridSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.PickupSystem
{
    /// <summary> Отвечает за автоматическое притягивание предмета к игроку. </summary>
    [RequireComponent(typeof(Pickup))]
    public class PickupMagnet : MonoBehaviour
    {
        /// <summary> Максимальная дистанция для притягивания (в тайлах). </summary>
        [SerializeField, Tooltip("Максимальная дистанция для примагничивания (в тайлах).")]
        private float _magnetRangeTiles = 3f;

        /// <summary> Скорость движения предмета к игроку (в тайлах/секунду). </summary>
        [SerializeField, Tooltip("Скорость движения к игроку (в тайлах/сек).")]
        private float _magnetSpeedTiles = 9f;

        /// <summary> Смещение вверх при движении к игроку, чтобы предмет не въезжал в ноги. </summary>
        private readonly Vector3 _playerPositionOffset = new(0f, 0.5f, 0f);

        /// <summary> Провайдер позиции игрока. </summary>
        private IPlayerPositionProvider _playerPositionProvider;

        /// <summary> Ссылка на инвентарь игрока. </summary>
        private IInventoryProvider _inventoryProvider;

        /// <summary> Компонент Pickup, отвечающий за предмет. </summary>
        private Pickup _pickup;

        /// <summary> Rigidbody предмета. </summary>
        private Rigidbody _rigidbody;

        /// <summary> Коллайдер триггера, используемый для захвата предмета. </summary>
        private SphereCollider _pickupTrigger;

        /// <summary> Все коллайдеры на предмете (включая вложенные). </summary>
        private Collider[] _colliders;

        /// <summary> Флаг, указывающий, что запланировано начало притягивания. </summary>
        private bool _isScheduled;

        /// <summary> Флаг, указывающий, что предмет сейчас летит к игроку. </summary>
        private bool _isFlying;

        /// <summary> Время ожидания до начала притягивания. </summary>
        private float _delayTimer;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="playerPositionProvider">  Провайдер позиции игрока. </param>
        /// <param name="inventoryProvider"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(IPlayerPositionProvider playerPositionProvider, IInventoryProvider inventoryProvider)
        {
            _playerPositionProvider = playerPositionProvider;
            _inventoryProvider = inventoryProvider;
        }

        /// <summary> Инициализация компонентов при создании объекта. </summary>
        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
            _rigidbody = GetComponent<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>(true);

            _pickupTrigger = FindTriggerCollider(_colliders);
        }

        /// <summary> Ищет и возвращает триггерный <see cref="SphereCollider"/> среди всех коллайдеров объекта. </summary>
        /// <param name="colliders"> Массив коллайдеров. </param>
        /// <returns> Триггерный <see cref="SphereCollider"/> или <c>null</c>, если не найден. </returns>
        private static SphereCollider FindTriggerCollider(Collider[] colliders)
        {
            foreach (var collider in colliders)
                if (collider is SphereCollider { isTrigger: true } trigger)
                    return trigger;

            Debug.LogWarning("[PickupMagnet] No trigger SphereCollider found.");
            return null;
        }

        /// <summary> Подписка на события при включении объекта. </summary>
        private void OnEnable() => _inventoryProvider.GetPlayerInventory().InventoryUpdated += OnInventoryChanged;

        /// <summary> Отписка от событий при отключении объекта. </summary>
        private void OnDisable() => _inventoryProvider.GetPlayerInventory().InventoryUpdated -= OnInventoryChanged;

        /// <summary> Коллбэк при изменении состояния инвентаря игрока. </summary>
        private void OnInventoryChanged() => TryScheduleMagnet();

        /// <summary> Планирует начало притягивания, если условия соблюдены. </summary>
        private void TryScheduleMagnet()
        {
            if (_isScheduled || _isFlying || !CanStartMagnet()) return;

            _isScheduled = true;
            _delayTimer = 0f;
        }

        /// <summary> Обновление состояния объекта каждый кадр. </summary>
        private void Update()
        {
            if (WorldTime.IsPaused) return;

            if (_isScheduled && !_isFlying) UpdateDelayTimer();

            if (_isFlying)
            {
                MoveToPlayer();
                if (ShouldCancelFlying()) CancelFlying();
            }
            else
            {
                TryScheduleMagnet();
            }
        }

        /// <summary> Обновляет таймер задержки и начинает полёт при его окончании. </summary>
        private void UpdateDelayTimer()
        {
            _delayTimer -= Time.deltaTime;
            if (_delayTimer <= 0f) BeginFlying();
        }

        /// <summary> Активирует режим полёта к игроку. </summary>
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

        /// <summary> Отключает все коллайдеры, кроме триггера. </summary>
        private void EnableOnlyTriggerCollider()
        {
            foreach (var collider in _colliders) collider.enabled = collider == _pickupTrigger || collider.isTrigger;
        }

        /// <summary> Двигает предмет к игроку. </summary>
        private void MoveToPlayer()
        {
            float speed = _magnetSpeedTiles * GridPositionProvider.CellSize;
            var target = _playerPositionProvider.GetPlayerPosition() + _playerPositionOffset;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        /// <summary> Проверяет, следует ли отменить притягивание (например, если игрок ушёл слишком далеко). </summary>
        private bool ShouldCancelFlying() => DistanceToPlayer() > MaxRangeWorld();

        /// <summary> Проверяет, можно ли начать притягивание. </summary>
        /// <returns> <c>True</c>, если условия для начала притягивания соблюдены. </returns>
        private bool CanStartMagnet()
        {
            if (!_pickup.CanBePickedUp) return false;

            return DistanceToPlayer() <= MaxRangeWorld();
        }

        /// <summary> Возвращает расстояние от предмета до игрока. </summary>
        private float DistanceToPlayer() =>
            Vector3.Distance(transform.position, _playerPositionProvider.GetPlayerPosition());

        /// <summary> Возвращает максимальную дистанцию притягивания в мировых координатах. </summary>
        private float MaxRangeWorld() => _magnetRangeTiles * GridPositionProvider.CellSize;

        /// <summary> Останавливает притягивание и возвращает коллайдеры к обычному состоянию. </summary>
        private void CancelFlying()
        {
            _isFlying = false;
            _isScheduled = false;

            _rigidbody.detectCollisions = true;
            _rigidbody.useGravity = false;

            EnableAllColliders();
        }

        /// <summary> Включает все физические (не-триггерные) коллайдеры. </summary>
        private void EnableAllColliders()
        {
            foreach (var collider in _colliders)
                if (!collider.isTrigger)
                    collider.enabled = true;
        }
    }
}