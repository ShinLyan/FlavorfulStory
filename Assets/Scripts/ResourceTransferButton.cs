using System;
using UnityEngine;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.UI;
using JetBrains.Annotations;

public class ResourceTransferButton : CustomButton
{
    //Назначается один раз в инспекторе
    [SerializeField] private ResourceTransferButtonType _buttonType;

    [CanBeNull] private InventoryItem _resource;

    public event Action<InventoryItem, ResourceTransferButtonType> OnClick;

    public void SetResource(InventoryItem resource)
    {
        _resource = resource;
    }
    
    protected override void Initialize()
    {
        //TODO: Добавить проверку _clickAction + LogError();
    }

    protected override void HoverStart()
    {
    }

    protected override void HoverEnd()
    {
    }

    protected override void Click()
    {
        if (_resource == null) Debug.LogError("Не назначен ресурс (Ремонт зданий/Требование)!");
        
        OnClick?.Invoke(_resource, _buttonType);
    }

    protected override void OnInteractionEnabled() {}

    protected override void OnInteractionDisabled() {}

    public void TriggerClick()
    {
        Click();
    }
}