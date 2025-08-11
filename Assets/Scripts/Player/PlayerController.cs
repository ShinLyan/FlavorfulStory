using FlavorfulStory.CursorSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.Stats;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.Toolbar;
using FlavorfulStory.Toolbar.UI;
using FlavorfulStory.Tools;
using FlavorfulStory.Utils;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Player
{
    /// <summary> Контроллер игрока, отвечающий за всё управление персонажем. </summary>
    [RequireComponent(typeof(PlayerMover), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Трансформ выброса предмета. </summary>
        /// <remarks> Прокидывается в <see cref="IItemDropService"/>. </remarks>
        [SerializeField] private Transform _dropPoint;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Панель быстрого доступа. </summary>
        private ToolbarView _toolbarView;

        /// <summary> Сервис выброса предметов. </summary>
        private IItemDropService _itemDropService;

        /// <summary> Статы игрока. </summary>
        private PlayerStats _playerStats;

        /// <summary> Передвижение игрока. </summary>
        private PlayerMover _playerMover;

        /// <summary> Занят ли игрок? </summary>
        private bool _isBusy;

        /// <summary> Взаимодействие игрока с объектами. </summary>
        private InteractionController _interactionController;

        /// <summary> Аниматор игрока. </summary>
        private Animator _animator; // TODO: DELETE

        /// <summary> Сигнальная шина Zenject. </summary>
        private SignalBus _signalBus;

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="signalBus"> Шина сигналов Zenject для отправки событий. </param>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="playerStats"> Статы игрока. </param>
        /// <param name="toolbarView"> Панель быстрого доступа игрока. </param>
        /// <param name="itemDropService"> Сервис выброса предметов из инвентаря. </param>
        /// <param name="toolUsageService"> Сервис использования инструментов. </param>
        [Inject]
        private void Construct(SignalBus signalBus, Inventory inventory, PlayerStats playerStats,
            ToolbarView toolbarView, IItemDropService itemDropService, ToolUsageService toolUsageService)
        {
            _signalBus = signalBus;

            _playerInventory = inventory;
            _playerStats = playerStats;

            _toolbarView = toolbarView;
            _itemDropService = itemDropService;
        }

        /// <summary> Инициализация компонентов. </summary>
        private void Awake()
        {
            _playerMover = GetComponent<PlayerMover>();
            _animator = GetComponent<Animator>();

            _interactionController = GetComponentInChildren<InteractionController>();
            _interactionController.StartInteractionAction = () => SetBusyState(true);
            _interactionController.EndInteractionAction = () => SetBusyState(false);

            PlayerModel.SetPositionProvider(() => transform.position);
        }

        /// <summary> Обновление состояния игрока. </summary>
        private void Update()
        {
            if (WorldTime.IsPaused) return;

            HandleInput();

            if (InteractWithComponent()) return; // TODO: Удалить

            CursorController.SetCursor(CursorType.Default);
        }

        /// <summary> Обработка пользовательского ввода. </summary>
        private void HandleInput()
        {
            HandleToolbarSelection();
            HandleCurrentItemDrop();
            HandleMovement();
        }

        /// <summary> Взаимодействие с компонентами через курсор. </summary>
        /// <returns> True, если взаимодействие было обработано. </returns>
        private bool InteractWithComponent()
        {
            var hits = PhysicsUtils.SphereCastAllSorted(CameraUtils.GetMouseRay()); // TODO: Вынести отсюда
            foreach (var hit in hits)
            {
                var cursorInteractables = hit.transform.GetComponents<ICursorInteractable>();
                foreach (var cursorInteractable in cursorInteractables)
                    if (cursorInteractable.TryInteractWithCursor(this))
                    {
                        CursorController.SetCursor(cursorInteractable.CursorType);
                        return true;
                    }
            }

            return false;
        }

        /// <summary> Обработка выбора предмета на панели быстрого доступа. </summary>
        private void HandleToolbarSelection()
        {
            if (_isBusy) return;

            const int ToolbarItemsCount = 10;
            for (int i = 0; i < ToolbarItemsCount; i++)
            {
                var key = i == 9 ? KeyCode.Alpha0 : KeyCode.Alpha1 + i;
                if (!Input.GetKeyDown(key)) continue;

                _signalBus.Fire(new ToolbarHotkeyPressedSignal(i));
                break;
            }
        }

        /// <summary> Обработка выброса предмета из панели быстрого доступа. </summary>
        private void HandleCurrentItemDrop()
        {
            if (!InputWrapper.GetButtonDown(InputButton.DropCurrentItem) || !_toolbarView.SelectedItem.CanBeDropped)
                return;

            const float DropItemForce = 2.5f;
            _itemDropService.DropFromInventory(_playerInventory, _toolbarView.SelectedItemIndex,
                _dropPoint.transform.position, _dropPoint.forward * DropItemForce);
        }

        /// <summary> Обработка передвижения. </summary>
        private void HandleMovement()
        {
            var direction = new Vector3(
                InputWrapper.GetAxisRaw(InputButton.Horizontal),
                0,
                InputWrapper.GetAxisRaw(InputButton.Vertical)
            ).normalized;

            _playerMover.SetMoveDirection(direction);
            if (direction != Vector3.zero) _playerMover.SetLookRotation(direction);
        }

        /// <summary> Задать состояние занятости игрока. </summary>
        /// <param name="isBusy"> Состояние. </param>
        /// <remarks> Когда игрок занят - не может использовать инструмент или взаимодействовать с окружением. </remarks>
        public void SetBusyState(bool isBusy)
        {
            _isBusy = isBusy;
            _toolbarView.IsInteractable = !isBusy;
            if (isBusy)
                InputWrapper.BlockAllInput();
            else
                InputWrapper.UnblockAllInput();
        }

        /// <summary> Завершение взаимодействия. </summary>
        /// <remarks> Метод подписан на событие в анимации игрока (Gather_interaction). </remarks>
        private void EndInteraction() => _interactionController.EndInteraction();

        /// <summary> Запустить анимацию. </summary>
        /// <param name="animationType"> Тип проигрываемой анимации.</param>
        public void TriggerAnimation(AnimationType animationType)
        {
            string animationName = animationType.ToString();
            _animator.SetTrigger(Animator.StringToHash(animationName));
        }

        /// <summary> Запустить анимацию. </summary>
        /// <param name="animationName"> Тип проигрываемой анимации.</param>
        public void TriggerAnimation(string animationName) =>
            _animator.SetTrigger(Animator.StringToHash(animationName));

        /// <summary> Повернуть игрока в направлении указанной позиции. </summary>
        /// <param name="position"> Позиция для поворота. </param>
        public void RotateTowards(Vector3 position)
        {
            var direction = (position - transform.position).normalized;
            direction.y = 0;
            _playerMover.SetLookRotation(direction);
        }

        /// <summary> Обновить позицию и направление взгляда игрока после телепортации. </summary>
        /// <param name="newTransform"> Трансформ игрока. </param>
        public void UpdatePosition(Transform newTransform)
        {
            SetPosition(newTransform.position);
            _playerMover.SetLookRotation(newTransform.forward);
        }

        /// <summary> Установить позицию игрока. </summary>
        /// <param name="newPosition"> Позиция игрока, которую нужно установить. </param>
        public void SetPosition(Vector3 newPosition) => _playerMover.SetPosition(newPosition);

        /// <summary> Восстановить статы игрока после сна. </summary>
        /// <param name="isExhausted"> Истощенный сон или нет? </param>
        public void RestoreStatsAfterSleep(bool isExhausted = false)
        {
            const float StaminaRestoreMultiplier = 0.75f;

            var health = _playerStats.GetStat<Health>();
            health.RestoreFull();

            var stamina = _playerStats.GetStat<Stamina>();
            stamina.RestorePercent(isExhausted ? StaminaRestoreMultiplier : 1f);
        }
    }
}