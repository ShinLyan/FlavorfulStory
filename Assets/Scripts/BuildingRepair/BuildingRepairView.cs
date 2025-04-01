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
    /// <summary> Представление UI для системы ремонта зданий. </summary>
    public class BuildingRepairView : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Текст с названием ремонтируемого объекта. </summary>
        [SerializeField] private TMP_Text _objectNameText;

        /// <summary> Текст, отображаемый при завершении ремонта. </summary>
        [SerializeField] private TMP_Text _repairCompletedText;

        /// <summary> Префаб отображения ресурсного требования. </summary>
        [SerializeField] private ResourceRequirementView _requirementViewPrefab;

        /// <summary> Родитель для префабов отображений ресурсных требований. </summary>
        [SerializeField] private Transform _requirementViewsContainer;

        /// <summary> Список вьюшек требований ресурсов. </summary>
        private readonly List<ResourceRequirementView> _requirementViews = new();

        /// <summary> Открыто ли окно ремонта? </summary>
        private bool _isOpen;

        /// <summary> Контейнер содержимого окна. </summary>
        private GameObject _content;

        /// <summary> Кнопка для подтверждения ремонта. </summary>
        public UIButton BuildButton { get; private set; }

        /// <summary> Обработчик событий передачи ресурсов. </summary>
        private Action<InventoryItem, ResourceTransferButtonType> _resourceTransferHandler;

        /// <summary> Событие, вызываемое при закрытии окна ремонта. </summary>
        public event Action OnClose;

        #endregion

        /// <summary> Первичная инициализация компонентов. </summary>
        private void Awake()
        {
            _content = transform.GetChild(0).gameObject;
            BuildButton = GetComponentInChildren<UIButton>(true);
        }

        /// <summary> Обновление состояния окна ремонта. </summary>
        private void Update()
        {
            if (!_isOpen || !InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            Close();
            StartCoroutine(BlockGameMenuForOneFrame());
        }

        /// <summary> Открыть окно ремонта. </summary>
        public void Open()
        {
            _isOpen = true;
            _content.SetActive(true);
            WorldTime.Pause();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        /// <summary> Закрыть окно ремонта. </summary>
        public void Close()
        {
            _isOpen = false;
            _content.SetActive(false);
            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();
            BuildButton.RemoveAllListeners();
            OnClose?.Invoke();
        }

        /// <summary> Заблокировать кнопку переключения игрового меню на один кадр. </summary>
        private static IEnumerator BlockGameMenuForOneFrame()
        {
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            yield return null;
            InputWrapper.UnblockAllInput();
        }

        /// <summary> Инициализировать обработчик передачи ресурсов. </summary>
        /// <param name="resourceTransferHandler"> Делегат для обработки событий передачи ресурсов. </param>
        public void Initialize(Action<InventoryItem, ResourceTransferButtonType> resourceTransferHandler) =>
            _resourceTransferHandler = resourceTransferHandler;

        /// <summary> Установить данные для окна ремонта. </summary>
        /// <param name="stage"> Текущая стадия ремонта. </param>
        /// <param name="investedResources"> Список ресурсов, вложенных в ремонт. </param>
        public void SetData(RepairStage stage, List<int> investedResources)
        {
            DestroyRequirementViews();
            SpawnRequirementViews(stage, investedResources);

            _objectNameText.text = stage.BuildingName;
            _repairCompletedText.gameObject.SetActive(false);
            _repairCompletedText.text = $"{stage.BuildingName}'s repair completed";
        }

        /// <summary> Удалить предыдущие отображения ресурсных требований. </summary>
        private void DestroyRequirementViews()
        {
            foreach (Transform child in _requirementViewsContainer.transform)
                Destroy(child.gameObject);

            foreach (var view in _requirementViews)
                view.OnResourceTransferButtonClick -= _resourceTransferHandler;
            _requirementViews.Clear();
        }

        /// <summary> Заспавнить новые отображения ресурсных требований. </summary>
        /// <param name="stage"> Текущая стадия ремонта. </param>
        /// <param name="investedResources"> Список ресурсов, вложенных в ремонт. </param>
        private void SpawnRequirementViews(RepairStage stage, List<int> investedResources)
        {
            for (int i = 0; i < stage.Requirements.Count; i++)
            {
                var requirementView = Instantiate(_requirementViewPrefab, _requirementViewsContainer);
                requirementView.OnResourceTransferButtonClick += _resourceTransferHandler;
                requirementView.Setup(stage.Requirements[i], investedResources[i]);
                _requirementViews.Add(requirementView);
            }
        }

        /// <summary> Отобразить сообщение о завершении ремонта. </summary>
        public void DisplayCompletionMessage()
        {
            _requirementViews.ForEach(view => view.gameObject.SetActive(false));
            _repairCompletedText.gameObject.SetActive(true);
            BuildButton.IsInteractable = false;
        }
    }
}