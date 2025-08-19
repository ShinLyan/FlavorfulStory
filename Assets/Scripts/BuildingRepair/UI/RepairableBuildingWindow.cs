using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.Infrastructure.Factories;
using FlavorfulStory.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.BuildingRepair.UI
{
    /// <summary> Визуальное представление ремонта зданий. </summary>
    public class RepairableBuildingWindow : BaseWindow
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

        /// <summary> Пул для повторного использования требований к ремонту. </summary>
        private ObjectPool<ResourceRequirementView> _requirementPool;

        /// <summary> Список отображений требований ресурсов. </summary>
        private readonly List<ResourceRequirementView> _requirementViews = new();

        /// <summary> Делегат, вызываемый при добавлении ресурса. </summary>
        private Action<InventoryItem> _onAdd;

        /// <summary> Делегат, вызываемый при возврате ресурса. </summary>
        private Action<InventoryItem> _onReturn;

        /// <summary> Делегат, вызываемый при нажатии кнопки постройки. </summary>
        private Action _onBuild;

        /// <summary> Фабрика создания отображений требований ресурсов. </summary>
        private IPrefabFactory<ResourceRequirementView> _requirementViewFactory;

        #endregion

        /// <summary> Внедрить фабрику создания отображений требований ресурсов. </summary>
        /// <param name="factory"> Фабрика создания отображений требований ресурсов. </param>
        [Inject]
        private void Construct(IPrefabFactory<ResourceRequirementView> factory) => _requirementViewFactory = factory;

        /// <summary> Инициализация кэша вьюшек, если они уже присутствуют в иерархии. </summary>
        private void Awake()
        {
            _requirementPool = new ObjectPool<ResourceRequirementView>(
                () => _requirementViewFactory.Create(parentTransform: _requirementViewsContainer),
                view =>
                {
                    view.gameObject.SetActive(true);
                    view.transform.SetParent(_requirementViewsContainer, false);
                },
                view =>
                {
                    view.OnAddClicked -= _onAdd;
                    view.OnReturnClicked -= _onReturn;
                    view.gameObject.SetActive(false);
                }
            );

            RegisterExistingViews(_requirementViewsContainer, _requirementPool);
        }

        /// <summary> Регистрирует уже существующие дочерние элементы в переданном контейнере
        /// как неактивные и добавляет их в пул. </summary>
        /// <typeparam name="T"> Тип компонента, который нужно зарегистрировать. </typeparam>
        /// <param name="parent"> Родительский трансформ, содержащий компоненты. </param>
        /// <param name="pool"> Пул объектов, в который добавляются компоненты. </param>
        private static void RegisterExistingViews<T>(Transform parent, ObjectPool<T> pool) where T : Component
        {
            foreach (Transform child in parent)
                if (child.TryGetComponent<T>(out var component))
                {
                    component.gameObject.SetActive(false);
                    pool.Release(component);
                }
        }

        /// <summary> Отобразить UI ремонта для указанного объекта. </summary>
        /// <param name="stage"> Объект ремонта. </param>
        /// <param name="investedResources"> Список вложенных ресурсов. </param>
        /// <param name="onAdd"> Обработчик добавления ресурсов. </param>
        /// <param name="onReturn"> Обработчик забирания ресурсов. </param>
        /// <param name="onBuild"> Обработчик подтверждения ремонта. </param>
        public void Setup(RepairStage stage, List<int> investedResources,
            Action<InventoryItem> onAdd, Action<InventoryItem> onReturn, Action onBuild)
        {
            _onAdd = onAdd;
            _onReturn = onReturn;
            _onBuild = onBuild;
            
            _buildButton.onClick.AddListener(_onBuild.Invoke);

            UpdateView(stage, investedResources);
        }

        /// <summary> Обновить отображение требований и состояния ремонта. </summary>
        /// <param name="stage"> Текущая стадия ремонта. </param>
        /// <param name="investedResources"> Список вложенных ресурсов. </param>
        public void UpdateView(RepairStage stage, List<int> investedResources)
        {
            ClearView();

            for (int i = 0; i < stage.Requirements.Count; i++)
            {
                var view = _requirementPool.Get();
                view.transform.SetAsLastSibling();
                view.Setup(stage.Requirements[i], investedResources[i]);

                view.OnAddClicked += _onAdd;
                view.OnReturnClicked += _onReturn;

                _requirementViews.Add(view);
            }

            _buildingNameText.text = stage.BuildingName;
            _buildButton.interactable = IsRepairPossible(stage, investedResources);
        }

        /// <summary> Очистить отображение окна ремонта. </summary>
        private void ClearView()
        {
            foreach (var view in _requirementViews) _requirementPool.Release(view);
            _requirementViews.Clear();
        }

        /// <summary> Проверить возможность завершения ремонта. </summary>
        /// <param name="stage"> Стадия ремонта. </param>
        /// <param name="investedResources"> Вложенные ресурсы. </param>
        /// <returns> <c>true</c>, если все ресурсы вложены; иначе <c>false</c>. </returns>
        private static bool IsRepairPossible(RepairStage stage, List<int> investedResources) =>
            !stage.Requirements.Where((itemStack, i) => investedResources[i] < itemStack.Number).Any();

        protected override void OnOpened()
        {
            base.OnOpened();
            _requirementViewsContainer.gameObject.SetActive(true);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            ClearView();

            _buildButton.onClick.RemoveListener(_onBuild.Invoke);
            
            _onAdd = null;
            _onReturn = null;
            _onBuild = null;
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