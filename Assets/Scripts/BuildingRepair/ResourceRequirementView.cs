using System;
using FlavorfulStory.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Отображение информации о требуемых ресурсах для текущей стадии ремонта. </summary>
    /// <remarks> Управляет визуализацией и взаимодействием с кнопками добавления и возврата ресурса. </remarks>
    public class ResourceRequirementView : MonoBehaviour
    {
        /// <summary> Иконка ресурса, отображаемая на панели. </summary>
        [SerializeField] private Image _resourceIcon;

        /// <summary> Текстовое поле, отображающее количество текущего ресурса
        /// и необходимое для завершения стадии. </summary>
        [SerializeField] private TMP_Text _quantityText;

        /// <summary> Кнопка добавления ресурса в процесс ремонта. </summary>
        [SerializeField] private ResourceTransferButton _addResourceButton;

        /// <summary> Кнопка возврата ресурса в инвентарь. </summary>
        [SerializeField] private ResourceTransferButton _returnResourceButton;

        /// <summary> Текущий ресурс, с которым работает этот элемент. </summary>
        private InventoryItem _resource;

        /// <summary> Требуемое количество ресурса. </summary>
        private int _requiredQuantity;

        /// <summary> Вложенное количество ресурса. </summary>
        private int _investedQuantity;

        /// <summary> Событие, которое вызывается при клике на одну из кнопок
        /// добавления или возврата ресурса. </summary>
        public event Action<InventoryItem, ResourceTransferButtonType> OnResourceTransferButtonClick;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Внедрение зависимости — инвентарь игрока. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        [Inject]
        private void Construct(Inventory inventory) => _playerInventory = inventory;

        /// <summary> Подписаться на события при активации объекта. </summary>
        private void OnEnable()
        {
            _addResourceButton.OnClick += OnResourceTransferButtonClick;
            _returnResourceButton.OnClick += OnResourceTransferButtonClick;
        }

        /// <summary> Отписаться от событий при деактивации объекта. </summary>
        private void OnDisable()
        {
            _addResourceButton.OnClick -= OnResourceTransferButtonClick;
            _returnResourceButton.OnClick -= OnResourceTransferButtonClick;
        }

        /// <summary> Установить ресурс для текущего элемента. </summary>
        /// <param name="requirement"> Ресурсное требование. </param>
        /// <param name="investedNumber"> Количество вложенного ресурса в процесс ремонта. </param>
        public void Setup(ItemRequirement requirement, int investedNumber)
        {
            if (!requirement.Item)
            {
                Debug.LogError("ResourceRequirementView: item is null in requirement.");
                return;
            }

            InitializeResourceData(requirement.Item, requirement.Quantity, investedNumber);
            UpdateView();
            UpdateButtonsStates();
        }

        /// <summary> Инициализировать данные о ресурсе. </summary>
        /// <param name="resource"> Ресурс. </param>
        /// <param name="requiredQuantity"> Требуемое количество ресурса. </param>
        /// <param name="investedQuantity"> Вложенное количество ресурса. </param>
        private void InitializeResourceData(InventoryItem resource, int requiredQuantity, int investedQuantity)
        {
            _resource = resource;
            _requiredQuantity = requiredQuantity;
            _investedQuantity = investedQuantity;

            _addResourceButton.SetResource(_resource);
            _returnResourceButton.SetResource(_resource);
        }

        /// <summary> Обновить визуальное отображение информации о ресурсе. </summary>
        private void UpdateView()
        {
            _resourceIcon.sprite = _resource.Icon;
            _quantityText.text = $"{_investedQuantity}/{_requiredQuantity}";
        }

        /// <summary> Обновить состояние кнопок добавления и возврата ресурса. </summary>
        private void UpdateButtonsStates()
        {
            _addResourceButton.Interactable =
                _investedQuantity < _requiredQuantity && _playerInventory.GetItemNumber(_resource) > 0;
            _returnResourceButton.Interactable = _investedQuantity > 0 && _playerInventory.HasSpaceFor(_resource);
        }
    }
}