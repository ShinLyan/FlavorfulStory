using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.TimeManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.BuildingRepair.UI
{
    /// <summary> Визуальное представление ремонта зданий. </summary>
    public class RepairableBuildingView : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Текст с названием ремонтируемого объекта. </summary>
        [SerializeField] private TMP_Text _buildingNameText;

        /// <summary> Текст, отображаемый при завершении ремонта. </summary>
        [SerializeField] private TMP_Text _repairCompletedText;

        /// <summary> Родитель для префабов отображений ресурсных требований. </summary>
        [SerializeField] private Transform _requirementViewsContainer;

        /// <summary> Кнопка подтверждения ремонта. </summary>
        [SerializeField] private Button _buildButton;

        /// <summary> Список отображений требований ресурсов. </summary>
        private readonly List<ResourceRequirementView> _requirementViews = new();

        /// <summary> Открыто ли окно ремонта? </summary>
        private bool _isOpen;

        /// <summary> Обработчик событий передачи ресурсов. </summary>
        private Action<InventoryItem, ResourceTransferButtonType> _transferHandler;

        /// <summary> Обработчик нажатия кнопки постройки. </summary>
        private Action _onBuildPressed;

        /// <summary> Обработчик запроса на закрытие окна. </summary>
        private Action _onCloseRequested;

        /// <summary> Фабрика создания отображений требований ресурсов. </summary>
        private IGameFactory<ResourceRequirementView> _requirementViewFactory;

        #endregion

        /// <summary> Внедрить фабрику создания отображений требований ресурсов. </summary>
        /// <param name="factory"> Фабрика создания отображений требований ресурсов. </param>
        [Inject]
        private void Construct(IGameFactory<ResourceRequirementView> factory) => _requirementViewFactory = factory;

        /// <summary> Инициализация кэша вьюшек, если они уже присутствуют в иерархии. </summary>
        private void Awake() => CacheInitialViews();

        /// <summary> Сохранить существующие отображения ресурсов, если они уже находятся в контейнере. </summary>
        private void CacheInitialViews() // TODO: ПЕРЕПИСАТЬ НА НОВЫЙ OBJECTPOOL
        {
            foreach (Transform child in _requirementViewsContainer)
            {
                var view = child.GetComponent<ResourceRequirementView>();
                if (!view || _requirementViews.Contains(view)) continue;

                view.gameObject.SetActive(false);
                _requirementViews.Add(view);
            }
        }

        /// <summary> Проверить ввод пользователя и закрыть окно при необходимости. </summary>
        private void Update()
        {
            if (!_isOpen || !InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            Close();
            BlockGameMenuForOneFrame().Forget();
        }

        /// <summary> Заблокировать кнопку игрового меню на один кадр. </summary>
        private static async UniTaskVoid BlockGameMenuForOneFrame() // TODO: УДАЛИТЬ КОСТЫЛЬ НА WINDOW FABRIC
        {
            InputWrapper.BlockInput(InputButton.SwitchGameMenu);
            await UniTask.Yield();
            InputWrapper.UnblockAllInput();
        }

        /// <summary> Отобразить UI ремонта для указанного объекта. </summary>
        /// <param name="stage"> Объект ремонта. </param>
        /// <param name="investedResources"> Список вложенных ресурсов. </param>
        /// <param name="onTransfer"> Обработчик передачи ресурсов. </param>
        /// <param name="onBuild"> Обработчик подтверждения ремонта. </param>
        /// <param name="onCloseRequested"> Обработчик запроса закрытия окна. </param>
        public void Show(RepairStage stage, List<int> investedResources,
            Action<InventoryItem, ResourceTransferButtonType> onTransfer,
            Action onBuild, Action onCloseRequested)
        {
            _transferHandler = onTransfer;
            _onBuildPressed = onBuild;
            _onCloseRequested = onCloseRequested;

            UpdateView(stage, investedResources);
            Open();
        }

        /// <summary> Обновить отображение требований и состояния ремонта. </summary>
        /// <param name="stage"> Текущая стадия ремонта. </param>
        /// <param name="investedResources"> Список вложенных ресурсов. </param>
        public void UpdateView(RepairStage stage, List<int> investedResources)
        {
            for (int i = 0; i < stage.Requirements.Count; i++)
            {
                var view = EnsureRequirementView(i);
                UpdateRequirementView(view, stage.Requirements[i], investedResources[i]);
            }

            DisableExcessViews(stage.Requirements.Count);
            _buildingNameText.text = stage.BuildingName;
            _buildButton.interactable = IsRepairPossible(stage, investedResources);
        }

        /// <summary> Получить или создать отображение требования по индексу. </summary>
        /// <param name="index"> Индекс требования. </param>
        /// <returns> Отображение требования ресурса. </returns>
        private ResourceRequirementView EnsureRequirementView(int index)
        {
            if (index < _requirementViews.Count) return _requirementViews[index];

            var view = _requirementViewFactory.Create(_requirementViewsContainer);
            _requirementViews.Add(view);
            return view;
        }

        /// <summary> Настроить отображение отдельного требования ресурса. </summary>
        /// <param name="view"> Отображение требования. </param>
        /// <param name="requirement"> Требование ресурса. </param>
        /// <param name="invested"> Количество вложенного ресурса. </param>
        private void UpdateRequirementView(ResourceRequirementView view, ItemStack requirement, int invested)
        {
            view.gameObject.SetActive(true);
            view.Setup(requirement, invested);
            view.OnResourceTransferButtonClick -= _transferHandler;
            view.OnResourceTransferButtonClick += _transferHandler;
        }

        /// <summary> Отключить лишние отображения требований. </summary>
        /// <param name="activeCount"> Количество активных требований. </param>
        private void DisableExcessViews(int activeCount)
        {
            for (int i = activeCount; i < _requirementViews.Count; i++)
            {
                _requirementViews[i].gameObject.SetActive(false);
                _requirementViews[i].OnResourceTransferButtonClick -= _transferHandler;
            }
        }

        /// <summary> Проверить возможность завершения ремонта. </summary>
        /// <param name="stage"> Стадия ремонта. </param>
        /// <param name="investedResources"> Вложенные ресурсы. </param>
        /// <returns> <c>true</c>, если все ресурсы вложены; иначе <c>false</c>. </returns>
        private static bool IsRepairPossible(RepairStage stage, List<int> investedResources) =>
            !stage.Requirements.Where((itemStack, i) => investedResources[i] < itemStack.Number).Any();

        /// <summary> Открыть окно ремонта. </summary>
        private void Open()
        {
            _isOpen = true;
            gameObject.SetActive(true);
            _requirementViewsContainer.gameObject.SetActive(true);
            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);
            if (_onBuildPressed != null) _buildButton.onClick.AddListener(_onBuildPressed.Invoke);
        }

        /// <summary> Закрыть окно ремонта и сбросить состояние. </summary>
        public void Close()
        {
            _isOpen = false;
            gameObject.SetActive(false);
            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();
            if (_onBuildPressed != null) _buildButton.onClick.RemoveListener(_onBuildPressed.Invoke);

            CleanupViews();

            _onCloseRequested?.Invoke();
        }

        /// <summary> Сбросить отображения требований и обработчики. </summary>
        private void CleanupViews() // TODO: ПЕРЕПИСАТЬ НА НОВЫЙ OBJECTPOOL
        {
            foreach (var view in _requirementViews)
            {
                view.gameObject.SetActive(false);
                view.OnResourceTransferButtonClick -= _transferHandler;
            }

            _transferHandler = null;
            _onBuildPressed = null;
        }

        /// <summary> Отобразить сообщение об окончании ремонта. </summary>
        /// <param name="repairableBuildingName"> Название ремонтируемого здания. </param>
        public void DisplayCompletionMessage(RepairableBuildingName repairableBuildingName)
        {
            _requirementViewsContainer.gameObject.SetActive(false);

            _repairCompletedText.text = $"{repairableBuildingName}'s repair completed";
            _repairCompletedText.gameObject.SetActive(true);

            _buildButton.gameObject.SetActive(false);
        }
    }
}