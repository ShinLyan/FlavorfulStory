using UnityEngine;

public class ReusableInteractableObject : AbstractInteractableObject
{
    [SerializeField] private GameObject _fruit;

    [SerializeField] private float _interactionCooldown;
    
    /// <summary> Флаг возможности взаимодействия с объектом. </summary>
    private bool _isInteractionAllowed = true;
    
    public override bool IsInteractionAllowed
    {
        get => _isInteractionAllowed;
        set
        {
            _isInteractionAllowed = value;
            if (_fruit != null)
                _fruit.SetActive(_isInteractionAllowed);
        }
    }
    
    public override void Interact()
    {
        base.Interact();
        StartCoroutine(EnableInteractionAfterCooldown());
    }
    
    private System.Collections.IEnumerator EnableInteractionAfterCooldown()
    {
        yield return new WaitForSeconds(_interactionCooldown);
        IsInteractionAllowed = true;
    }
}