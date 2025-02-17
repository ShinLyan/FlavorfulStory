using System;
using System.Collections.Generic;
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
    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary> Панель быстрого доступа. </summary>
        [SerializeField] private Toolbar _toolbar;

        /// <summary> Время перезарядки использования инструмента. </summary>
        [SerializeField] private float _toolCooldown = 1f;

        /// <summary> Передвижение игрока. </summary>
        private PlayerMover _playerMover;

        /// <summary> Аниматор игрока. </summary>
        private Animator _animator;

        #region Tools

        /// <summary> Массив привязок типов инструментов к их префабам. </summary>
        [SerializeField] private List<ToolPrefabMapping> _toolMappings;

        /// <summary> Точка, где появляется инструмент в руке. </summary>
        [SerializeField] private Transform _toolHolder;

        /// <summary> Текущий экипированный инструмент. </summary>
        private GameObject _currentTool;

        /// <summary> Таймер для отслеживания перезарядки инструмента. </summary>
        private float _toolCooldownTimer;

        /// <summary> Можно ли использовать инструмент? </summary>
        public bool CanUseTool => _toolCooldownTimer <= 0f;

        #endregion

        /// <summary> Текущий выбранный предмет из панели быстрого доступа. </summary>
        public InventoryItem CurrentItem => _toolbar.SelectedItem;

        /// <summary> Событие, вызываемое при окончании взаимодейтсвия. </summary>
        /// <remarks> Событие срабатывает внутри метода EndInteraction(). </remarks>
        public event Action OnInteractionEnded;

        /// <summary> Инициализация необходимых компонентов. </summary>
        private void Awake()
        {
            _playerMover = GetComponent<PlayerMover>();
            _animator = GetComponent<Animator>();
        }

        /// <summary> Выполнение различных действий в зависимости от состояния. </summary>
        private void Update()
        {
            InteractSpecialAbilityKeys();
            InteractWithMovement();

            UpdateTimers();
        }

        /// <summary> Взаимодействовать со специальными клавишами. </summary>
        private void InteractSpecialAbilityKeys()
        {
            SelectToolbarItem();
            UseToolbarItem();
        }

        /// <summary> Выбор предмета на панели быстрого доступа. </summary>
        private void SelectToolbarItem()
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

        /// <summary> Использовать предмет из панели быстрого доступа. </summary>
        /// <remarks> Если предмет расходуемый, то один экземпляр будет уничтожен. </remarks>
        private void UseToolbarItem()
        {
            if (_toolbar && CurrentItem is ActionItem actionItem &&
                (Input.GetMouseButton(0) || Input.GetMouseButton(1)) &&
                CanUseTool && !EventSystem.current.IsPointerOverGameObject())
            {
                var actionType = Input.GetMouseButton(0) ? UseActionType.LeftClick : UseActionType.RightClick;
                if (actionItem.UseActionType == actionType)
                {
                    actionItem.Use(this);
                    _toolCooldownTimer = _toolCooldown;
                    InputWrapper.BlockInput(new[] { InputButton.Horizontal, InputButton.Vertical });
                }

                if (actionItem.IsConsumable)
                {
                    Inventory.PlayerInventory.RemoveFromSlot(_toolbar.SelectedItemIndex, 1);
                    print($"{CurrentItem} потратился");
                }
            }

            if (CanUseTool) InputWrapper.UnblockInput(new[] { InputButton.Horizontal, InputButton.Vertical });
        }

        /// <summary> Обработка ввода. Передача ввода в PlayerMover. </summary>
        private void InteractWithMovement()
        {
            float x = InputWrapper.GetAxisRaw(InputButton.Horizontal),
                z = InputWrapper.GetAxisRaw(InputButton.Vertical);
            var direction = new Vector3(x, 0, z).normalized;
            _playerMover.SetMoveDirection(direction);

            if (direction != Vector3.zero)
            {
                _playerMover.SetLookRotation(direction);
            }
        }

        /// <summary> Обновить таймеры. </summary>
        private void UpdateTimers()
        {
            if (_toolCooldownTimer > 0f) _toolCooldownTimer -= Time.deltaTime;
        }

        /// <summary> Закончить взаимодействие. </summary>
        /// <remarks> Метод подписан на событие в анимации игрока (Gather_interaction). </remarks>
        private void EndInteraction() => OnInteractionEnded?.Invoke();

        /// <summary> Запуск анимации. </summary>
        /// <param name="animationName"> Имя анимации. </param>
        public void TriggerAnimation(string animationName) => _animator.SetTrigger(animationName);

        /// <summary> Получить позицию курсора. </summary>
        /// <returns> Возвращает позицию курсора. </returns>
        public static Vector3 GetCursorPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(InputWrapper.GetMousePosition());
            return Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) ? hit.point : Vector3.zero;
        }

        /// <summary> Повернуть игрока в направлении указанной позиции. </summary>
        /// <param name="position"> Позиция для поворота. </param>
        public void RotateTowards(Vector3 position)
        {
            Vector3 direction = (position - transform.position).normalized;
            direction.y = 0; // Игнорируем вертикальную составляющую
            _playerMover.SetLookRotation(direction);
        }

        /// <summary> Экипировать инструмент в руку игрока. </summary>
        /// <param name="tool"> Инструмент, который должен быть экипирован. </param>
        public void EquipTool(Tool tool)
        {
            foreach (var mapping in _toolMappings)
            {
                if (mapping.ToolType == tool.ToolType && !_currentTool)
                {
                    _currentTool = Instantiate(mapping.ToolPrefab, _toolHolder);
                    Debug.Log("EquipTool");
                    return;
                }
            }
        }

        /// <summary> Убрать инструмент из руки игрока. </summary>
        public void UnequipTool()
        {
            if (!_currentTool) return;

            Destroy(_currentTool);
            _currentTool = null;
            
            Debug.Log("UnequipTool");
        }
        
        /// <summary> Запланировать удаление инструмента после завершения анимации. </summary>
        public void ScheduleUnequipTool()
        {
            StartCoroutine(WaitForAnimationAndUnequip());
        }

        /// <summary> Корутина для ожидания завершения анимации и удаления инструмента. </summary>
        private System.Collections.IEnumerator WaitForAnimationAndUnequip()
        {
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            UnequipTool();
        }
    }
}