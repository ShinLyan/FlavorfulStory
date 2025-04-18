using System;
using FlavorfulStory.Actions;
using FlavorfulStory.Control.CursorSystem;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Movement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.Control
{
    /// <summary> Контроллер игрока, отвечающий за управление,
    /// использование предметов и взаимодействие с окружением. </summary>
    [RequireComponent(typeof(PlayerMover), typeof(Animator), typeof(ToolHandler))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Панель быстрого доступа, содержащая инвентарь игрока. </summary>
        [Tooltip("Панель быстрого доступа, содержащая инвентарь игрока."), SerializeField]
        private Toolbar _toolbar;

        /// <summary> Занят ли игрок? </summary>2
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

        /// <summary>
        /// 
        /// </summary>
        private const float ToolCooldown = 1.5f;

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

        /// <summary> Инициализация компонентов. </summary>
        private void Awake()
        {
            _playerMover = GetComponent<PlayerMover>();
            _animator = GetComponent<Animator>();
            _interactionController = GetComponentInChildren<InteractionController>();
            _interactionController.SetInteractionActions(() => SetBusyState(true), () => SetBusyState(false));
            _toolHandler = GetComponent<ToolHandler>();
            _toolHandler.SetUnequipAction(() => SetBusyState(false));
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

        private bool InteractWithComponent()
        {
            var hits = SphereCastAllSorted();
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

        private static RaycastHit[] SphereCastAllSorted()
        {
            const float SphereCastRadius = 0.1f;
            var hits = Physics.SphereCastAll(GetMouseRay(), SphereCastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++) distances[i] = hits[i].distance;
            Array.Sort(distances, hits);
            return hits;
        }

        private static Camera _mainCamera;

        /// <summary> Получить луч основной камеры. </summary>
        /// <returns> Луч основной камеры. </returns>
        private static Ray GetMouseRay()
        {
            if (!_mainCamera) _mainCamera = Camera.main;
            return _mainCamera.ScreenPointToRay(InputWrapper.GetMousePosition());
        }

        /// <summary> Обработка выбора предмета на панели быстрого доступа. </summary>
        private void HandleToolbarSelectionInput()
        {
            if (_isBusy) return;

            // TODO: Переделать на количество 10
            const int ToolbarItemsCount = 9;
            for (int i = 0; i < ToolbarItemsCount; i++)
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    _toolbar.SelectItem(i);
        }

        /// <summary> Обработка использования предмета из панели быстрого доступа. </summary>
        private void HandleToolbarUseInput()
        {
            if (CurrentItem is not IUsable usable || IsToolUseBlocked) return;

            if ((!Input.GetMouseButton(0) || usable.UseActionType != UseActionType.LeftClick) &&
                (!Input.GetMouseButton(1) || usable.UseActionType != UseActionType.RightClick)) return;

            BeginInteraction(usable);
        }

        /// <summary> Начать взаимодйствие с предметом. </summary>
        /// <param name="usable"> Используемый предмет. </param>
        private void BeginInteraction(IUsable usable)
        {
            if (usable == null) return;

            StartUsingItem(usable);
            if (usable is EdibleInventoryItem) ConsumeEdibleItem();
            _toolCooldownTimer = ToolCooldown;
        }

        /// <summary> Начать использование предмета. </summary>
        /// <param name="usable"> Используемый предмет. </param>
        private void StartUsingItem(IUsable usable)
        {
            if (!usable.Use(this, _toolHandler.HitableLayers))
                return;

            _toolHandler.Equip(CurrentItem as Tool);
            SetBusyState(true);
        }

        /// <summary> Съесть используемый предмет. </summary>
        private void ConsumeEdibleItem()
        {
            Inventory.PlayerInventory.RemoveFromSlot(_toolbar.SelectedItemIndex, 1);
            InputWrapper.UnblockPlayerMovement();
            SetBusyState(false);
        }

        /// <summary> Задать состояние занятости игрока. </summary>
        /// <param name="state"> Состояние. </param>
        /// <remarks> Когда игрок занят - не может использовать инструмент или взаимодействовать с окружением. </remarks>
        public void SetBusyState(bool state)
        {
            _isBusy = state;
            _toolbar.SetInteractableState(!state);
            if (state) InputWrapper.BlockAllInput();
            else InputWrapper.UnblockAllInput();
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
    }
}