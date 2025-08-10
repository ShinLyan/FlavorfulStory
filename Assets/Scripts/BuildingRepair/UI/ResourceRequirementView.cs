using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.BuildingRepair.UI
{
    /// <summary> Отображение информации о требуемых ресурсах для текущей стадии ремонта. </summary>
    /// <remarks> Управляет визуализацией и взаимодействием с кнопками добавления и возврата ресурса. </remarks>
    public class ResourceRequirementView : MonoBehaviour
    {
        /// <summary> Иконка ресурса, отображаемая на панели. </summary>
        [SerializeField] private RepairResourceSlotView _resourceSlotView;

        /// <summary> Текстовое поле, отображающее количество текущего ресурса
        /// и необходимое для завершения стадии. </summary>
        [SerializeField] private TMP_Text _quantityText;

        /// <summary> Кнопка добавления ресурса в процесс ремонта. </summary>
        [SerializeField] private Button _addResourceButton;

        /// <summary> Кнопка возврата ресурса в инвентарь. </summary>
        [SerializeField] private Button _returnResourceButton;

        /// <summary> Текущий ресурс, с которым работает этот элемент. </summary>
        private InventoryItem _resource;

        /// <summary> Требуемое количество ресурса. </summary>
        private int _requiredQuantity;

        /// <summary> Вложенное количество ресурса. </summary>
        private int _investedQuantity;

        /// <summary> Событие, вызываемое при нажатии кнопки добавления ресурса. </summary>
        public event Action<InventoryItem> OnAddClicked;

        /// <summary> Событие, вызываемое при нажатии кнопки возврата ресурса. </summary>
        public event Action<InventoryItem> OnReturnClicked;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Внедрение зависимости — инвентарь игрока. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(Inventory inventory) => _playerInventory = inventory;

        /// <summary> Подписаться на события при активации объекта. </summary>
        private void OnEnable()
        {
            _addResourceButton.onClick.AddListener(() => OnAddClicked?.Invoke(_resource));
            _returnResourceButton.onClick.AddListener(() => OnReturnClicked?.Invoke(_resource));
        }

        /// <summary> Отписаться от событий при деактивации объекта. </summary>
        private void OnDisable()
        {
            _addResourceButton.onClick.RemoveAllListeners();
            _returnResourceButton.onClick.RemoveAllListeners();
        }

        /// <summary> Установить ресурс для текущего элемента. </summary>
        /// <param name="requirement"> Ресурсное требование. </param>
        /// <param name="investedNumber"> Количество вложенного ресурса в процесс ремонта. </param>
        public void Setup(ItemStack requirement, int investedNumber)
        {
            InitializeResourceData(requirement.Item, requirement.Number, investedNumber);
            UpdateView();
            UpdateButtonsStates();
        }

        /// <summary> Инициализировать данные о ресурсе. </summary>
        /// <param name="resource"> Ресурс. </param>
        /// <param name="requiredNumber"> Требуемое количество ресурса. </param>
        /// <param name="investedNumber"> Вложенное количество ресурса. </param>
        private void InitializeResourceData(InventoryItem resource, int requiredNumber, int investedNumber)
        {
            _resource = resource;
            _requiredQuantity = requiredNumber;
            _investedQuantity = investedNumber;
        }

        /// <summary> Обновить визуальное отображение информации о ресурсе. </summary>
        private void UpdateView()
        {
            _resourceSlotView.Setup(_resource);
            _quantityText.text = $"{_investedQuantity}/{_requiredQuantity}";
        }

        /// <summary> Обновить состояние кнопок добавления и возврата ресурса. </summary>
        private void UpdateButtonsStates()
        {
            _addResourceButton.interactable =
                _investedQuantity < _requiredQuantity && _playerInventory.GetItemNumber(_resource) > 0;
            _returnResourceButton.interactable = _investedQuantity > 0 && _playerInventory.HasSpaceFor(_resource);
        }
    }
}