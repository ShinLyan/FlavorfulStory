using FlavorfulStory.InventorySystem;
using FlavorfulStory.InventorySystem.UI.Tooltips;
using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Спавнер тултипа с описанием предмета. </summary>
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        #region Override Methods

        /// <summary> Можно ли создать тултип?</summary>
        /// <remarks> Возвращает True, если спавнеру можно создать тултип. </remarks>
        protected override bool CanCreateTooltip() => GetComponent<IItemHolder>().GetItem();

        /// <summary> Вызывается, когда приходит время обновить информацию в префабе тултипа. </summary>
        /// <param name="tooltip"> Заспавненный префаб тултипа для обновления. </param>
        protected override void UpdateTooltip(GameObject tooltip)
        {
            if (!tooltip.TryGetComponent<ItemTooltip>(out var itemTooltip)) return;

            var item = GetComponent<IItemHolder>().GetItem();
            itemTooltip.Setup(item.ItemName, item.Description);
        }

        #endregion
    }
}