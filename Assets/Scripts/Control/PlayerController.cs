using FlavorfulStory.Actions.ActionItems;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Movement;
using UnityEngine;

namespace FlavorfulStory.Control
{
    /// <summary> Контроллер игрока.</summary>
    [RequireComponent(typeof(PlayerMover))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private Toolbar _toolbar;

        /// <summary> Передвижение игрока.</summary>
        private PlayerMover _playerMover;

        /// <summary> Инициализация полей.</summary>
        private void Awake()
        {
            _playerMover = GetComponent<PlayerMover>();
        }

        /// <summary> Выполнение различных действий в зависимости от состояния.</summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<ItemDropper>().DropItem(InventoryItem.GetItemFromID("f5d03d4d-c544-4e51-8921-d219a76ce08f"), 1);
            }

            InteractSpecialAbilityKeys();
            InteractWithMovement();
        }

        private void InteractWithMovement()
        {
            float x = Input.GetAxis("Horizontal"), z = Input.GetAxis("Vertical");
            var direction = new Vector3(x, 0, z).normalized;
            _playerMover.MoveAndRotate(direction);
        }

        /// <summary> Взаимодействовать со специальными клавишами.</summary>
        private void InteractSpecialAbilityKeys()
        {
            SelectToolbarItem();
            UseToolbarItem();
        }

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

        /// <summary> Использовать предмет в панели быстрого доступа.</summary>
        /// <remarks> Если предмет расходуемый, то один экземпляр будет уничтожен.</remarks>
        private void UseToolbarItem()
        {
            if (_toolbar && _toolbar.SelectedItem is ActionItem actionItem &&
                (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
            {
                var actionType = Input.GetMouseButtonDown(0) ? 
                    UseActionType.LeftClick : UseActionType.RightClick;
                if (actionItem.UseActionType == actionType)
                {
                    actionItem.Use(this);
                }

                if (actionItem.IsConsumable)
                {
                    Inventory.PlayerInventory.RemoveFromSlot(_toolbar.SelectedItemIndex, 1);
                    print($"{_toolbar.SelectedItem} потратился");
                }
            }
        }

        /// <summary> Переключение контроллера игрока.</summary>
        /// <remarks> Используется чтобы игрок не двигался до загрузки другой сцены.</remarks> 
        /// <param name="enabled"> Включить / Выключить контроллер.</param>
        public static void SwitchController(bool enabled)
        {
            var playerController = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
            if (playerController) playerController.enabled = enabled;
        }
    }
}