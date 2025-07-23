using UnityEngine;
using Zenject;
using FlavorfulStory.InventorySystem;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Спавнер тултипа с описанием предмета. </summary>
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        /// <summary> Хранит компонент, содержащий предмет, для отображения его в тултипе. </summary>
        private IItemHolder _item;

        /// <summary> Внедряет префаб тултипа предмета. </summary>
        /// <param name="itemTooltipPrefab"> Префаб тултипа предмета. </param>
        [Inject]
        private void Construct(ItemTooltipView itemTooltipPrefab) => TooltipPrefab = itemTooltipPrefab.gameObject;

        /// <summary> Инициализация полей. </summary>
        private void Awake() => _item = GetComponent<IItemHolder>();

        #region Override Methods

        /// <summary> Можно ли создать тултип? </summary>
        /// <returns> <c>true</c>, если предмет существует и тултип можно создать; иначе <c>false</c>. </returns>
        protected override bool CanCreateTooltip() => _item != null && _item.GetItem();

        /// <summary> Обновляет содержимое тултипа на основе текущего предмета. </summary>
        /// <param name="tooltip"> Заспавненный префаб тултипа для обновления. </param>
        protected override void UpdateTooltip(GameObject tooltip)
        {
            if (!tooltip.TryGetComponent<ItemTooltipView>(out var itemTooltip)) return;

            itemTooltip.Setup(_item.GetItem());
        }

        #endregion
    }
}