using FlavorfulStory.InventorySystem.UI.Tooltips;
using UnityEngine;

namespace FlavorfulStory.InventorySystem.TooltipSystem
{
    /// <summary> Помещается в слот UI инвентаря для отображения тултипа предмета. </summary>
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        #region Override Methods
        /// <summary> Можно ли создать тултип?</summary>
        /// <remarks> Возвращает True, если спавнеру можно создать тултип. </remarks>
        public override bool CanCreateTooltip()
        {
            var item = GetComponent<IItemHolder>().GetItem();
            if (!item) return false;

            return true;
        }

        /// <summary> Вызывается, когда приходит время обновить информацию в префабе тултипа. </summary>
        /// <param name="tooltip"> Заспавненный префаб тултипа для обновления. </param>
        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemTooltip = tooltip.GetComponent<ItemTooltip>();
            if (itemTooltip == null) return;

            var item = GetComponent<IItemHolder>().GetItem();
            itemTooltip.Setup(item);
        }
        #endregion
    }
}