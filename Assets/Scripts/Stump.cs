using FlavorfulStory.Actions;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.DropSystem;
using UnityEngine;

public class Stump : MonoBehaviour, IInteractable
{
    [SerializeField] private DropItem _dropItem;
    
    private const string Name = "Wending stump";
    private const string Description = "Press E, if hungry";

    private bool _canInteract;

    private void Awake()
    {
        _canInteract = true;
    }

    public float GetDistanceTo(Transform otherTransform)
    {
        return Vector3.Distance(transform.position, otherTransform.position);
    }

    public bool IsInteractionAllowed()
    {
        return _canInteract;
    }

    public void Interact()
    {
        if (_canInteract)
        {
            Inventory.PlayerInventory.TryAddToFirstEmptySlot(_dropItem.ItemPrefab, _dropItem.Quantity);
        }
    }

    public string GetTooltipTitle() => Name;
    
    public string GetTooltipDescription() => Description;

    public Vector3 GetWorldPosition() => transform.position;
}