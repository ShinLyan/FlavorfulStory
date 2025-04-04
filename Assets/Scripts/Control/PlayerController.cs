﻿using System;
using FlavorfulStory.Actions;
using FlavorfulStory.Actions.Interactables;
using FlavorfulStory.InputSystem;
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

        /// <summary> Занят ли игрок? </summary>2
        private bool _isBusy;

        /// <summary> Панель быстрого доступа, содержащая инвентарь игрока. </summary>
        [Tooltip("Панель быстрого доступа, содержащая инвентарь игрока."), SerializeField]
        private Toolbar _toolbar;

        /// <summary> Передвижение игрока. </summary>
        private PlayerMover _playerMover;

        /// <summary> Аниматор игрока. </summary>
        private Animator _animator;

        /// <summary> Взаимодействие игрока с объектами. </summary>
        private InteractFeature _interactFeature;

        /// <summary> Обработчик инструмента. </summary>
        private ToolHandler _toolHandler;

        #region Tools

        [SerializeField, Range(1f, 5f)] private float _toolCooldown = 1.5f;

        /// <summary> Таймер перезарядки использования инструмента. </summary>
        private float _toolCooldownTimer;

        /// <summary> Можно ли использовать инструмент? </summary>
        private bool CanUseTool => _toolCooldownTimer <= 0f;

        /// <summary> Заблокировано ли использование предмета? </summary>
        private bool IsToolUseBlocked => !CanUseTool || _isBusy || EventSystem.current.IsPointerOverGameObject();

        #endregion

        /// <summary> Текущий выбранный предмет из панели быстрого доступа. </summary>
        private InventoryItem CurrentItem => _toolbar?.SelectedItem;

        /// <summary> Событие, вызываемое при окончании взаимодействия с объектом. </summary>
        public event Action OnInteractionEnded;

        #endregion

        /// <summary> Инициализация компонентов. </summary>
        private void Awake()
        {
            _playerMover = GetComponent<PlayerMover>();
            _animator = GetComponent<Animator>();
            _interactFeature = GetComponentInChildren<InteractFeature>();
            _interactFeature.SetInteractionActions(() => SetBusyState(true), () => SetBusyState(false));
            _toolHandler = GetComponent<ToolHandler>();
            _toolHandler.SetUnequipAction(() => SetBusyState(false));
        }

        /// <summary> Обновление состояния игрока. </summary>
        private void Update()
        {
            HandleInput();
            if (_toolCooldownTimer > 0f) _toolCooldownTimer -= Time.deltaTime;
        }

        /// <summary> Обработка пользовательского ввода. </summary>
        private void HandleInput()
        {
            HandleToolbarSelectionInput();
            HandleToolbarUseInput();
            HandleMovementInput();
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
            _toolCooldownTimer = _toolCooldown;
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
        private void SetBusyState(bool state)
        {
            _isBusy = state;
            _interactFeature.SetInteractionState(state);
            _toolbar.SetInteractableState(!state);
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
        private void EndInteraction() => OnInteractionEnded?.Invoke();

        /// <summary> Запуск анимации. </summary>
        /// <param name="animationName"> Название анимации. </param>
        public void TriggerAnimation(string animationName) => _animator.SetTrigger(animationName);

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