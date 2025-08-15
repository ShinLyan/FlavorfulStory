using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure;
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

        /// <summary> Пул для повторного использования требований к ремонту. </summary>
        private ObjectPool<ResourceRequirementView> _requirementPool;

        /// <summary> Список отображений требований ресурсов. </summary>
        private readonly List<ResourceRequirementView> _requirementViews = new();

        /// <summary> Открыто ли окно ремонта? </summary>
        private bool _isOpen;

        /// <summary> Делегат, вызываемый при добавлении ресурса. </summary>
        private Action<InventoryItem> _onAdd;

        /// <summary> Делегат, вызываемый при возврате ресурса. </summary>
        private Action<InventoryItem> _onReturn;

        /// <summary> Делегат, вызываемый при нажатии кнопки постройки. </summary>
        private Action _onBuild;

        /// <summary> Делегат, вызываемый при закрытии окна. </summary>
        private Action _onClose;

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
        /// <param name="onAdd"> Обработчик добавления ресурсов. </param>
        /// <param name="onReturn"> Обработчик забирания ресурсов. </param>
        /// <param name="onBuild"> Обработчик подтверждения ремонта. </param>
        /// <param name="onClose"> Обработчик закрытия окна ремонта. </param>
        public void Show(RepairStage stage, List<int> investedResources,
            Action<InventoryItem> onAdd, Action<InventoryItem> onReturn, Action onBuild, Action onClose)
        {
            _onAdd = onAdd;
            _onReturn = onReturn;
            _onBuild = onBuild;
            _onClose = onClose;

            Open();
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

        /// <summary> Открыть окно ремонта. </summary>
        private void Open()
        {
            _isOpen = true;
            gameObject.SetActive(true);
            _requirementViewsContainer.gameObject.SetActive(true);

            WorldTime.Pause();
            InputWrapper.BlockAllInput();
            InputWrapper.UnblockInput(InputButton.SwitchGameMenu);

            if (_onBuild != null) _buildButton.onClick.AddListener(_onBuild.Invoke);
        }

        /// <summary> Закрыть окно ремонта и сбросить состояние. </summary>
        public void Close()
        {
            _isOpen = false;
            gameObject.SetActive(false);

            WorldTime.Unpause();
            InputWrapper.UnblockAllInput();

            if (_onBuild != null) _buildButton.onClick.RemoveListener(_onBuild.Invoke);
            ClearView();

            _onAdd = null;
            _onReturn = null;
            _onBuild = null;

            _onClose?.Invoke();
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