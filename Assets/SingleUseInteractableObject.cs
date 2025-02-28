using UnityEngine;

public class SingleUseInteractableObject : AbstractInteractableObject
{
    [SerializeField] private float _destroyDelay;
    
    public override void Interact()
    {
        base.Interact();
        Destroy(gameObject, _destroyDelay);
    }
}