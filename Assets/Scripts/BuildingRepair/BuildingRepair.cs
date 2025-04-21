using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InteractionSystem;
using FlavorfulStory.Audio;
using FlavorfulStory.InventorySystem;
using FlavorfulStory.ObjectManagement;
using FlavorfulStory.Player;
using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.BuildingRepair
{
    /// <summary> Ремонтируемое здание. </summary>
    /// <remarks> Управляет стадиями ремонта, состоянием объектов,
    /// взаимодействием с ресурсами и сохранением прогресса. </remarks>
    [RequireComponent(typeof(ObjectSwitcher))]
    public class BuildingRepair : MonoBehaviour, IInteractable, ISaveable
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

        /// <summary> Представление интерфейса ремонта здания. </summary>
        private BuildingRepairView _view;

        /// <summary> Текущая стадия ремонта. </summary>
        private RepairStage CurrentStage => _buildingData.Stages[_repairStageIndex];

        /// <summary> Завершен ли ремонт здания? </summary>
        private bool IsRepairCompleted => _repairStageIndex >= _buildingData.Stages.Count;

        /// <summary> Событие при обновлении стадии ремонта. </summary>
        private event Action<RepairStage, List<int>> _onStageUpdated;

        /// <summary> Событие при завершении ремонта. </summary>
        private event Action _onRepairCompleted;

        #endregion

        /// <summary> Инициализация компонента. </summary>
        private void Awake()
        {
            _objectSwitcher = GetComponent<ObjectSwitcher>();
            _view = FindFirstObjectByType<BuildingRepairView>(FindObjectsInactive.Include);
        }

        /// <summary> Запуск инициализации после загрузки сцены. </summary>
        private void Start()
        {
            InitializeInvestedResourcesList();
            InitializeRepairStages();
        }

        /// <summary> Инициализация списка вложенных ресурсов. </summary>
        /// <remarks> Список инвестированных ресурсов на текущей стадии будет инициализирован нулями. </remarks>
        private void InitializeInvestedResourcesList() =>
            _investedResources = CurrentStage.Requirements.Select(_ => 0).ToList();

        /// <summary> Инициализация стадий ремонта. </summary>
        private void InitializeRepairStages()
        {
            UpdateInteractionState();
            _objectSwitcher.Initialize();
            _objectSwitcher.SwitchTo(_repairStageIndex);
        }

        /// <summary> Обновить состояние возможности взаимодействия с ремонтируемым объектом. </summary>
        /// <remarks> После завершения ремонта взаимодействие становится невозможным. </remarks>
        private void UpdateInteractionState() => IsInteractionAllowed = !IsRepairCompleted;

        #region ITooltipable

        /// <summary> Получить название объекта для тултипа. </summary>
        /// <returns> Название объекта для тултипа. </returns>
        public string TooltipTitle => CurrentStage.BuildingName;

        /// <summary> Получить описание объекта для тултипа. </summary>
        /// <returns> Описание объекта для тултипа. </returns>
        public string TooltipDescription => _buildingData.Description;

        /// <summary> Получить мировую позицию объекта для взаимодействия. </summary>
        /// <returns> Мировая позиция объекта для взаимодействия. </returns>
        public Vector3 WorldPosition => transform.position;

        #endregion

        #region IInteractable

        /// <summary> Доступно ли взаимодействие с объектом в текущий момент? </summary>
        public bool IsInteractionAllowed { get; private set; }

        /// <summary> Получить расстояние до указанного объекта. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого нужно получить расстояние. </param>
        /// <returns> Расстояние до другого объекта в мировых координатах. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        /// <summary> Начинает процесс взаимодействия с объектом. </summary>
        /// <param name="player"> Игрок, который начал взаимодействие. </param>
        public void BeginInteraction(PlayerController player)
        {
            _onStageUpdated += _view.UpdateStageUI;
            _onRepairCompleted += _view.DisplayCompletionMessage;

            _view.Show(CurrentStage, _investedResources, TransferResource, Build,
                () =>
                {
                    _onStageUpdated -= _view.UpdateStageUI;
                    _onRepairCompleted -= _view.DisplayCompletionMessage;
                    EndInteraction(player);
                }
            );
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
            UpdateInteractionState();
            SfxPlayer.Instance.PlayOneShot(SfxType.Build);
            
            if (IsRepairCompleted)
            {
                _onRepairCompleted?.Invoke();
                return;
            }

            InitializeInvestedResourcesList();
            _onStageUpdated?.Invoke(CurrentStage, _investedResources);
        }

        /// <summary> Обработка передачи ресурса в ремонт или возврата. </summary>
        /// <param name="resource"> Ресурс, который передается в ремонт. </param>
        /// <param name="type"> Тип кнопки для добавления или возвращения ресурса. </param>
        private void TransferResource(InventoryItem resource, ResourceTransferButtonType type)
        {
            if (type == ResourceTransferButtonType.Add) AddResource(resource);
            else ReturnResource(resource);

            _onStageUpdated?.Invoke(CurrentStage, _investedResources);
        }

        /// <summary> Добавить ресурс в процесс ремонта. </summary>
        /// <param name="resource"> Ресурс, который будет добавлен в ремонт. </param>
        private void AddResource(InventoryItem resource)
        {
            int index = CurrentStage.Requirements.FindIndex(requirement => requirement.Item.ItemID == resource.ItemID);
            if (index == -1) return;

            var requirement = CurrentStage.Requirements[index];
            int available = Inventory.PlayerInventory.GetItemNumber(resource);
            int needed = requirement.Quantity - _investedResources[index];
            int toInvest = Mathf.Min(available, needed);
            if (toInvest <= 0) return;

            _investedResources[index] += toInvest;
            Inventory.PlayerInventory.RemoveItem(resource, toInvest);
        }

        /// <summary> Вернуть ресурс в инвентарь. </summary>
        /// <param name="resource"> Ресурс, который возвращается в инвентарь. </param>
        private void ReturnResource(InventoryItem resource)
        {
            int index = CurrentStage.Requirements.FindIndex(requirement => requirement.Item.ItemID == resource.ItemID);
            if (index == -1) return;

            int invested = _investedResources[index];
            if (invested <= 0 || !Inventory.PlayerInventory.HasSpaceFor(resource)) return;

            Inventory.PlayerInventory.TryAddToFirstAvailableSlot(resource, invested);
            _investedResources[index] = 0;
        }

        #region Saving

        /// <summary> Структура для сохранения состояния объекта ремонта. </summary>
        [Serializable]
        private struct RepairableBuildingRecord
        {
            /// <summary> Индекс текущей стадии ремонта. </summary>
            public int StageIndex;

            /// <summary> Количество вложенных ресурсов для текущей стадии ремонта. </summary>
            public List<int> InvestedResources;
        }

        /// <summary> Сохранить состояние объекта для дальнейшего восстановления. </summary>
        /// <returns> Объект состояния для последующего восстановления. </returns>
        public object CaptureState() => new RepairableBuildingRecord
        {
            StageIndex = _repairStageIndex,
            InvestedResources = _investedResources
        };

        /// <summary> Восстановить состояние объекта из сохранённого состояния. </summary>
        /// <param name="state"> Сохраненное состояние для восстановления. </param>
        public void RestoreState(object state)
        {
            if (state is not RepairableBuildingRecord data) return;

            _repairStageIndex = data.StageIndex;
            _investedResources = data.InvestedResources ?? new List<int>();
        }

        #endregion
    }
}