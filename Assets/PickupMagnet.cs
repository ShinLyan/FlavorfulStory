using Cysharp.Threading.Tasks;
using FlavorfulStory.PickupSystem;
using FlavorfulStory.Player;
using FlavorfulStory.TimeManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    [RequireComponent(typeof(Pickup))]
    public class PickupMagnet : MonoBehaviour
    {
        [Tooltip("Задержка перед началом притягивания (сек).")]
        [SerializeField] private float _magnetDelay = 0.5f;

        [Tooltip("Максимальная дистанция для притягивания (в тайлах).")]
        [SerializeField] private float _magnetRangeTiles = 3f;

        [Tooltip("Скорость движения к игроку (в тайлах в секунду).")]
        [SerializeField] private float _magnetSpeedTiles = 9f;

        private const float TileSize = 1f;
        private const float CheckDistance = 0.1f;

        private Pickup _pickup;
        private Transform _playerTransform;
        private bool _isFlying;

        [Inject]
        public void Construct(PlayerController playerController) => _playerTransform = playerController.transform;

        private void Awake() => _pickup = GetComponent<Pickup>();

        private void OnEnable()
        {
            _isFlying = false;
            StartMagnetRoutine().Forget();
        }

        private async UniTaskVoid StartMagnetRoutine()
        {
            await UniTask.Delay((int)(_magnetDelay * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());

            if (WorldTime.IsPaused || _playerTransform == null || !_pickup.Item || !_pickup.Item.IsStackable)
                return;

            if (!_pickup.Inventory.HasSpaceFor(_pickup.Item))
                return;

            float rangeWorld = _magnetRangeTiles * TileSize;
            if (Vector3.Distance(transform.position, _playerTransform.position) > rangeWorld)
                return;

            _isFlying = true;
            float speedWorld = _magnetSpeedTiles * TileSize;

            while (_isFlying && !WorldTime.IsPaused)
            {
                Vector3 direction = (_playerTransform.position - transform.position).normalized;
                transform.position += direction * speedWorld * Time.deltaTime;

                if (Vector3.Distance(transform.position, _playerTransform.position) <= CheckDistance)
                {
                    _pickup.TryPickup();
                    break;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
        }
    }
}