using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FlavorfulStory.Actions.Interactables;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;

public class RepairableBuilding : MonoBehaviour, IInteractable
{
    [Tooltip("Стадии ремонта.")] 
    [SerializeField] private BuildingRepairConfig _repairConfig;

    private BuildingRepairView _repairView;

    private List<int> _investedResources;

    private int _currentRepairStageIndex;

    private bool _repairCompleted;
    
    private void Awake()
    {
        _repairView = FindFirstObjectByType<BuildingRepairView>(FindObjectsInactive.Include);
        _investedResources = new(_repairConfig._stages.Count);
    }

    private void Start()
    {
        IsInteractionAllowed = true;
        IsBlockingMovement = true;
        _currentRepairStageIndex = 0;
        for (int i = 0; i < _repairConfig._stages[_currentRepairStageIndex].Requirements.Count; i++)
        {
            _investedResources.Add(0);
        }
        _repairView.Initialize(TransferResource);
    }

    private void Update()
    {
        if (_repairView.IsOpen && InputWrapper.GetButtonDown(InputButton.SwitchGameMenu, true))
        {
            _repairView.Close();
        }
    }

    //TODO: Можно вызывать только, если требования удовлетворены. Иначе - блокать
    private void Build()
    {
        _repairCompleted = _currentRepairStageIndex >= _repairConfig._stages.Count - 1;
        if (!_repairCompleted)
        {
            _currentRepairStageIndex++;
            _investedResources.Clear();
            foreach (var requirement in _repairConfig._stages[_currentRepairStageIndex].Requirements)
            {
                _investedResources.Add(0);
            }
        }
        _repairView.SetData(_repairConfig._stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
        _repairView.BuildButton.IsInteractable = CanBeRepaired();
    }
    
    private void TransferResource(InventoryItem resource, ResourceTransferButtonType transferButtonType)
    {
        var requriments = _repairConfig._stages[_currentRepairStageIndex].Requirements;

        switch (transferButtonType)
        {
            case ResourceTransferButtonType.Add:
                TryAddResource(resource);
                break;
            case ResourceTransferButtonType.Return:
                TryReturnResource(resource);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(transferButtonType), transferButtonType, null);
        }

        _repairView.BuildButton.IsInteractable = CanBeRepaired();
    }

    private void TryAddResource(InventoryItem resource)
    {
        int resourceRequirementNumber = _repairConfig._stages[_currentRepairStageIndex].Requirements
            .Find(requirement => requirement.Item.ItemID == resource.ItemID).Quantity;
        
        var investedResourceIndex = _repairConfig._stages[_currentRepairStageIndex].Requirements.FindIndex(x => x.Item.ItemID == resource.ItemID);
        if (_investedResources[investedResourceIndex] < resourceRequirementNumber)
        {
            if (Inventory.PlayerInventory.HasItem(resource) && Inventory.PlayerInventory.GetItemNumber(resource) > 0)
            {
                int numberToInvest = resourceRequirementNumber - _investedResources[investedResourceIndex];
                int investedNumber = Math.Min(numberToInvest, Inventory.PlayerInventory.GetItemNumber(resource));
                _investedResources[investedResourceIndex] += investedNumber;
                Inventory.PlayerInventory.RemoveItem(resource, investedNumber);
                _repairView.SetData(_repairConfig._stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
            }
        }
    }

    private void TryReturnResource(InventoryItem resource)
    {
        var investedResourceIndex = _repairConfig._stages[_currentRepairStageIndex].Requirements.
            FindIndex(requirement => requirement.Item.ItemID == resource.ItemID);
        int investedResourceNumber = _investedResources[investedResourceIndex];
        
        if (investedResourceNumber > 0 && Inventory.PlayerInventory.HasSpaceFor(resource))
        {
            Inventory.PlayerInventory.TryAddToFirstEmptySlot(resource, investedResourceNumber);
            _investedResources[investedResourceIndex] = 0;
            _repairView.SetData(_repairConfig._stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
        }
        //TODO: учесть случай, когда инаентарь полон(не можем вернуть ресы).
    }

    private bool CanBeRepaired()
    {
        return _repairConfig._stages[_currentRepairStageIndex].Requirements
            .Select((requirement, i) => _investedResources[i] >= requirement.Quantity)
            .All(valid => valid) && 
            !_repairCompleted;
        // for (int i = 0; i < _repairConfig._stages[_currentRepairStageIndex].Requirements.Count; i++)
        // {
        //     if (_investedResources[i] < _repairConfig._stages[_currentRepairStageIndex].Requirements[i].Quantity)
        //     {
        //         return false;
        //     }
        // }
        // return true;
    }
    
    #region Iteractable
    public string GetTooltipTitle() => "Building";

    public string GetTooltipDescription() => "Repair me!";

    public Vector3 GetWorldPosition() => transform.position;

    public bool IsInteractionAllowed { get; set; }
    
    [field: SerializeField] public bool IsBlockingMovement { get; set; }

    public void Interact()
    {
        _repairView.BuildButton.OnClick += Build;
        _repairView.Open();
        _repairView.SetData(_repairConfig._stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
        _repairView.BuildButton.IsInteractable = CanBeRepaired();
    }

    public float GetDistanceTo(Transform otherTransform) => Vector3.Distance(otherTransform.position, transform.position);
    #endregion
}