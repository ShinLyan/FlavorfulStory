using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FlavorfulStory.Actions.Interactables;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Saving;
using FlavorfulStory.SceneManagement;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Класс, представляющий ремонтируемое здание в игре. </summary>
    /// <remarks> Управляет стадиями ремонта, состоянием объектов, взаимодействием с ресурсами и сохранением прогресса. </remarks>
    public class BuildingRepair : MonoBehaviour, IInteractable, ISaveable
    {
        /// <summary> Список стадий ремонта здания. </summary>
        [Tooltip("Стадии строительства"), SerializeField]
        private List<RepairStage> _stages;

        /// <summary> Вьюшка представления интерфейса ремонта для текущего здания. </summary>
        private BuildingRepairView _repairView;

        /// <summary> Список количества вложенных ресурсов для текущей стадии ремонта. </summary>
        private List<int> _investedResources;

        /// <summary> Индекс текущей стадии ремонта здания. </summary>
        private int _currentRepairStageIndex;

        /// <summary> Флаг, указывающий, завершен ли ремонт здания. </summary>
        private bool _repairCompleted;

        /// <summary> Список визуальных объектов для каждой стадии ремонта, включая начальную стадию. </summary>
        private List<GameObject> _stagesVisuals;

        /// <summary> Инициализировать объект. Коллбэк из UnityAPI. </summary>
        /// <remarks> Ищет вьюшку окна ремонта. </remarks>
        private void Awake()
        {
            _repairView = FindFirstObjectByType<BuildingRepairView>(FindObjectsInactive.Include);
            _investedResources = new();
        }

        /// <summary> Коллбэк из UnityAPI. </summary>
        /// <remarks> Загрузить состояние по умолчанию, если файл сохранения не существует. </remarks>
        private void Start()
        {
            if (!SavingWrapper.SaveFileExists) LoadDefaultStage();

            // TODO: На рефакторинг
            IsBlockingMovement = true;
        }

        /// <summary> Загрузить начальное состояние ремонта. </summary>
        /// <remarks> Сбрасывает стадии ремонта, ресурсы и инициализирует состояние объекта. </remarks>
        private void LoadDefaultStage()
        {
            InitializeInvestedResourcesList();
            UpdateInteractableState();
            SpawnStagesVisuals();
            UpdateVisualStage();
        }

        /// <summary> Инициализировать список вложенных ресурсов для текущей стадии. </summary>
        /// <remarks> Список инвестированных ресурсов на текущей стадии будет инициализирован нулями. </remarks>
        private void InitializeInvestedResourcesList()
        {
            _investedResources.Clear();
            for (int i = 0; i < _stages[_currentRepairStageIndex].Requirements.Count; i++)
                _investedResources.Add(0);
        }

        /// <summary> Обновить состояние возможности взаимодействия с ремонтируемым объектом. </summary>
        /// <remarks> После завершения ремонта взаимодействие становится невозможным. </remarks>
        private void UpdateInteractableState() => IsInteractionAllowed = !_repairCompleted;

        /// <summary> Заспавнить визуальные объекты для стадий ремонта. </summary>
        /// <remarks> Создает и добавляет все визуальные элементы для стадий ремонта в сцену. </remarks>
        private void SpawnStagesVisuals()
        {
            if (_stagesVisuals != null) return;

            _stagesVisuals = new(_stages.Count + 1);
            _stagesVisuals.Add(transform.GetChild(0).gameObject);
            foreach (var stage in _stages)
            {
                var stageGo = Instantiate(stage.StagePrefab, transform, false);
                _stagesVisuals.Add(stageGo);
            }
        }

        /// <summary> Обновить текущую визуализацию для стадии ремонта. </summary>
        private void UpdateVisualStage()
        {
            _stagesVisuals.ForEach(go => go.SetActive(false));
            _stagesVisuals[_currentRepairStageIndex + (_repairCompleted ? 1 : 0)].gameObject.SetActive(true);
        }

        /// <summary> Построить (продолжить ремонт) на основе вложенных ресурсов. </summary>
        /// <remarks> Переходит к следующей стадии ремонта, если все ресурсы добавлены. </remarks>
        private void Build()
        {
            _repairCompleted = _currentRepairStageIndex >= _stages.Count - 1;
            if (!_repairCompleted)
            {
                _currentRepairStageIndex++;
                InitializeInvestedResourcesList();
            }

            _repairView.SetData(_stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
            _repairView.BuildButton.IsInteractable = CanBeRepaired();
            UpdateInteractableState();
            UpdateVisualStage();
            if (!_repairCompleted) _repairView.Close();
        }

        /// <summary> Передать ресурс в процесс ремонта. </summary>
        /// <param name="resource"> Ресурс, который передается в ремонт. </param>
        /// <param name="transferButtonType"> Тип кнопки для добавления или возвращения ресурса. </param>
        private void TransferResource(InventoryItem resource, ResourceTransferButtonType transferButtonType)
        {
            switch (transferButtonType)
            {
                case ResourceTransferButtonType.Add:
                    AddResource(resource);
                    break;
                case ResourceTransferButtonType.Return:
                    ReturnResource(resource);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transferButtonType), transferButtonType, null);
            }

            _repairView.SetData(_stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
            _repairView.BuildButton.IsInteractable = CanBeRepaired();
        }

        /// <summary> Попробовать добавить ресурс в процесс ремонта. </summary>
        /// <param name="resource"> Ресурс, который будет добавлен в ремонт. </param>
        /// <returns> Флаг успешной операции. </returns>
        private void AddResource(InventoryItem resource)
        {
            int resourceRequirementNumber = _stages[_currentRepairStageIndex].Requirements
                .Find(requirement => requirement.Item.ItemID == resource.ItemID).Quantity;

            int investedResourceIndex = _stages[_currentRepairStageIndex].Requirements
                .FindIndex(x => x.Item.ItemID == resource.ItemID);

            if (_investedResources[investedResourceIndex] >= resourceRequirementNumber) return;

            if (!Inventory.PlayerInventory.HasItem(resource) || Inventory.PlayerInventory.GetItemNumber(resource) <= 0)
                return;

            int numberToInvest = resourceRequirementNumber - _investedResources[investedResourceIndex];
            int investedNumber = Math.Min(numberToInvest, Inventory.PlayerInventory.GetItemNumber(resource));
            _investedResources[investedResourceIndex] += investedNumber;
            Inventory.PlayerInventory.RemoveItem(resource, investedNumber);
        }

        /// <summary> Попробовать вернуть ресурс в инвентарь. </summary>
        /// <param name="resource"> Ресурс, который возвращается в инвентарь. </param>
        private void ReturnResource(InventoryItem resource)
        {
            int investedResourceIndex = _stages[_currentRepairStageIndex].Requirements
                .FindIndex(requirement => requirement.Item.ItemID == resource.ItemID);
            int investedResourceNumber = _investedResources[investedResourceIndex];

            // TODO: Убедиться в работоспособности HasSpaceFor(): учесть случай, когда инаентарь полон(не можем вернуть ресы).
            if (investedResourceNumber <= 0 || !Inventory.PlayerInventory.HasSpaceFor(resource)) return;

            Inventory.PlayerInventory.TryAddToFirstEmptySlot(resource, investedResourceNumber);
            _investedResources[investedResourceIndex] = 0;
        }

        /// <summary> Проверить, можно ли совершить ремонт. </summary>
        /// <returns> Возвращает <c>true</c>, если все ресурсы для текущей стадии вложены, иначе <c>false</c>. </returns>
        private bool CanBeRepaired() => !_repairCompleted && _stages[_currentRepairStageIndex].Requirements
            .Select((requirement, i) => _investedResources[i] >= requirement.Quantity)
            .All(requirementCompleted => requirementCompleted);

        #region Interactable

        /// <summary> Получить название объекта для тултипа. </summary>
        /// <returns> Название объекта для тултипа. </returns>
        public string GetTooltipTitle() => "Building";

        /// <summary> Получить описание объекта для тултипа. </summary>
        /// <returns> Описание объекта для тултипа. </returns>
        public string GetTooltipDescription() => "Repair me!";

        /// <summary> Получить мировую позицию объекта для взаимодействия. </summary>
        /// <returns> Мировая позиция объекта для взаимодействия. </returns>
        public Vector3 GetWorldPosition() => transform.position;

        /// <summary> Флаг, разрешающий взаимодействие с объектом. </summary>
        public bool IsInteractionAllowed { get; set; }

        /// <summary> Флаг, блокирующий передвижение персонажа. </summary>
        [field: SerializeField]
        public bool IsBlockingMovement { get; set; }

        /// <summary> Провести взаимодействие с объектом. </summary>
        /// <remarks> Инициализирует интерфейс и позволяет пользователю взаимодействовать с объектом. </remarks>
        public void Interact()
        {
            _repairView.Initialize(TransferResource);
            _repairView.BuildButton.OnClick += Build;
            _repairView.Open();
            _repairView.SetData(_stages[_currentRepairStageIndex], _investedResources, _repairCompleted);
            _repairView.BuildButton.IsInteractable = CanBeRepaired();
        }

        /// <summary> Получить расстояние до указанного объекта. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого нужно получить расстояние. </param>
        /// <returns> Расстояние до другого объекта в мировых координатах. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        #endregion

        #region Saving

        /// <summary> Структура для сохранения состояния объекта ремонта. </summary>
        [Serializable]
        private struct RepairableBuildingRecord
        {
            public List<int> InvestedResources;
            public int RepairStageIndex;
            public bool RepairCompleted;
        }

        /// <summary> Сохранить состояние объекта для дальнейшего восстановления. </summary>
        /// <returns> Объект состояния для последующего восстановления. </returns>
        public object CaptureState() => new RepairableBuildingRecord
        {
            RepairStageIndex = _currentRepairStageIndex,
            InvestedResources = _investedResources,
            RepairCompleted = _repairCompleted
        };

        /// <summary> Восстановить состояние объекта из сохранённого состояния. </summary>
        /// <param name="state"> Сохраненное состояние для восстановления. </param>
        public void RestoreState(object state)
        {
            var record = state is RepairableBuildingRecord buildingRecord ? buildingRecord : default;

            _investedResources = record.InvestedResources;
            _currentRepairStageIndex = record.RepairStageIndex;
            _repairCompleted = record.RepairCompleted;
            UpdateInteractableState();
            SpawnStagesVisuals();
            UpdateVisualStage();
        }

        #endregion
    }
}