using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.TimeManagement;

// <summary> Представление UI для системы ремонта зданий. </summary>
public class BuildingRepairView : MonoBehaviour
{
    /// <summary> Фоновое изображение окна ремонта. </summary>
    [SerializeField] private Image _background;

    /// <summary> Контейнер для основного содержимого окна. </summary>
    [SerializeField] private RectTransform _mainWindowContent;

    /// <summary> Текст с названием ремонтируемого объекта. </summary>
    [SerializeField] private TMP_Text _objectNameText;

    /// <summary> Текст, отображаемый при завершении ремонта. </summary>
    [SerializeField] private TMP_Text _repairCompletedText;

    /// <summary> Кнопка для подтверждения ремонта. </summary>
    public UIButton BuildButton { get; private set; }

    /// <summary> Список вьюшек требований ресурсов. </summary>
    private List<ResourceRequirementView> _requirementViews;

    /// <summary> Обработчик передачи ресурсов. </summary>
    private Action<InventoryItem, ResourceTransferButtonType> _resourceTransferHandler;

    /// <summary> Флаг, указывающий открыто ли окно ремонта. </summary>
    private bool _isOpen;

    /// <summary> Инициализация. Коллбэк из UnityAPI. </summary>
    /// <remarks> Собирает вьюшки ресурсных требований. Собирает кнопку строительства. </remarks>
    private void Awake()
    {
        _requirementViews = GetComponentsInChildren<ResourceRequirementView>(true).ToList();
        BuildButton = GetComponentInChildren<UIButton>(true);
    }

    /// <summary> Обновление состояния окна. Коллбэк из UnityAPI. </summary>
    private void Update()
    {
        if (_isOpen && InputWrapper.GetButtonDown(InputButton.SwitchGameMenu, true))
        {
            Close();
        }
    }

    /// <summary> Инициализировать обработчик передачи ресурсов. </summary>
    /// <param name="resourceTransferHandler"> Обработчик передачи ресурсов. </param>
    public void Initialize(Action<InventoryItem, ResourceTransferButtonType> resourceTransferHandler)
    {
        _resourceTransferHandler = resourceTransferHandler;
    }

    /// <summary> Открыть окно ремонта. </summary>
    public void Open()
    {
        _isOpen = true;
        _background.gameObject.SetActive(_isOpen);
        _mainWindowContent.gameObject.SetActive(_isOpen);
        WorldTime.Pause();
        InputWrapper.BlockAllInput();
        foreach (var view in _requirementViews)
        {
            view.OnResourceTransferButtonClick += _resourceTransferHandler;
        }
    }

    /// <summary> Закрыть окно ремонта. </summary>
    public void Close()
    {
        _isOpen = false;
        _background.gameObject.SetActive(_isOpen);
        _mainWindowContent.gameObject.SetActive(_isOpen);
        WorldTime.Unpause();
        InputWrapper.UnblockAllInput();
        foreach (var view in _requirementViews)
        {
            view.OnResourceTransferButtonClick -= _resourceTransferHandler;
        }

        BuildButton.RemoveAllListeners();
    }

    /// <summary> Установить данные для отображения в окне ремонта. </summary>
    /// <param name="stage"> Текущая стадия ремонта. </param>
    /// <param name="investedResources"> Список инвестированных в ремонт ресурсов. </param>
    /// <param name="repairCompleted"> Флаг завершения ремонта. </param>
    public void SetData(RepairStage stage, List<int> investedResources, bool repairCompleted)
    {
        _objectNameText.text = stage.ObjectName;
        _repairCompletedText.text = $"{stage.ObjectName}'s repair completed";
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