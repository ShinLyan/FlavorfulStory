using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;

public class BuildingRepairView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private RectTransform _mainWindowContent;
    [SerializeField] private TMP_Text _objectNameText;
    [SerializeField] private TMP_Text _repairCompletedText;
    public UIButton BuildButton { get; private set; }

    private List<ResourceRequirementView> _requirementViews;

    private Action<InventoryItem, ResourceTransferButtonType> _resourceTransferHandler;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        _requirementViews = GetComponentsInChildren<ResourceRequirementView>(true).ToList();
        BuildButton = GetComponentInChildren<UIButton>(true);
    }

    public void Initialize(Action<InventoryItem, ResourceTransferButtonType> resourceTransferHandler)
    {
        _resourceTransferHandler = resourceTransferHandler;
    }

    public void Open()
    {
        IsOpen = true;
        _background.gameObject.SetActive(IsOpen);
        _mainWindowContent.gameObject.SetActive(IsOpen);
        InputWrapper.BlockAllInput();
        foreach (var view in _requirementViews)
        {
            view.OnResourceTransferButtonClick += _resourceTransferHandler;
        }
    }

    public void Close()
    {
        IsOpen = false;
        _background.gameObject.SetActive(IsOpen);
        _mainWindowContent.gameObject.SetActive(IsOpen);
        InputWrapper.UnblockAllInput();
        foreach (var view in _requirementViews)
        {
            view.OnResourceTransferButtonClick -= _resourceTransferHandler;
        }

        BuildButton.RemoveAllListeners();
    }

    public void SetData(RepairStage stage, List<int> investedResources, bool repairCompleted)
    {
        _objectNameText.text = stage.objectName;
        _repairCompletedText.text = $"{stage.objectName}'s repair completed";
        _repairCompletedText.gameObject.SetActive(repairCompleted);

        _requirementViews.ForEach(view => view.gameObject.SetActive(false));

        if (!repairCompleted)
        {
            var activeRequirementViews = _requirementViews.Take(stage.Requirements.Count);
            foreach (var view in activeRequirementViews)
            {
                view.gameObject.SetActive(true);
            }

            for (int i = 0; i < stage.Requirements.Count; i++)
            {
                _requirementViews[i].SetResource(stage.Requirements[i].Item);
                _requirementViews[i].UpdateResourceIcon();
                _requirementViews[i].SetQuantityText(investedResources[i], stage.Requirements[i].Quantity);
                _requirementViews[i].UpdateTransferButtonsInteractableState(
                    stage.Requirements[i].Quantity, investedResources[i]
                );
            }
        }
    }
}