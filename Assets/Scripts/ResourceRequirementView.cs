using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FlavorfulStory.InventorySystem;
using JetBrains.Annotations;

public class ResourceRequirementView : MonoBehaviour
{
    [SerializeField] private Image _resourceIcon;
    [SerializeField] private TMP_Text _quantityText;
    
    //TODO: Можно кликать только, если требования удовлетворены. Иначе - блокать
    [SerializeField] private ResourceTransferButton _addResourceButton;
    
    //TODO: Можно кликать только, если требования удовлетворены. Иначе - блокать
    [SerializeField] private ResourceTransferButton _returnResourceButton;

    [CanBeNull] private InventoryItem _resource;
    public event Action<InventoryItem, ResourceTransferButtonType> OnResourceTransferButtonClick;

    private void OnEnable()
    {
        _addResourceButton.OnClick += OnResourceTransferButtonClick;
        _returnResourceButton.OnClick += OnResourceTransferButtonClick;
    }

    private void OnDisable()
    {
        _addResourceButton.OnClick -= OnResourceTransferButtonClick;
        _returnResourceButton.OnClick -= OnResourceTransferButtonClick;
    }

    public void SetResource(InventoryItem resource)
    {
        _resource = resource;
        _addResourceButton.SetResource(_resource);
        _returnResourceButton.SetResource(_resource);
    }

    public void UpdateResourceIcon()
    {
        _resourceIcon.sprite = _resource?.Icon;
    }
    
    public void SetQuantityText(int currentQuantity, int requiredQuantity)
    {
        _quantityText.text = $"{currentQuantity}/{requiredQuantity}";
    }

    public void UpdateTransferButtonsInteractableState(int requirementNumber, int investedNumber)
    {
        _addResourceButton.IsInteractable = (investedNumber != requirementNumber && Inventory.PlayerInventory.GetItemNumber(_resource) > 0);
        _returnResourceButton.IsInteractable = !(investedNumber == 0 || !Inventory.PlayerInventory.HasSpaceFor(_resource));
    }
}