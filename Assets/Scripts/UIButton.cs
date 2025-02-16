using System;
using FlavorfulStory.UI;

public class UIButton : CustomButton
{
    public event Action OnClick;

    public void EnableInteraction() => IsInteractable = true;
    public void DisableInteraction() => IsInteractable = false;
    
    public void TriggerClick()
    {
        Click();
    }

    public void RemoveAllListeners() => OnClick = null;
    
    protected override void Initialize()
    {
        
    }

    protected override void HoverStart()
    {
        
    }

    protected override void HoverEnd()
    {
        
    }

    protected override void Click()
    {
        OnClick?.Invoke();
    }

    protected override void OnInteractionEnabled() {}

    protected override void OnInteractionDisabled() {}
}