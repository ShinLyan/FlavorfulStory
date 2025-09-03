using System;
using FlavorfulStory.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.BuildingRepair.UI
{
    /// <summary> Отображение информации о требуемых ресурсах для текущей стадии ремонта. </summary>
    /// <remarks> Управляет визуализацией и взаимодействием с кнопками добавления и возврата ресурса. </remarks>
    public class ResourceRequirementView : MonoBehaviour
    {
        /// <summary> Иконка ресурса, отображаемая на панели. </summary>
        [SerializeField] private ResourceSlotView _resourceSlotView;

        /// <summary> Текстовое поле, отображающее текущее количество ресурса и необходимое для завершения. </summary>
        [SerializeField] private TMP_Text _quantityText;

        /// <summary> Кнопка добавления ресурса в процесс ремонта. </summary>
        [SerializeField] private Button _addResourceButton;

        /// <summary> Кнопка возврата ресурса в инвентарь. </summary>
        [SerializeField] private Button _returnResourceButton;

        /// <summary> Текущий ресурс, с которым работает этот элемент. </summary>
        private InventoryItem _resource;

        /// <summary> Требуемое количество ресурса. </summary>
        private int _requiredAmount;

        /// <summary> Вложенное количество ресурса. </summary>
        private int _investedAmount;

        /// <summary> Событие, вызываемое при нажатии кнопки добавления ресурса. </summary>
        public event Action<InventoryItem> OnAddClicked;

        /// <summary> Событие, вызываемое при нажатии кнопки возврата ресурса. </summary>
        public event Action<InventoryItem> OnReturnClicked;

        /// <summary> Провайдер инвентарей. </summary>
        private IInventoryProvider _inventoryProvider;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="inventoryProvider"> Провайдер инвентарей. </param>
        [Inject]
        private void Construct(IInventoryProvider inventoryProvider) => _inventoryProvider = inventoryProvider;

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
        /// <param name="investedAmount"> Количество вложенного ресурса в процесс ремонта. </param>
        public void Setup(ItemStack requirement, int investedAmount)
        {
            _resource = requirement.Item;
            _requiredAmount = requirement.Number;
            _investedAmount = investedAmount;

            _resourceSlotView.Setup(_resource);

            Redraw();
        }

        /// <summary> Обновляет количество вложенного ресурса. </summary>
        /// <param name="newAmount"> Количество вложенного ресурса в процесс ремонта. </param>
        public void UpdateInvestedAmount(int newAmount)
        {
            _investedAmount = newAmount;
            Redraw();
        }

        /// <summary> Обновить визуальное отображение информации о ресурсе. </summary>
        private void Redraw()
        {
            _quantityText.text = $"{_investedAmount}/{_requiredAmount}";
            UpdateButtonsStates();
        }

        /// <summary> Обновить состояние кнопок добавления и возврата ресурса. </summary>
        private void UpdateButtonsStates()
        {
            var playerInventory = _inventoryProvider.GetPlayerInventory();
            _addResourceButton.interactable = _investedAmount < _requiredAmount &&
                                              playerInventory.GetItemNumber(_resource) > 0;
            _returnResourceButton.interactable = _investedAmount > 0 && playerInventory.HasSpaceFor(_resource);
        }
    }
}