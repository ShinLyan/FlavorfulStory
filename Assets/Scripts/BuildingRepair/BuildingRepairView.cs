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

        /// <summary> Контейнер содержимого окна. </summary>
        [SerializeField] private GameObject _content;

        /// <summary> Текст с названием ремонтируемого объекта. </summary>
        [SerializeField] private TMP_Text _objectNameText;

        /// <summary> Текст, отображаемый при завершении ремонта. </summary>
        [SerializeField] private TMP_Text _repairCompletedText;

        /// <summary> Префаб отображения ресурсного требования. </summary>
        [SerializeField] private ResourceRequirementView _requirementViewPrefab;

        /// <summary> Родитель для префабов отображений ресурсных требований. </summary>
        [SerializeField] private Transform _requirementViewsContainer;

        /// <summary> Кнопка подтверждения ремонта. </summary>
        [SerializeField] private UIButton _buildButton;

        /// <summary> Список вьюшек требований ресурсов. </summary>
        private readonly List<ResourceRequirementView> _requirementViews = new();

        /// <summary> Открыто ли окно ремонта? </summary>
        private bool _isOpen;

        /// <summary> Текущий ремонтируемый объект. </summary>
        private BuildingRepair _currentRepairable;

        /// <summary> Обработчик событий передачи ресурсов. </summary>
        private Action<InventoryItem, ResourceTransferButtonType> _transferHandler;

        /// <summary> Событие, вызываемое при закрытии окна ремонта. </summary>
        public event Action OnClose;

        #endregion

        /// <summary> Обновление состояния окна ремонта. </summary>
        private void Update()
        {
            if (!_isOpen || !InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            Close();
            StartCoroutine(BlockGameMenuForOneFrame());
        }

        /// <summary> Заблокировать кнопку игрового меню на один кадр. </summary>
        private static IEnumerator BlockGameMenuForOneFrame()
        {
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            yield return null;
            InputWrapper.UnblockAllInput();
        }

        /// <summary> Отобразить UI ремонта для указанного объекта. </summary>
        /// <param name="repairable"> Объект ремонта. </param>
        public void Show(BuildingRepair repairable)
        {
            _currentRepairable = repairable;
            _transferHandler = repairable.TransferResource;

            repairable.OnStageUpdated += OnStageUpdated;
            repairable.OnRepairCompleted += DisplayCompletionMessage;
            _buildButton.OnClick += repairable.Build;

            OnStageUpdated(repairable.CurrentStage, repairable.InvestedResources);
            Open();
        }

        /// <summary> Обновить отображение требований и состояния ремонта. </summary>
        /// <param name="stage"> Текущая стадия ремонта. </param>
        /// <param name="investedResources"> Список вложенных ресурсов. </param>
        private void OnStageUpdated(RepairStage stage, List<int> investedResources)
        {
            if (_requirementViews.Count == stage.Requirements.Count)
            {
                for (int i = 0; i < _requirementViews.Count; i++)
                    _requirementViews[i].Setup(stage.Requirements[i], investedResources[i]);
            }
            else
            {
                DestroyRequirementViews();
                SpawnRequirementViews(stage, investedResources);
            }

            _objectNameText.text = stage.BuildingName;
            _repairCompletedText.gameObject.SetActive(false);
            _repairCompletedText.text = $"{stage.BuildingName}'s repair completed";

            _buildButton.IsInteractable = IsRepairPossible(stage, investedResources);
        }

        /// <summary> Проверка возможности завершения текущей стадии ремонта. </summary>
        /// <param name="stage"> Стадия ремонта. </param>
        /// <param name="investedResources"> Вложенные ресурсы. </param>
        /// <returns> <c>true</c>, если все ресурсы вложены; иначе <c>false</c>. </returns>
        private static bool IsRepairPossible(RepairStage stage, List<int> investedResources)
        {
            for (int i = 0; i < stage.Requirements.Count; i++)
                if (investedResources[i] < stage.Requirements[i].Quantity)
                    return false;

            return true;
        }

        /// <summary> Открыть окно ремонта. </summary>
        private void Open()
        {
            _isOpen = true;
            _content.SetActive(true);
            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
        }

        /// <summary> Закрыть окно ремонта и сбросить состояние. </summary>
        private void Close()
        {
            if (_currentRepairable)
            {
                _currentRepairable.OnStageUpdated -= OnStageUpdated;
                _currentRepairable.OnRepairCompleted -= DisplayCompletionMessage;
                _buildButton.OnClick -= _currentRepairable.Build;
                _currentRepairable = null;
            }

            _isOpen = false;
            _content.SetActive(false);
            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();
            OnClose?.Invoke();
        }

        /// <summary> Удалить старые отображения требований ресурсов. </summary>
        private void DestroyRequirementViews()
        {
            foreach (var view in _requirementViews)
                view.OnResourceTransferButtonClick -= _transferHandler;

            foreach (Transform child in _requirementViewsContainer.transform)
                Destroy(child.gameObject);

            _requirementViews.Clear();
        }

        /// <summary> Создать отображения требований ресурсов. </summary>
        /// <param name="stage"> Текущая стадия ремонта. </param>
        /// <param name="investedResources"> Количество вложенных ресурсов. </param>
        private void SpawnRequirementViews(RepairStage stage, List<int> investedResources)
        {
            for (int i = 0; i < stage.Requirements.Count; i++)
            {
                var requirementView = Instantiate(_requirementViewPrefab, _requirementViewsContainer);
                requirementView.Setup(stage.Requirements[i], investedResources[i]);
                requirementView.OnResourceTransferButtonClick += _transferHandler;
                _requirementViews.Add(requirementView);
            }
        }

        /// <summary> Отобразить сообщение об окончании ремонта. </summary>
        private void DisplayCompletionMessage()
        {
            _requirementViews.ForEach(view => view.gameObject.SetActive(false));
            _repairCompletedText.gameObject.SetActive(true);
            _buildButton.IsInteractable = false;
        }
    }
}