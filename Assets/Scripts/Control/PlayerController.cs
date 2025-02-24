using System;
using System.Linq;
using FlavorfulStory.Actions;
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
    [RequireComponent(typeof(PlayerMover)), RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields

        /// <summary> Панель быстрого доступа. </summary>
        [SerializeField] private Toolbar _toolbar;

        /// <summary> Время перезарядки использования инструмента. </summary>
        [SerializeField] private float _toolCooldown = 1f;

        /// <summary> Передвижение игрока. </summary>
        private PlayerMover _playerMover;

        /// <summary> Аниматор игрока. </summary>
        private Animator _animator;

        #region Tools

        /// <summary> Соответствия типов инструментов и их префабов. </summary>
        [SerializeField] private ToolPrefabMapping[] _toolMappings;

        /// <summary> Текущий экипированный инструмент. </summary>
        private GameObject _currentTool;

        /// <summary> Таймер перезарядки использования инструмента. </summary>
        private float _toolCooldownTimer;

        /// <summary> Можно ли использовать инструмент? </summary>
        public bool CanUseTool => _toolCooldownTimer <= 0f;

        #endregion

        /// <summary> Текущий выбранный предмет из панели быстрого доступа. </summary>
        public InventoryItem CurrentItem => _toolbar?.SelectedItem;

        /// <summary> Событие, вызываемое при окончании взаимодейтсвия. </summary>
        /// <remarks> Событие срабатывает внутри метода EndInteraction(). </remarks>
        public event Action OnInteractionEnded;

        #endregion

        /// <summary> Инициализация компонентов. </summary>
        private void Awake()
        {
            _playerMover = GetComponent<PlayerMover>();
            _animator = GetComponent<Animator>();
        }

        /// <summary> Обновление состояния игрока. </summary>
        private void Update()
        {
            HandleInput();
            ReduceCooldownTimer();
        }

        /// <summary> Обработка ввода. </summary>
        private void HandleInput()
        {
            HandleToolbarSelection();
            HandleToolbarUsage();
            HandleMovement();
        }

        /// <summary> Обработка выбора предмета на панели быстрого доступа. </summary>
        private void HandleToolbarSelection()
        {
            const int ToolbarItemsCount = 9;
            for (int i = 0; i < ToolbarItemsCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    _toolbar.SelectItem(i);
                }
            }
        }

        /// <summary> Обработка использования предмета из панели быстрого доступа. </summary>
        private void HandleToolbarUsage()
        {
            if (!CanUseTool || CurrentItem is not ActionItem actionItem ||
                EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButton(0) && actionItem.UseActionType == UseActionType.LeftClick ||
                Input.GetMouseButton(1) && actionItem.UseActionType == UseActionType.RightClick)
            {
                actionItem.Use(this);
                _toolCooldownTimer = _toolCooldown;
                InputWrapper.BlockPlayerMovement();

                if (actionItem.IsConsumable)
                    Inventory.PlayerInventory.RemoveFromSlot(_toolbar.SelectedItemIndex, 1);
            }

            if (!CanUseTool) return;

            UnequipTool();
            InputWrapper.UnblockPlayerMovement();
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
            if (direction != Vector3.zero)
            {
                _playerMover.SetLookRotation(Quaternion.LookRotation(direction));
            }
        }

        /// <summary> Уменьшение таймера перезарядки. </summary>
        private void ReduceCooldownTimer()
        {
            if (_toolCooldownTimer > 0f) _toolCooldownTimer -= Time.deltaTime;
        }

        /// <summary> Завершение взаимодействия. </summary>
        /// <remarks> Метод подписан на событие в анимации игрока (Gather_interaction). </remarks>
        private void EndInteraction() => OnInteractionEnded?.Invoke();

        /// <summary> Запуск анимации. </summary>
        /// <param name="animationName"> Название анимации. </param>
        public void TriggerAnimation(string animationName) => _animator.SetTrigger(animationName);

        /// <summary> Получение позиции курсора. </summary>
        /// <returns> Позиция точки, в которую направлен курсор. </returns>
        public static Vector3 GetCursorPosition()
        {
            var ray = Camera.main.ScreenPointToRay(InputWrapper.GetMousePosition());
            return Physics.Raycast(ray, out var hit, Mathf.Infinity) ? hit.point : Vector3.zero;
        }

        /// <summary> Повернуть игрока в направлении указанной позиции. </summary>
        /// <param name="position"> Позиция для поворота. </param>
        public void RotateTowards(Vector3 position)
        {
            var direction = (position - transform.position).normalized;
            direction.y = 0; // Игнорируем вертикальную составляющую
            _playerMover.SetLookRotation(Quaternion.LookRotation(direction));
        }

        /// <summary> Экипировать инструмент в руку игрока. </summary>
        /// <param name="tool"> Инструмент, который должен быть экипирован. </param>
        public void EquipTool(Tool tool)
        {
            if (_currentTool) return;

            var mapping = _toolMappings.FirstOrDefault(m => m.ToolType == tool.ToolType);
            if (mapping == null) return;

            _currentTool = mapping.ToolPrefab;
            _currentTool.SetActive(true);
        }

        /// <summary> Убрать инструмент из руки игрока. </summary>
        private void UnequipTool()
        {
            if (!_currentTool) return;

            _currentTool.SetActive(false);
            _currentTool = null;
        }
    }
}