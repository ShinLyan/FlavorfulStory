using FlavorfulStory.Actions;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Movement;
using UnityEngine;

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

        /// <summary> Таймер для отслеживания перезарядки инструмента. </summary>
        private float _toolCooldownTimer;

        /// <summary> Текущий выбранный предмет из панели быстрого доступа. </summary>
        public InventoryItem CurrentItem => _toolbar.SelectedItem;

        /// <summary> Можно ли использовать инструмент? </summary>
        public bool CanUseTool => _toolCooldownTimer <= 0f;

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
                (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && CanUseTool)
            {
                var actionType = Input.GetMouseButtonDown(0) ? UseActionType.LeftClick : UseActionType.RightClick;
                if (actionItem.UseActionType == actionType)
                {
                    actionItem.Use(this);
                    _toolCooldownTimer = _toolCooldown;
                }

                if (actionItem.IsConsumable)
                {
                    Inventory.PlayerInventory.RemoveFromSlot(_toolbar.SelectedItemIndex, 1);
                    print($"{CurrentItem} потратился");
                }
            }
        }

        /// <summary> Обработка ввода. Передача ввода в PlayerMover. </summary>
        private void InteractWithMovement()
        {
            float x = Input.GetAxisRaw("Horizontal"), z = Input.GetAxisRaw("Vertical");
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

        /// <summary> Переключение контроллера игрока. </summary>
        /// <remarks> Используется чтобы игрок не двигался до загрузки другой сцены. </remarks> 
        /// <param name="enabled"> Включить / Выключить контроллер. </param>
        public static void SwitchController(bool enabled)
        {
            // TODO: УДАЛИТЬ ЭТОТ КОСТЫЛЬ
            var playerController = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
            if (playerController) playerController.enabled = enabled;
        }

        /// <summary> Запуск анимации. </summary>
        /// <param name="animationName"> Имя анимации. </param>
        public void TriggerAnimation(string animationName) => _animator.SetTrigger(animationName);

        /// <summary> Получить позицию курсора. </summary>
        /// <returns> Возвращает позицию курсора. </returns>
        public static Vector3 GetCursorPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
    }
}