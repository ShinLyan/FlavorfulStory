using UnityEngine;

public interface IInteractable : ITooltipable
{
    public bool IsInteractionAllowed();
    
    public void Interact();

    public float GetDistanceTo(Transform otherTransform);
}