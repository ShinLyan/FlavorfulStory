using FlavorfulStory.Actions;
using FlavorfulStory.CursorSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Stats;
using FlavorfulStory.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace FlavorfulStory.Player
{
    /// <summary> Контроллер игрока, отвечающий за управление,
    /// использование предметов и взаимодействие с окружением. </summary>
    [RequireComponent(typeof(PlayerMover), typeof(Animator), typeof(ToolHandler))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Трансформ выброса предмета. </summary>
        /// <remarks> Прокидывается в <see cref="IItemDropService"/>. </remarks>
        [SerializeField] private Transform _dropPoint;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Панель быстрого доступа. </summary>
        private Toolbar _toolbar;

        /// <summary> Сервис выброса предметов. </summary>
        private IItemDropService _itemDropService;

        /// <summary> Статы игрока. </summary>
        private PlayerStats _playerStats;

        /// <summary> Занят ли игрок? </summary>
        private bool _isBusy;

        /// <summary> Передвижение игрока. </summary>
        private PlayerMover _playerMover;

        /// <summary> Аниматор игрока. </summary>
        private Animator _animator;

        /// <summary> Взаимодействие игрока с объектами. </summary>
        private InteractionController _interactionController;

        #region Tools

        /// <summary> Обработчик инструмента. </summary>
        private ToolHandler _toolHandler;

        /// <summary> Таймер перезарядки использования инструмента. </summary>
        private float _toolCooldownTimer;

        /// <summary> Можно ли использовать инструмент? </summary>
        private bool CanUseTool => _toolCooldownTimer <= 0f;

        /// <summary> Заблокировано ли использование предмета? </summary>
        private bool IsToolUseBlocked => !CanUseTool || _isBusy || EventSystem.current.IsPointerOverGameObject();

        #endregion

        /// <summary> Текущий выбранный предмет из панели быстрого доступа. </summary>
        private InventoryItem CurrentItem => _toolbar?.SelectedItem;

        #endregion

        /// <summary> Внедрение зависимости — инвентарь игрока. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="toolbar"> </param>
        /// <param name="itemDropService"> Сервис выброса предметов в игровой мир. </param>
        [Inject]
        private void Construct(Inventory inventory, Toolbar toolbar, IItemDropService itemDropService)
        {
            _playerInventory = inventory;
            _toolbar = toolbar;
            _itemDropService = itemDropService;
        }

        /// <summary> Инициализация компонентов. </summary>
        private void Awake()
        {
            _playerStats = GetComponent<PlayerStats>();
            _playerMover = GetComponent<PlayerMover>();
            _animator = GetComponent<Animator>();

            _interactionController = GetComponentInChildren<InteractionController>();
            _interactionController.StartInteractionAction = () => SetBusyState(true);
            _interactionController.EndInteractionAction = () => SetBusyState(false);

            _toolHandler = GetComponent<ToolHandler>();
            _toolHandler.UnequipAction = () => SetBusyState(false);
            PlayerModel.SetPositionProvider(() => transform.position);
        }

        /// <summary> Обновление состояния игрока. </summary>
        private void Update()
        {
            HandleInput();
            if (_toolCooldownTimer > 0f) _toolCooldownTimer -= Time.deltaTime;
            if (InteractWithComponent()) return;
            CursorController.SetCursor(CursorType.Default);
        }

        /// <summary> Обработка пользовательского ввода. </summary>
        private void HandleInput()
        {
            HandleToolbarSelectionInput();
            HandleToolbarUseInput();
            HandleMovementInput();
        }

        /// <summary> Взаимодействие с компонентами через курсор. </summary>
        /// <returns> True, если взаимодействие было обработано. </returns>
        private bool InteractWithComponent()
        {
            var hits = PhysicsUtils.SphereCastAllSorted(CameraUtils.GetMouseRay());
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
        private void HandleToolbarSelectionInput()
        {
            if (_isBusy) return;

            const int ToolbarItemsCount = 10;
            for (int i = 0; i < ToolbarItemsCount; i++)
            {
                var key = i == 9 ? KeyCode.Alpha0 : KeyCode.Alpha1 + i;
                if (Input.GetKeyDown(key))
                {
                    _toolbar.SelectItem(i);
                    break;
                }
            }
        }

        /// <summary> Обработка доступных действий выбранного предмета из панели быстрого доступа. </summary>
        private void HandleToolbarUseInput()
        {
            if (CurrentItem == null) return;

            if (CurrentItem is IUsable usable && !IsToolUseBlocked) HandleCurrentItemUse(usable);

            if (CurrentItem.CanBeDropped && !IsToolUseBlocked) HandleCurrentItemDrop();
        }

        /// <summary> Обработка использования предмета из панели быстрого доступа. </summary>
        private void HandleCurrentItemUse(IUsable usable)
        {
            if ((Input.GetMouseButton(0) && usable.UseActionType == UseActionType.LeftClick) ||
                (Input.GetMouseButton(1) && usable.UseActionType == UseActionType.RightClick))
                BeginInteraction(usable);
        }

        /// <summary> Обработка выброса предмета из панели быстрого доступа. </summary>
        private void HandleCurrentItemDrop()
        {
            const float DropItemForce = 2.5f;
            if (InputWrapper.GetButtonDown(InputButton.DropCurrentItem))
                _itemDropService.DropFromInventory(_playerInventory, _toolbar.SelectedItemIndex,
                    _dropPoint.transform.position, _dropPoint.forward * DropItemForce);
        }

        /// <summary> Начать взаимодействие с предметом. </summary>
        /// <param name="usable"> Используемый предмет. </param>
        private void BeginInteraction(IUsable usable)
        {
            if (usable == null) return;

            // TODO: ЧТО-то непонятное тут происходит. ПОФИКСИТЬ. ПЛЮС ПОЧЕМУ-то НЕ УНИЧТОЖАЕТСЯ ХЛЕБ ЕСЛИ СЪЕСТЬ ЕГО
            StartUsingItem(usable);
            if (usable is EdibleInventoryItem) ConsumeEdibleItem();
            _toolCooldownTimer = PlayerModel.ToolCooldown;
        }

        /// <summary> Начать использование предмета. </summary>
        /// <param name="usable"> Используемый предмет. </param>
        private void StartUsingItem(IUsable usable)
        {
            if (!usable.Use(this, _toolHandler.HitableLayers)) return;

            _toolHandler.Equip(CurrentItem as Tool);
            SetBusyState(true);
        }

        /// <summary> Съесть используемый предмет. </summary>
        private void ConsumeEdibleItem()
        {
            _playerInventory.RemoveFromSlot(_toolbar.SelectedItemIndex, 1);
            InputWrapper.UnblockPlayerInput();
            SetBusyState(false);
        }

        /// <summary> Задать состояние занятости игрока. </summary>
        /// <param name="state"> Состояние. </param>
        /// <remarks> Когда игрок занят - не может использовать инструмент или взаимодействовать с окружением. </remarks>
        public void SetBusyState(bool state)
        {
            _isBusy = state;
            _toolbar.SetInteractableState(!state);
            if (state)
                InputWrapper.BlockAllInput();
            else
                InputWrapper.UnblockAllInput();
        }

        /// <summary> Обработка передвижения. </summary>
        private void HandleMovementInput()
        {
            var direction = new Vector3(
                InputWrapper.GetAxisRaw(InputButton.Horizontal),
                0,
                InputWrapper.GetAxisRaw(InputButton.Vertical)
            ).normalized;

            _playerMover.SetMoveDirection(direction);
            if (direction != Vector3.zero) _playerMover.SetLookRotation(direction);
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