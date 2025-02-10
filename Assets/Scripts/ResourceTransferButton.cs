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

    private Action _clickAction;

    public void SetAction(Action clickAction) => _clickAction = clickAction;
    
    public void SetResource(InventoryItem resource) => _resource = resource;
    
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

        _clickAction?.Invoke();
    }
}