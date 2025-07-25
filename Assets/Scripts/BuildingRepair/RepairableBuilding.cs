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

        /// <summary> Представление интерфейса ремонта здания. </summary>
        private RepairableBuildingView _view;

        /// <summary> Завершен ли ремонт здания? </summary>
        private bool _isRepairCompleted;

        /// <summary> Инвентарь игрока. </summary>
        private Inventory _playerInventory;

        /// <summary> Текущая стадия ремонта. </summary>
        private RepairStage CurrentStage => _buildingData.Stages[_repairStageIndex];

        /// <summary> Событие при обновлении стадии ремонта. </summary>
        private event Action<RepairStage, List<int>> _onStageUpdated;

        /// <summary> Событие при завершении ремонта. </summary>
        public static event Action<RepairableBuildingName> OnRepairCompleted;

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="inventory"> Инвентарь игрока. </param>
        /// <param name="view"> Визуальное представление ремонта зданий. </param>
        [Inject]
        private void Construct(Inventory inventory, RepairableBuildingView view)
        {
            _playerInventory = inventory;
            _view = view;
        }

        /// <summary> Инициализация компонента. </summary>
        private void Awake() => _objectSwitcher = GetComponent<ObjectSwitcher>();

        /// <summary> Запуск инициализации после загрузки сцены. </summary>
        private void Start()
        {
            _investedResources = CurrentStage.Requirements.Select(_ => 0).ToList();
            _objectSwitcher.Initialize();

            // На случай системы сохранения
            int index = _isRepairCompleted ? _repairStageIndex + 1 : _repairStageIndex;
            _objectSwitcher.SwitchTo(index);
        }

        #region IInteractable

        /// <summary> Описание действия с объектом. </summary>
        public ActionTooltipData ActionTooltip => new("E", ActionType.Build, CurrentStage.BuildingName);

        /// <summary> Доступно ли взаимодействие с объектом в текущий момент? </summary>
        public bool IsInteractionAllowed => !_isRepairCompleted;

        /// <summary> Получить расстояние до указанного объекта. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого нужно получить расстояние. </param>
        /// <returns> Расстояние до другого объекта в мировых координатах. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(otherTransform.position, transform.position);

        /// <summary> Начинает процесс взаимодействия с объектом. </summary>
        /// <param name="player"> Игрок, который начал взаимодействие. </param>
        public void BeginInteraction(PlayerController player)
        {
            _onStageUpdated += _view.UpdateView;
            OnRepairCompleted += _view.DisplayCompletionMessage;

            _view.Show(CurrentStage, _investedResources, AddResource, ReturnResource, Build,
                () =>
                {
                    _onStageUpdated -= _view.UpdateView;
                    OnRepairCompleted -= _view.DisplayCompletionMessage;
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
            if (_isRepairCompleted) return;

            if (_repairStageIndex + 1 >= _buildingData.Stages.Count)
            {
                _isRepairCompleted = true;
                _objectSwitcher.SwitchTo(_repairStageIndex + 1);
                OnRepairCompleted?.Invoke(_buildingData.Name);
                return;
            }

            _repairStageIndex++;
            _objectSwitcher.SwitchTo(_repairStageIndex);
            SfxPlayer.Play(SfxType.Build);

            _onStageUpdated?.Invoke(CurrentStage, _investedResources);
        }

        /// <summary> Добавить ресурс в процесс ремонта. </summary>
        /// <param name="resource"> Ресурс, который будет добавлен в ремонт. </param>
        private void AddResource(InventoryItem resource)
        {
            int index = CurrentStage.Requirements.FindIndex(requirement => requirement.Item.ItemID == resource.ItemID);
            if (index == -1) return;

            var requirement = CurrentStage.Requirements[index];
            int available = _playerInventory.GetItemNumber(resource);
            int needed = requirement.Number - _investedResources[index];
            int toInvest = Mathf.Min(available, needed);
            if (toInvest <= 0) return;

            _investedResources[index] += toInvest;
            _playerInventory.RemoveItem(resource, toInvest);

            _onStageUpdated?.Invoke(CurrentStage, _investedResources);
        }

        /// <summary> Вернуть ресурс в инвентарь. </summary>
        /// <param name="resource"> Ресурс, который возвращается в инвентарь. </param>
        private void ReturnResource(InventoryItem resource)
        {
            int index = CurrentStage.Requirements.FindIndex(requirement => requirement.Item.ItemID == resource.ItemID);
            if (index == -1) return;

            int invested = _investedResources[index];
            if (invested <= 0 || !_playerInventory.HasSpaceFor(resource)) return;

            _playerInventory.TryAddToFirstAvailableSlot(resource, invested);
            _investedResources[index] = 0;

            _onStageUpdated?.Invoke(CurrentStage, _investedResources);
        }

        #region Saving

        /// <summary> Структура для сохранения состояния объекта ремонта. </summary>
        [Serializable]
        private struct RepairableBuildingRecord
        {
            /// <summary> Индекс текущей стадии ремонта. </summary>
            public int StageIndex;

            /// <summary> Завершён ли ремонт? </summary>
            public bool IsRepairCompleted;

            /// <summary> Количество вложенных ресурсов для текущей стадии ремонта. </summary>
            public List<int> InvestedResources;
        }

        /// <summary> Сохранить состояние объекта для дальнейшего восстановления. </summary>
        /// <returns> Объект состояния для последующего восстановления. </returns>
        public object CaptureState() => new RepairableBuildingRecord
        {
            StageIndex = _repairStageIndex,
            IsRepairCompleted = _isRepairCompleted,
            InvestedResources = _investedResources
        };

        /// <summary> Восстановить состояние объекта из сохранённого состояния. </summary>
        /// <param name="state"> Сохраненное состояние для восстановления. </param>
        public void RestoreState(object state)
        {
            if (state is not RepairableBuildingRecord data) return;

            _repairStageIndex = data.StageIndex;
            _isRepairCompleted = data.IsRepairCompleted;
            _investedResources = data.InvestedResources;
        }

        #endregion
    }
}