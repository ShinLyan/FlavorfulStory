using FlavorfulStory.Actions;
using FlavorfulStory.Control;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Предмет инвентаря, который можно съесть. </summary>
    /// <remarks> Является насодеником класса <see cref="InventoryItem"/>. </remarks>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Edible Item")]
    public class EdibleInventoryItem : InventoryItem, IUsable, IEdible
    {
        /// <summary> Кнопка мыши для использования предмета. </summary>
        [field: Tooltip("Кнопка использования предмета."), SerializeField]
        public UseActionType UseActionType { get; set; }

        /// <summary> Съесть поедаемый предмет инвентаря. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        /// <param name="hitableLayers"></param>
        public void Use(PlayerController player, LayerMask hitableLayers)
        {
            Eat();

            //На будущее
            //Eat(player.GetComponent<PlayerStats>());
        }

        public void Eat()
        {
            Debug.Log("🍎 Ем вкусную еду. Восстановил HP и энергию.");
        }
    }
}