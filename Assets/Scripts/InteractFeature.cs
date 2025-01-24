using System.Collections.Generic;
using FlavorfulStory.InputSystem;
using JetBrains.Annotations;
using UnityEngine;

public class InteractFeature : MonoBehaviour
{
    [SerializeField] private InteractableObjectToolTip _interactableObjectTooltip;

    private readonly List<IInteractable> _reachableInteractables = new();

    [CanBeNull] private IInteractable _nearestAllowedInteractable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            _reachableInteractables.Add(interactable);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        _nearestAllowedInteractable = GetNearestAllowedInteractable();

        _interactableObjectTooltip.gameObject.SetActive(_nearestAllowedInteractable != null);

        if (_nearestAllowedInteractable == null) return;
        
        _interactableObjectTooltip.SetTitleAndDescription(_nearestAllowedInteractable);
        _interactableObjectTooltip.SetPositionWithOffset(_nearestAllowedInteractable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            _reachableInteractables.Remove(interactable);
        }
    }

    private void Update()
    {
        if (InputWrapper.GetButtonDown(InputButton.Interact))
        {
            _nearestAllowedInteractable?.Interact();
        }
    }

    [CanBeNull]
    private IInteractable GetNearestAllowedInteractable()
    {
        IInteractable result = null;
        foreach (var interactable in _reachableInteractables)
        {
            if (result == null && interactable.IsInteractionAllowed())
            {
                result = interactable;
                continue;
            }

            if (interactable.GetDistanceTo(transform) < result?.GetDistanceTo(transform) &&
                interactable.IsInteractionAllowed())
            {
                result = interactable;
            }
        }
        return result;
    }
}