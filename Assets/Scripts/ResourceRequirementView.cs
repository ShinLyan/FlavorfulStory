using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using FlavorfulStory.InventorySystem;

/// <summary> Класс, отвечающий за отображение информации о требуемых ресурсах для текущей стадии ремонта. </summary>
/// <remarks> Управляет визуализацией и взаимодействием с кнопками добавления и возврата ресурса. </remarks>
public class ResourceRequirementView : MonoBehaviour
{
    /// <summary> Иконка ресурса, отображаемая на панели. </summary>
    [SerializeField] private Image _resourceIcon;

    /// <summary> Текстовое поле, отображающее количество текущего ресурса и необходимое для завершения стадии. </summary>
    [SerializeField] private TMP_Text _quantityText;

    /// <summary> Кнопка для добавления ресурса в процесс ремонта. </summary>
    [SerializeField] private ResourceTransferButton _addResourceButton;

    /// <summary> Кнопка для возврата ресурса в инвентарь. </summary>
    [SerializeField] private ResourceTransferButton _returnResourceButton;

    /// <summary> Текущий ресурс, с которым работает этот элемент. Может быть null, если ресурс не задан. </summary>
    [CanBeNull] private InventoryItem _resource;

    /// <summary> Событие, которое вызывается при клике на одну из кнопок добавления или возврата ресурса. </summary>
    public event Action<InventoryItem, ResourceTransferButtonType> OnResourceTransferButtonClick;

    /// <summary> Подписка на события добавления и возврата ресурсов при активации объекта. </summary>
    private void OnEnable()
    {
        _addResourceButton.OnClick += OnResourceTransferButtonClick;
        _returnResourceButton.OnClick += OnResourceTransferButtonClick;
    }

    /// <summary> Отписка от событий добавления и возврата ресурсов при деактивации объекта. </summary>
    private void OnDisable()
    {
        _addResourceButton.OnClick -= OnResourceTransferButtonClick;
        _returnResourceButton.OnClick -= OnResourceTransferButtonClick;
    }

    /// <summary> Установить ресурс для текущего элемента. </summary>
    /// <param name="resource"> Ресурс, который будет установлен для текущего элемента. </param>
    public void SetResource(InventoryItem resource)
    {
        _resource = resource;
        _addResourceButton.SetResource(_resource);
        _returnResourceButton.SetResource(_resource);
    }

    /// <summary> Обновить иконку ресурса в панели. </summary>
    public void UpdateResourceIcon()
    {
        _resourceIcon.sprite = _resource?.Icon;
    }

    /// <summary> Установить текстовое отображение количества ресурса. </summary>
    /// <param name="currentQuantity"> Текущее количество ресурса. </param>
    /// <param name="requiredQuantity"> Необходимое количество ресурса для завершения стадии ремонта. </param>
    public void SetQuantityText(int currentQuantity, int requiredQuantity)
    {
        _quantityText.text = $"{currentQuantity}/{requiredQuantity}";
    }

    /// <summary> Обновить состояние кнопок добавления и возврата ресурса. </summary>
    /// <param name="requirementNumber"> Требуемое количество ресурса. </param>
    /// <param name="investedNumber"> Количество вложенного ресурса в процесс ремонта. </param>
    public void UpdateTransferButtonsInteractableState(int requirementNumber, int investedNumber)
    {
        _addResourceButton.IsInteractable = (investedNumber != requirementNumber &&
                                             Inventory.PlayerInventory.GetItemNumber(_resource) > 0);
        _returnResourceButton.IsInteractable =
            !(investedNumber == 0 || !Inventory.PlayerInventory.HasSpaceFor(_resource));
    }
}