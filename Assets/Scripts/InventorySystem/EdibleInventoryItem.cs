using FlavorfulStory.Actions;
using FlavorfulStory.Audio;
using FlavorfulStory.Player;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Предмет инвентаря, который можно съесть. </summary>
    /// <remarks> Является наследником класса <see cref="InventoryItem"/>. </remarks>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Edible Item")]
    public class EdibleInventoryItem : InventoryItem, IUsable, IEdible
    {
        /// <summary> Кнопка мыши для использования предмета. </summary>
        [field: Tooltip("Кнопка использования предмета."), SerializeField]
        public UseActionType UseActionType { get; private set; }

        /// <summary> Тип звуков поедания. </summary>
        [field: Tooltip("Тип звуков поедания."), SerializeField]
        public SfxType SfxType { get; private set; }

        /// <summary> Съесть поедаемый предмет инвентаря. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        /// <param name="hitableLayers"> Слои, которые можно бить. </param>
        public bool Use(PlayerController player, LayerMask hitableLayers)
        {
            Eat();

            return true;
            // TODO: На будущее
            //Eat(player.GetComponent<PlayerStats>());
        }

        /// <summary> Съесть предмет и применить его эффект к игроку. </summary>
        public void Eat()
        {
            SfxPlayer.Play(SfxType);
            Debug.Log("🍎 Ем вкусную еду. Восстановил HP и энергию.");
        }
    }
}