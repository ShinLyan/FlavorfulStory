using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FlavorfulStory.GridSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Player;

namespace FlavorfulStory.PickupSystem
{
    /// <summary> Обеспечивает автоматическое притягивание предмета к игроку при выполнении условий. </summary>
    public class PickupMagnet
    {
        /// <summary> Смещение позиции игрока вверх, чтобы предмет не въезжал в ноги. </summary>
        private readonly Vector3 _playerOffset = new(0f, 0.5f, 0f);

        /// <summary> Трансформ объекта предмета. </summary>
        private readonly Transform _transform;
        /// <summary> Rigidbody предмета, используемый для отключения физики во время притягивания. </summary>
        private readonly Rigidbody _rigidbody;
        /// <summary> Все коллайдеры предмета, включая триггер и физические. </summary>
        private readonly Collider[] _colliders;
        /// <summary> Компонент Pickup, содержащий данные предмета и флаг CanBePickedUp. </summary>
        private readonly Pickup _pickup;
        /// <summary> Провайдер позиции игрока. </summary>
        private readonly IPlayerPositionProvider _playerPositionProvider;
        /// <summary> Инвентарь игрока, в который будет добавлен предмет. </summary>
        private readonly Inventory _playerInventory;
        
        /// <summary> Настройки поведения притягивания предметов. </summary>
        private PickupSettings _pickupSettings;
        
        /// <summary> Источник отмены для текущей задачи притягивания. </summary>
        private CancellationTokenSource _cts;
        /// <summary> Флаг, указывающий, что предмет сейчас летит к игроку. </summary>
        private bool _isFlying;
        
        /// <summary> Конструктор притягивателя. </summary>
        /// <param name="pickup"> Ссылка на компонент Pickup. </param>
        /// <param name="transform"> Transform объекта предмета. </param>
        /// <param name="rigidbody"> Rigidbody предмета. </param>
        /// <param name="colliders"> Массив всех коллайдеров предмета. </param>
        /// <param name="playerPositionProvider"> Провайдер позиции игрока. </param>
        /// <param name="inventoryProvider"> Провайдер инвентарей, откуда берётся инвентарь игрока. </param>
        /// <param name="pickupSettings"> Настройки притягивания предметов. </param>
        public PickupMagnet(
            Pickup pickup,
            Transform transform,
            Rigidbody rigidbody,
            Collider[] colliders,
            IPlayerPositionProvider playerPositionProvider,
            IInventoryProvider inventoryProvider,
            PickupSettings pickupSettings)
        {
            _pickup = pickup;
            _transform = transform;
            _rigidbody = rigidbody;
            _colliders = colliders;
            _playerPositionProvider = playerPositionProvider;
            _playerInventory = inventoryProvider.GetPlayerInventory();
            _pickupSettings = pickupSettings;

            _playerInventory.InventoryUpdated += OnInventoryChanged;
        }

        /// <summary> Планирует запуск притягивания, если предмет может быть притянут. </summary>
        public void ScheduleMagnet()
        {
            if (_isFlying || !_pickup.CanBePickedUp)
                return;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            CheckNearbyAsync(_cts.Token).Forget();
        }
        
        /// <summary> Вызывается при изменении инвентаря. Повторно запускает магнит, если возможно. </summary
        private void OnInventoryChanged()
        {
            if (!_isFlying && CanStartMagnet()) ScheduleMagnet();
        }
        
        /// <summary> Освобождает ресурсы и отписывается от событий. </summary>
        public void Dispose()
        {
            _playerInventory.InventoryUpdated -= OnInventoryChanged;
            CancelFlying();
        }

        /// <summary> Асинхронно проверяет, может ли предмет начать притягиваться, с периодическими интервалами. </summary>
        /// <param name="token"> Токен отмены для прерывания задачи. </param>
        private async UniTaskVoid CheckNearbyAsync(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_pickupSettings.MagnetActivationDelay), cancellationToken: token); 

                while (!token.IsCancellationRequested)
                {
                    if (!_isFlying && CanStartMagnet())
                    {
                        BeginFlying();
                        await MoveToPlayerAsync(token);
                        FinishFlying();

                        break;
                    }

                    await UniTask.Delay(
                        TimeSpan.FromSeconds(_pickupSettings.MagnetCheckIntervalSeconds), cancellationToken: token);
                }
            }
            catch (OperationCanceledException) { }
        }

        /// <summary> Инициализирует полёт предмета: отключает физику и коллайдеры. </summary>
        private void BeginFlying()
        {
            _isFlying = true;
            if (!_rigidbody.isKinematic)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }

            foreach (var collider in _colliders) collider.enabled = collider == collider.isTrigger;
        }

        /// <summary> Перемещает предмет к игроку с заданной скоростью до выхода из радиуса. </summary>
        /// <param name="token"> Токен отмены для остановки движения. </param>
        private async UniTask MoveToPlayerAsync(CancellationToken token)
        {
            float speed = _pickupSettings.MagnetSpeedTiles * GridPositionProvider.CellSize;

            while (IsInRange())
            {
                Vector3 target = _playerPositionProvider.GetPlayerPosition() + _playerOffset;
                _transform.position = Vector3.MoveTowards(_transform.position, target, speed * Time.deltaTime);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        /// <summary> Завершает притягивание и включает все коллайдеры. </summary>
        private void FinishFlying()
        {
            _isFlying = false;
            EnableAllColliders();
        }

        /// <summary> Отменяет текущую задачу притягивания и сбрасывает флаги. </summary>
        private void CancelFlying()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _isFlying = false;
        }

        /// <summary> Проверяет, можно ли начать притягивание (по условиям и дистанции). </summary>
        private bool CanStartMagnet() => _pickup.CanBePickedUp && IsInRange();

        /// <summary> Проверяет, находится ли предмет в радиусе магнитного притягивания. </summary>
        private bool IsInRange() =>
            Vector3.Distance(_transform.position, _playerPositionProvider.GetPlayerPosition()) <= 
                             _pickupSettings.MagnetRangeTiles * GridPositionProvider.CellSize;

        /// <summary> Включает все коллайдеры предмета. </summary>
        private void EnableAllColliders()
        {
            foreach (var collider in _colliders) collider.enabled = true;
        }
    }
}