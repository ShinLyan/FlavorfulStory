using FlavorfulStory.Actions;
using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Предмет инвентаря, который можно съесть. </summary>
    /// <remarks> Является наследником класса <see cref="InventoryItem"/>. </remarks>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Edible Item")]
    public class EdibleInventoryItem : InventoryItem
    {
        /// <summary> Кнопка мыши для использования предмета. </summary>
        [field: Header("Edible Properties")]
        [field: Tooltip("Кнопка использования предмета."), SerializeField]
        public UseActionType UseActionType { get; private set; }

        /// <summary> Тип звуков поедания. </summary>
        [field: Tooltip("Тип звуков поедания."), SerializeField]
        public SfxType SfxType { get; private set; }
    }
}