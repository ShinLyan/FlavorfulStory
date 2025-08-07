using FlavorfulStory.Actions;
using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> Инструмент, используемый игроком для взаимодействия с объектами. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Tool")]
    public class Tool : InventoryItem
    {
        /// <summary> Тип инструмента. </summary>
        [field: Header("Tool Properties")]
        [field: Tooltip("Тип инструмента."), SerializeField]
        public ToolType ToolType { get; private set; }

        /// <summary> Кнопка использования инструмента. </summary>
        [field: Tooltip("Кнопка использования инструмента."), SerializeField]
        public UseActionType UseActionType { get; private set; }

        /// <summary> Стоимость использования по выносливости. </summary>
        [field: Tooltip("Стоимость использования по выносливости."), SerializeField]
        public float StaminaCost { get; private set; }

        /// <summary> Тип SFX использования. </summary>
        [field: Tooltip("Тип SFX использования."), SerializeField]
        public SfxType SfxType { get; private set; }

        // TODO: если появится
        // [field: SerializeField] public int Level { get; private set; }
    }
}