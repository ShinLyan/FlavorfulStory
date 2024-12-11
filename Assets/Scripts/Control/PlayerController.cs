using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI;
using FlavorfulStory.Movement;
using UnityEngine;

namespace FlavorfulStory.Control
{
    /// <summary> Контроллер игрока.</summary>
    [RequireComponent(typeof(PlayerMover))]
    public class PlayerController : MonoBehaviour
    {
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
            InteractWithMovement();
            InteractSpecialAbilityKeys();
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
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    _toolbar.SelectItem(i);
                }
            }
        }

        private void UseToolbarItem()
        {
            if (Input.GetMouseButtonDown(1))
            {
                print("Input.GetMouseButtonDown(1)");
                _toolbar.TryUseSelectedItem();
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