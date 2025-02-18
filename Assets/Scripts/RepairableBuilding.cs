using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FlavorfulStory.Actions.Interactables;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Saving;
using FlavorfulStory.SceneManagement;

public class RepairableBuilding : MonoBehaviour, IInteractable, ISaveable
{
    [Tooltip("Стадии ремонта.")] 
    [SerializeField] private BuildingRepairConfig _repairConfig;

    private BuildingRepairView _repairView;

    private List<int> _investedResources;

    private int _currentRepairStageIndex;

    private bool _repairCompleted;

    private List<GameObject> _stagesGameobjects;
    
    private void Awake()
    {
        _repairView = FindFirstObjectByType<BuildingRepairView>(FindObjectsInactive.Include);
        _stagesGameobjects = new(_repairConfig.Stages.Count + 1);
    }

    private void Start()
    {
        if (!SavingWrapper.SaveFileExists)
        {
            LoadDefaultState();
        }
        IsBlockingMovement = true;
    }

    private void LoadDefaultState()
    {
        _currentRepairStageIndex = 0;
        _investedResources = new(_repairConfig.Stages[_currentRepairStageIndex].Requirements.Count);
        for (int i = 0; i < _repairConfig.Stages[_currentRepairStageIndex].Requirements.Count; i++)
        {
            _investedResources.Add(0);
        }
        _repairCompleted = false;
        IsInteractionAllowed = !_repairCompleted;
        SpawnVisualStates();
        SetVisualState(_currentRepairStageIndex, _repairCompleted);
    }
    
    private void SpawnVisualStates()
    {
        var go = Instantiate(_repairConfig.DefaultGameObject, transform, false);
        _stagesGameobjects.Add(go);
        foreach (var stage in _repairConfig.Stages)
        {
            var stageGo = Instantiate(stage.Gameobject, transform, false);
            _stagesGameobjects.Add(stageGo);
        }
    }

    private void SetVisualState(int currentStageIndex, bool repairCompleted)
    {
        int deltaIndex = repairCompleted ? 1 : 0;
        foreach (var stageGameobject in _stagesGameobjects)
        {
            stageGameobject.gameObject.SetActive(false);
        }
        _stagesGameobjects[currentStageIndex + deltaIndex].gameObject.SetActive(true);
    }
    
    private void Build()
    {
        _repairCompleted = _currentRepairStageIndex >= _repairConfig.Stages.Count - 1;
        if (!_repairCompleted)
        {
            _currentRepairStageIndex++;
            _investedResources.Clear();
            foreach (var requirement in _repairConfig.Stages[_currentRepairStageIndex].Requirements)
            {
                _investedResources.Add(0);
            }
        }
        _repairView.SetData(_repairConfig.Stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
        _repairView.BuildButton.IsInteractable = CanBeRepaired();
        IsInteractionAllowed = !_repairCompleted;
        SetVisualState(_currentRepairStageIndex, _repairCompleted);
    }
    
    private void TransferResource(InventoryItem resource, ResourceTransferButtonType transferButtonType)
    {
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
        int resourceRequirementNumber = _repairConfig.Stages[_currentRepairStageIndex].Requirements
            .Find(requirement => requirement.Item.ItemID == resource.ItemID).Quantity;
        
        var investedResourceIndex = _repairConfig.Stages[_currentRepairStageIndex].Requirements.FindIndex(x => x.Item.ItemID == resource.ItemID);
        if (_investedResources[investedResourceIndex] < resourceRequirementNumber)
        {
            if (Inventory.PlayerInventory.HasItem(resource) && Inventory.PlayerInventory.GetItemNumber(resource) > 0)
            {
                int numberToInvest = resourceRequirementNumber - _investedResources[investedResourceIndex];
                int investedNumber = Math.Min(numberToInvest, Inventory.PlayerInventory.GetItemNumber(resource));
                _investedResources[investedResourceIndex] += investedNumber;
                Inventory.PlayerInventory.RemoveItem(resource, investedNumber);
                _repairView.SetData(_repairConfig.Stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
            }
        }
    }

    private void TryReturnResource(InventoryItem resource)
    {
        var investedResourceIndex = _repairConfig.Stages[_currentRepairStageIndex].Requirements.
            FindIndex(requirement => requirement.Item.ItemID == resource.ItemID);
        int investedResourceNumber = _investedResources[investedResourceIndex];
        
        //TODO: Убедиться в работоспособности HasSpaceFor(): учесть случай, когда инаентарь полон(не можем вернуть ресы).
        if (investedResourceNumber > 0 && Inventory.PlayerInventory.HasSpaceFor(resource))
        {
            Inventory.PlayerInventory.TryAddToFirstEmptySlot(resource, investedResourceNumber);
            _investedResources[investedResourceIndex] = 0;
            _repairView.SetData(_repairConfig.Stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
        }
    }

    private bool CanBeRepaired()
    {
        return _repairConfig.Stages[_currentRepairStageIndex].Requirements
            .Select((requirement, i) => _investedResources[i] >= requirement.Quantity)
            .All(valid => valid) && 
            !_repairCompleted;
    }
    
    #region Iteractable
    public string GetTooltipTitle() => "Building";

    public string GetTooltipDescription() => "Repair me!";

    public Vector3 GetWorldPosition() => transform.position;

    public bool IsInteractionAllowed { get; set; }
    
    [field: SerializeField] public bool IsBlockingMovement { get; set; }

    public void Interact()
    {
        _repairView.Initialize(TransferResource);
        _repairView.BuildButton.OnClick += Build;
        _repairView.Open();
        _repairView.SetData(_repairConfig.Stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
        _repairView.BuildButton.IsInteractable = CanBeRepaired();
    }

    public float GetDistanceTo(Transform otherTransform) => Vector3.Distance(otherTransform.position, transform.position);
    #endregion

    #region Saving
    
    [Serializable]
    private struct RepairableBuildingRecord
    {
        public List<int> InvestedResources;
        public int RepairStageIndex;
        public bool RepairCompleted;
    }
    public object CaptureState()
    {
        var state = new RepairableBuildingRecord
        {
            RepairStageIndex = _currentRepairStageIndex,
            InvestedResources = _investedResources,
            RepairCompleted = _repairCompleted
        };
        return state;
    }

    public void RestoreState(object state)
    {
        var record = state is RepairableBuildingRecord buildingRecord ? buildingRecord : default;

        if (SavingWrapper.SaveFileExists)
        {
            LoadFromSave(record);
        }
    }

    private void LoadFromSave(RepairableBuildingRecord record)
    {
        _investedResources = record.InvestedResources;
        _currentRepairStageIndex = record.RepairStageIndex;
        _repairCompleted = record.RepairCompleted;
        IsInteractionAllowed = !_repairCompleted;
        SpawnVisualStates();
        SetVisualState(_currentRepairStageIndex, _repairCompleted);
    }
    #endregion
}