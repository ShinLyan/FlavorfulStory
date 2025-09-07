using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Actions;
using FlavorfulStory.Audio;
using FlavorfulStory.BuildingRepair.UI;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.ObjectManagement;
using FlavorfulStory.Player;
using FlavorfulStory.Saving;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using FlavorfulStory.Windows;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Ремонтируемое здание. </summary>
    [RequireComponent(typeof(ObjectSwitcher))]
    public class RepairableBuilding : MonoBehaviour, IInteractable, ISaveable
    {
        #region Fields and Properties

        /// <summary> Данные ремонтируемого здания. </summary>
        [Tooltip("Данные ремонтируемого здания."), SerializeField]
        private RepairableBuildingData _buildingData;

        /// <summary> Переключатель визуальных состояний объекта. </summary>
        private ObjectSwitcher _objectSwitcher;

        /// <summary> Текущий индекс стадии ремонта. </summary>
        private int _repairStageIndex;

        /// <summary> Количество вложенных ресурсов для текущей стадии ремонта. </summary>
        private List<int> _investedResources;

        /// <summary> Сервис окон. </summary>
        private IWindowService _windowService;

        /// <summary> Провайдер окон. </summary>
        private IInventoryProvider _inventoryProvider;

        /// <summary> Текущая стадия ремонта. </summary>
        private RepairStage CurrentStage => _buildingData.Stages[_repairStageIndex];

        /// <summary> Завершен ли ремонт здания? </summary>
        private bool IsRepairCompleted => _repairStageIndex >= _buildingData.Stages.Count;

        /// <summary> Событие при обновлении стадии ремонта. </summary>
        private event Action<RepairStage, List<int>> _onStageUpdated;

        /// <summary> Событие при завершении ремонта. </summary>
        public static event Action<RepairableBuildingName> OnRepairCompleted;

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="windowService"> Сервис окон. </param>
        /// <param name="inventoryProvider"> Провайдер инвентарей. </param>
        [Inject]
        private void Construct(IWindowService windowService, IInventoryProvider inventoryProvider)
        {
            _windowService = windowService;
            _inventoryProvider = inventoryProvider;
        }

        /// <summary> Инициализация компонента. </summary>
        private void Awake() => _objectSwitcher = GetComponent<ObjectSwitcher>();

        /// <summary> Запуск инициализации после загрузки сцены. </summary>
        private void Start()
        {
            if (_investedResources == null) InitializeInvestments();
            _objectSwitcher.Initialize();
            _objectSwitcher.SwitchTo(_repairStageIndex);
        }

        /// <summary> Инициализация вложений. </summary>
        private void InitializeInvestments() => _investedResources = CurrentStage.Requirements.Select(_ => 0).ToList();

        #region IInteractable

        /// <summary> Описание действия с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Build, CurrentStage.BuildingName);

        /// <summary> Доступно ли взаимодействие с объектом в текущий момент? </summary>
        public bool IsInteractionAllowed => !IsRepairCompleted;

        /// <summary> Получить расстояние до указанного объекта. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого нужно получить расстояние. </param>
        /// <returns> Расстояние до другого объекта в мировых координатах. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        /// <summary> Начинает процесс взаимодействия с объектом. </summary>
        /// <param name="player"> Игрок, который начал взаимодействие. </param>
        public void BeginInteraction(PlayerController player)
        {
            var window = _windowService.GetWindow<RepairableBuildingWindow>();
            _onStageUpdated += window.UpdateView;
            OnRepairCompleted += window.DisplayCompletionMessage;

            window.Closed += () =>
            {
                _onStageUpdated -= window.UpdateView;
                OnRepairCompleted -= window.DisplayCompletionMessage;
                EndInteraction(player);
            };
            window.Setup(CurrentStage, _investedResources, AddResource, ReturnResource, Build);
            window.Open();
        }

        /// <summary> Завершает взаимодействие. </summary>
        /// <param name="player"> Игрок, завершивший взаимодействие. </param>
        public void EndInteraction(PlayerController player) => player.SetBusyState(false);

        #endregion

        /// <summary> Завершить текущую стадию и перейти к следующей. </summary>
        /// <remarks> Переходит к следующей стадии ремонта, если все ресурсы добавлены. </remarks>
        private void Build()
        {
            if (IsRepairCompleted) return;

            _repairStageIndex++;
            _objectSwitcher.SwitchTo(_repairStageIndex);
            SfxPlayer.Play(SfxType.Build);
            if (IsRepairCompleted)
            {
                OnRepairCompleted?.Invoke(_buildingData.Name);
            }
            else
            {
                InitializeInvestments();
                _onStageUpdated?.Invoke(CurrentStage, _investedResources);
            }
        }

        /// <summary> Добавить ресурс в процесс ремонта. </summary>
        /// <param name="resource"> Ресурс, который будет добавлен в ремонт. </param>
        private void AddResource(InventoryItem resource)
        {
            int index = FindRequirementIndex(resource);
            if (index == -1) return;

            var requirement = CurrentStage.Requirements[index];
            int available = _inventoryProvider.GetPlayerInventory().GetItemNumber(resource);
            int needed = requirement.Number - _investedResources[index];
            int toInvest = Mathf.Min(available, needed);
            if (toInvest <= 0) return;

            _investedResources[index] += toInvest;
            _inventoryProvider.GetPlayerInventory().RemoveItem(resource, toInvest);

            _onStageUpdated?.Invoke(CurrentStage, _investedResources);
        }

        /// <summary> Вернуть ресурс в инвентарь. </summary>
        /// <param name="resource"> Ресурс, который возвращается в инвентарь. </param>
        private void ReturnResource(InventoryItem resource)
        {
            int index = FindRequirementIndex(resource);
            if (index < 0) return;

            int invested = _investedResources[index];
            if (invested <= 0 || !_inventoryProvider.GetPlayerInventory().HasSpaceFor(resource)) return;

            _inventoryProvider.GetPlayerInventory().TryAddToFirstAvailableSlot(resource, invested);
            _investedResources[index] = 0;

            _onStageUpdated?.Invoke(CurrentStage, _investedResources);
        }

        /// <summary> Найти индекс ресурса. </summary>
        /// <param name="item"> Ресурс для поиска. </param>
        /// <returns> Индекс ресурса. </returns>
        private int FindRequirementIndex(InventoryItem item) =>
            CurrentStage.Requirements.FindIndex(itemStack => itemStack.Item.ItemID == item.ItemID);

        #region ISaveable

        /// <summary> Структура для сохранения состояния объекта ремонта. </summary>
        [Serializable]
        private readonly struct RepairableBuildingRecord
        {
            /// <summary> Индекс текущей стадии ремонта. </summary>
            public int StageIndex { get; }

            /// <summary> Количество вложенных ресурсов для текущей стадии ремонта. </summary>
            public List<int> InvestedResources { get; }

            /// <summary> Конструктор с параметрами. </summary>
            /// <param name="stageIndex"> Индекс текущей стадии ремонта. </param>
            /// <param name="investedResources"> Количество вложенных ресурсов для текущей стадии ремонта. </param>
            public RepairableBuildingRecord(int stageIndex, List<int> investedResources)
            {
                StageIndex = stageIndex;
                InvestedResources = investedResources;
            }
        }

        /// <summary> Сохранить состояние объекта для дальнейшего восстановления. </summary>
        /// <returns> Объект состояния для последующего восстановления. </returns>
        public object CaptureState() => new RepairableBuildingRecord(_repairStageIndex, _investedResources);

        /// <summary> Восстановить состояние объекта из сохранённого состояния. </summary>
        /// <param name="state"> Сохраненное состояние для восстановления. </param>
        public void RestoreState(object state)
        {
            if (state is not RepairableBuildingRecord record) return;

            _repairStageIndex = record.StageIndex;
            _investedResources = record.InvestedResources;
        }

        #endregion
    }
}