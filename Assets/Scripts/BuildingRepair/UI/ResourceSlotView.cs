using FlavorfulStory.InventorySystem;
using FlavorfulStory.TooltipSystem;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.BuildingRepair.UI
{
    /// <summary> Визуальное отображение одного ресурса, использованного при ремонте. </summary>
    /// <remarks> Показывает иконку ресурса и позволяет отобразить его тултип. </remarks>
    [RequireComponent(typeof(ItemTooltipSpawner))]
    public class ResourceSlotView : MonoBehaviour, IItemHolder
    {
        /// <summary> UI-иконка ресурса, отображаемая в этом слоте. </summary>
        [SerializeField] private Image _resourceIcon;

        /// <summary> Текущий ресурс, отображаемый в этом слоте. </summary>
        private InventoryItem _resource;

        /// <summary> Возвращает предмет, который отображается в этом слоте. </summary>
        public InventoryItem GetItem() => _resource;

        /// <summary> Устанавливает ресурс и обновляет его визуальное отображение. </summary>
        /// <param name="resource"> Ресурс, который будет отображаться. </param>
        public void Setup(InventoryItem resource)
        {
            _resource = resource;
            _resourceIcon.sprite = resource.Icon;
        }
    }
}