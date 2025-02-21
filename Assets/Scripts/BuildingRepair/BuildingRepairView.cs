using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.TimeManagement;

namespace FlavorfulStory.BuildingRepair
{
// <summary> Представление UI для системы ремонта зданий. </summary>
    public class BuildingRepairView : MonoBehaviour
    {
        /// <summary> Текст с названием ремонтируемого объекта. </summary>
        [SerializeField] private TMP_Text _objectNameText;

        /// <summary> Текст, отображаемый при завершении ремонта. </summary>
        [SerializeField] private TMP_Text _repairCompletedText;

        
        [SerializeField] private GameObject _requirementViewPrefab;

        [SerializeField] private Transform _requirementViewsContainer;

        /// <summary> Кнопка для подтверждения ремонта. </summary>
        public UIButton BuildButton { get; private set; }

        /// <summary> Список вьюшек требований ресурсов. </summary>
        private List<ResourceRequirementView> _requirementViews;

        /// <summary> Обработчик передачи ресурсов. </summary>
        private Action<InventoryItem, ResourceTransferButtonType> _resourceTransferHandler;

        /// <summary> Флаг, указывающий открыто ли окно ремонта. </summary>
        private bool _isOpen;

        /// <summary> Контейнер для основного содержимого окна. </summary>
        private GameObject _content;

        /// <summary> Инициализация. Коллбэк из UnityAPI. </summary>
        /// <remarks> Собирает вьюшки ресурсных требований. Собирает кнопку строительства. </remarks>
        private void Awake()
        {
            //_requirementViews = GetComponentsInChildren<ResourceRequirementView>(true).ToList();
            _requirementViews = new();
            _content = transform.GetChild(0).gameObject;
            BuildButton = GetComponentInChildren<UIButton>(true);
        }

        /// <summary> Обновление состояния окна. Коллбэк из UnityAPI. </summary>
        private void Update()
        {
            if (_isOpen && InputWrapper.GetButtonDown(InputButton.SwitchGameMenu))
            {
                Close();
                StartCoroutine(BlockGameMenuForOneFrame());
            }
        }

        /// <summary> Заблокировать кнопку переключения игрового меню на один кадр. </summary>
        private System.Collections.IEnumerator BlockGameMenuForOneFrame()
        {
            // Блокируем на следующий кадр
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            yield return null;
            // Разблокируем ввод
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        /// <summary> Инициализировать обработчик передачи ресурсов. </summary>
        /// <param name="resourceTransferHandler"> Обработчик передачи ресурсов. </param>
        public void Initialize(Action<InventoryItem, ResourceTransferButtonType> resourceTransferHandler)
        {
            _resourceTransferHandler = resourceTransferHandler;
        }

        /// <summary> Открыть окно ремонта. </summary>
        public void Open(int requirementsCount)
        {
            _isOpen = true;
            _content.SetActive(_isOpen);
            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
            SpawnRequirementViews(requirementsCount);
            // foreach (var view in _requirementViews)
            // {
            //     view.OnResourceTransferButtonClick += _resourceTransferHandler;
            // }
        }

        /// <summary> Закрыть окно ремонта. </summary>
        public void Close()
        {
            _isOpen = false;
            _content.SetActive(_isOpen);
            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
            DestroyRequirementViews();

            BuildButton.RemoveAllListeners();
        }

        /// <summary> Установить данные для отображения в окне ремонта. </summary>
        /// <param name="stage"> Текущая стадия ремонта. </param>
        /// <param name="investedResources"> Список инвестированных в ремонт ресурсов. </param>
        /// <param name="repairCompleted"> Флаг завершения ремонта. </param>
        public void SetData(RepairStage stage, List<int> investedResources, bool repairCompleted)
        {
            DestroyRequirementViews();
            SpawnRequirementViews(stage.Requirements.Count);
            
            _objectNameText.text = stage.ObjectName;
            _repairCompletedText.text = $"{stage.ObjectName}'s repair completed";
            _repairCompletedText.gameObject.SetActive(repairCompleted);

            _requirementViews.ForEach(view => view.gameObject.SetActive(false));

            if (repairCompleted) return;

            var activeRequirementViews = _requirementViews.Take(stage.Requirements.Count);
            foreach (var view in activeRequirementViews)
            {
                view.gameObject.SetActive(true);
            }

            for (int i = 0; i < stage.Requirements.Count; i++)
            {
                _requirementViews[i].Setup(stage.Requirements[i].Item);
                // TODO: Объединить в один метод
                _requirementViews[i].SetQuantityText(investedResources[i], stage.Requirements[i].Quantity);
                _requirementViews[i].UpdateTransferButtonsInteractableState(
                    stage.Requirements[i].Quantity, investedResources[i]
                );
            }
        }

        private void SpawnRequirementViews(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var requirementView = Instantiate(_requirementViewPrefab, _requirementViewsContainer).GetComponent<ResourceRequirementView>();
                _requirementViews.Add(requirementView);
            }
            
            foreach (var view in _requirementViews)
            {
                view.OnResourceTransferButtonClick += _resourceTransferHandler;
            }
        }

        private void DestroyRequirementViews()
        {
            foreach (var view in _requirementViews)
            {
                view.OnResourceTransferButtonClick -= _resourceTransferHandler;
            }
            foreach (Transform child in _requirementViewsContainer.transform)
            {
                Destroy(child.gameObject);
            }
            _requirementViews.Clear();
        }
    }
}