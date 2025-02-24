using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FlavorfulStory.Actions.Interactables;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.Saving;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Ремонтируемое здание. </summary>
    /// <remarks> Управляет стадиями ремонта, состоянием объектов, взаимодействием с ресурсами и сохранением прогресса. </remarks>
    public class BuildingRepair : MonoBehaviour, IInteractable, ISaveable
    {
        /// <summary> Стадии ремонта здания. </summary>
        [Tooltip("Стадии строительства"), SerializeField]
        private List<RepairStage> _stages;

        /// <summary> Представление UI ремонта текущего здания. </summary>
        private BuildingRepairView _repairView;

        /// <summary> Количество вложенных ресурсов для текущей стадии ремонта. </summary>
        private List<int> _investedResources;

        /// <summary> Индекс текущей стадии ремонта. </summary>
        private int _repairStageIndex;

        /// <summary> Завершен ли ремонт здания? </summary>
        private bool RepairCompleted => _repairStageIndex >= _stages.Count;

        /// <summary> Визуальные объекты для каждой стадии ремонта, включая начальную стадию. </summary>
        private List<GameObject> _stagesVisuals;

        /// <summary> Инициализация объекта. </summary>
        private void Awake()
        {
            _repairView = FindFirstObjectByType<BuildingRepairView>(FindObjectsInactive.Include);
            _investedResources = new List<int>();
        }

        /// <summary> Загрузка состояния или установка значений по умолчанию. </summary>
        private void Start()
        {
            InitializeRepairStages();
            InitializeInvestedResourcesList();

            // TODO: На рефакторинг
            IsBlockingMovement = true;
        }

        /// <summary> Инициализация стадий ремонта. </summary>
        private void InitializeRepairStages()
        {
            UpdateInteractionState();
            SpawnStagesVisuals();
            UpdateVisualStage();
        }

        /// <summary> Инициализация списка вложенных ресурсов. </summary>
        /// <remarks> Список инвестированных ресурсов на текущей стадии будет инициализирован нулями. </remarks>
        private void InitializeInvestedResourcesList()
        {
            _investedResources = _stages[_repairStageIndex].Requirements.Select(r => 0).ToList();
        }

        /// <summary> Обновить состояние возможности взаимодействия с ремонтируемым объектом. </summary>
        /// <remarks> После завершения ремонта взаимодействие становится невозможным. </remarks>
        private void UpdateInteractionState() => IsInteractionAllowed = !RepairCompleted;

        /// <summary> Заспавнить визуальные объекты для стадий ремонта. </summary>
        /// <remarks> Создает и добавляет все визуальные элементы для стадий ремонта в сцену. </remarks>
        private void SpawnStagesVisuals()
        {
            if (_stagesVisuals != null) return;

            _stagesVisuals = new List<GameObject>(_stages.Count + 1) { transform.GetChild(0).gameObject };
            _stagesVisuals.AddRange(_stages.Select(stage =>
                Instantiate(stage.StagePrefab, transform, false)));
        }

        /// <summary> Обновить визуализацию текущей стадии ремонта. </summary>
        private void UpdateVisualStage()
        {
            for (int i = 0; i < _stagesVisuals.Count; i++)
                _stagesVisuals[i].SetActive(i == _repairStageIndex);
        }

        /// <summary> Проведение ремонта. </summary>
        /// <remarks> Переходит к следующей стадии ремонта, если все ресурсы добавлены. </remarks>
        private void Build()
        {
            if (RepairCompleted) return;

            _repairStageIndex++;
            UpdateVisualStage();
            UpdateInteractionState();

            if (RepairCompleted)
            {
                _repairView.DisplayCompletionMessage();
                return;
            }

            InitializeInvestedResourcesList();
            _repairView.SetData(_stages[_repairStageIndex], _investedResources);
            _repairView.Close();
        }

        /// <summary> Передать ресурс в процесс ремонта. </summary>
        /// <param name="resource"> Ресурс, который передается в ремонт. </param>
        /// <param name="type"> Тип кнопки для добавления или возвращения ресурса. </param>
        private void TransferResource(InventoryItem resource, ResourceTransferButtonType type)
        {
            if (type == ResourceTransferButtonType.Add) AddResource(resource);
            else ReturnResource(resource);

            _repairView.SetData(_stages[_repairStageIndex], _investedResources);
            _repairView.BuildButton.IsInteractable = IsRepairPossible();
        }

        /// <summary> Добавить ресурс в процесс ремонта. </summary>
        /// <param name="resource"> Ресурс, который будет добавлен в ремонт. </param>
        private void AddResource(InventoryItem resource)
        {
            int investedResourceIndex = _stages[_repairStageIndex].Requirements
                .FindIndex(r => r.Item.ItemID == resource.ItemID);
            if (investedResourceIndex == -1) return;

            var requirement = _stages[_repairStageIndex].Requirements[investedResourceIndex];
            int investedNumber = Mathf.Min(requirement.Quantity - _investedResources[investedResourceIndex],
                Inventory.PlayerInventory.GetItemNumber(resource));
            if (investedNumber <= 0) return;

            _investedResources[investedResourceIndex] += investedNumber;
            Inventory.PlayerInventory.RemoveItem(resource, investedNumber);
        }

        /// <summary> Вернуть ресурс в инвентарь. </summary>
        /// <param name="resource"> Ресурс, который возвращается в инвентарь. </param>
        private void ReturnResource(InventoryItem resource)
        {
            int investedResourceIndex = _stages[_repairStageIndex].Requirements
                .FindIndex(requirement => requirement.Item.ItemID == resource.ItemID);
            if (investedResourceIndex == -1) return;

            int investedResourceNumber = _investedResources[investedResourceIndex];
            if (investedResourceNumber <= 0 || !Inventory.PlayerInventory.HasSpaceFor(resource)) return;

            Inventory.PlayerInventory.TryAddToFirstEmptySlot(resource, investedResourceNumber);
            _investedResources[investedResourceIndex] = 0;
        }

        /// <summary> Возможно ли совершить ремонт? </summary>
        /// <returns> <c>true</c>, если все ресурсы для текущей стадии вложены, иначе <c>false</c>. </returns>
        private bool IsRepairPossible() => !RepairCompleted && _stages[_repairStageIndex].Requirements
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
            _repairView.Open();
            _repairView.SetData(_stages[_repairStageIndex], _investedResources);
            _repairView.BuildButton.OnClick += Build;
            _repairView.BuildButton.IsInteractable = IsRepairPossible();
        }

        /// <summary> Получить расстояние до указанного объекта. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого нужно получить расстояние. </param>
        /// <returns> Расстояние до другого объекта в мировых координатах. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        #endregion

        #region Saving

        /// <summary> Структура для сохранения состояния объекта ремонта. </summary>
        [System.Serializable]
        private struct RepairableBuildingRecord
        {
            /// <summary> Индекс текущей стадии ремонта. </summary>
            public int RepairStageIndex;

            /// <summary> Количество вложенных ресурсов для текущей стадии ремонта. </summary>
            public List<int> InvestedResources;
        }

        /// <summary> Сохранить состояние объекта для дальнейшего восстановления. </summary>
        /// <returns> Объект состояния для последующего восстановления. </returns>
        public object CaptureState() => new RepairableBuildingRecord
        {
            RepairStageIndex = _repairStageIndex,
            InvestedResources = _investedResources
        };

        /// <summary> Восстановить состояние объекта из сохранённого состояния. </summary>
        /// <param name="state"> Сохраненное состояние для восстановления. </param>
        public void RestoreState(object state)
        {
            var record = state is RepairableBuildingRecord buildingRecord ? buildingRecord : default;
            _repairStageIndex = record.RepairStageIndex;
            _investedResources = record.InvestedResources;
            InitializeRepairStages();
        }

        #endregion
    }
}