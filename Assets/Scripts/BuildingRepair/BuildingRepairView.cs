using System;
using System.Collections;
using System.Collections.Generic;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.TimeManagement;
using FlavorfulStory.UI;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
// <summary> Представление UI для системы ремонта зданий. </summary>
    public class BuildingRepairView : MonoBehaviour
    {
        /// <summary> Текст с названием ремонтируемого объекта. </summary>
        [SerializeField] private TMP_Text _objectNameText;

        /// <summary> Текст, отображаемый при завершении ремонта. </summary>
        [SerializeField] private TMP_Text _repairCompletedText;

        /// <summary> Префаб вьюшки ресурсного требования. </summary>
        /// <remarks> Спавнится в родителя, что имеет VerticalLayoutGroup. </remarks>
        [SerializeField] private GameObject _requirementViewPrefab;

        /// <summary> Родитель вьюшек ресурсных требований. </summary>
        /// <remarks> Содержит компонент VerticalLayoutGroup. </remarks>
        [SerializeField] private Transform _requirementViewsContainer;

        /// <summary> Список вьюшек требований ресурсов. </summary>
        private List<ResourceRequirementView> _requirementViews;

        /// <summary> Флаг, указывающий открыто ли окно ремонта. </summary>
        private bool _isOpen;

        /// <summary> Контейнер для основного содержимого окна. </summary>
        private GameObject _content;

        /// <summary> Кнопка для подтверждения ремонта. </summary>
        public UIButton BuildButton { get; private set; }

        /// <summary> Обработчик передачи ресурсов. </summary>
        private Action<InventoryItem, ResourceTransferButtonType> _resourceTransferHandler;

        /// <summary> Инициализация. Коллбэк из UnityAPI. </summary>
        /// <remarks> Собирает вьюшки ресурсных требований. Собирает кнопку строительства. </remarks>
        private void Awake()
        {
            _requirementViews = new List<ResourceRequirementView>();
            _content = transform.GetChild(0).gameObject;
            BuildButton = GetComponentInChildren<UIButton>(true);
        }

        /// <summary> Обновление состояния окна. Коллбэк из UnityAPI. </summary>
        private void Update()
        {
            // TODO: КОСТЫЛЬ. Добавлен, чтобы после проигрывания анимации, игрок не мог бегать
            if (_isOpen) InputWrapper.BlockPlayerMovement();

            if (!_isOpen || !InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            Close();
            StartCoroutine(BlockGameMenuForOneFrame());
        }

        /// <summary> Закрыть окно ремонта. </summary>
        public void Close()
        {
            _isOpen = false;
            _content.SetActive(_isOpen);
            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();
            BuildButton.RemoveAllListeners();
        }

        /// <summary> Заблокировать кнопку переключения игрового меню на один кадр. </summary>
        private static IEnumerator BlockGameMenuForOneFrame()
        {
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            yield return null;
            InputWrapper.UnblockAllInput();
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
            _content.SetActive(_isOpen);
            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        /// <summary> Установить данные для отображения в окне ремонта. </summary>
        /// <param name="stage"> Текущая стадия ремонта. </param>
        /// <param name="investedResources"> Список инвестированных в ремонт ресурсов. </param>
        /// <param name="repairCompleted"> Флаг завершения ремонта. </param>
        public void SetData(RepairStage stage, List<int> investedResources)
        {
            DestroyRequirementViews();
            SpawnRequirementViews(stage.Requirements.Count);

            _objectNameText.text = stage.BuildingName;

            _repairCompletedText.gameObject.SetActive(false);
            _repairCompletedText.text = $"{stage.BuildingName}'s repair completed";

            _requirementViews.ForEach(view => view.gameObject.SetActive(true));

            for (int i = 0; i < stage.Requirements.Count; i++)
                _requirementViews[i].Setup(stage.Requirements[i], investedResources[i]);
        }

        public void DisplayCompletionMessage()
        {
            _requirementViews.ForEach(view => view.gameObject.SetActive(false));
            _repairCompletedText.gameObject.SetActive(true);
            BuildButton.IsInteractable = false;
        }

        /// <summary> Удалить вьюшки ресурсных требований. </summary>
        private void DestroyRequirementViews()
        {
            foreach (var view in _requirementViews)
                view.OnResourceTransferButtonClick -= _resourceTransferHandler;

            foreach (Transform child in _requirementViewsContainer.transform)
                Destroy(child.gameObject);

            _requirementViews.Clear();
        }

        /// <summary> Заспавнить вьюшки ресурсных требований. </summary>
        /// <param name="count"> Количество вьющек. </param>
        private void SpawnRequirementViews(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var requirementView = Instantiate(_requirementViewPrefab, _requirementViewsContainer);
                requirementView.SetActive(false);
                _requirementViews.Add(requirementView.GetComponent<ResourceRequirementView>());
            }

            foreach (var view in _requirementViews) view.OnResourceTransferButtonClick += _resourceTransferHandler;
        }
    }
}