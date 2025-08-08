using FlavorfulStory.Actions;
using FlavorfulStory.Audio;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Tools
{
    /// <summary> Инструмент, используемый игроком для взаимодействия с объектами. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Inventory/Tool")]
    public class Tool : InventoryItem
    {
        /// <summary> Тип инструмента. </summary>
        [field: Header("Tool Properties")]
        [field: Tooltip("Тип инструмента."), SerializeField]
        public ToolType ToolType { get; private set; }

        /// <summary> Уровень инструмента. </summary>
        [field: Tooltip("Уровень инструмента."), SerializeField, Range(1f, 3f)]
        public int ToolLevel { get; private set; }

        /// <summary> Кнопка использования инструмента. </summary>
        [field: Tooltip("Кнопка использования инструмента."), SerializeField]
        public UseActionType UseActionType { get; private set; }

        /// <summary> Стоимость использования по выносливости. </summary>
        [field: Tooltip("Стоимость использования по выносливости."), SerializeField]
        public float StaminaCost { get; private set; }

        /// <summary> Тип SFX использования. </summary>
        [field: Tooltip("Тип SFX использования."), SerializeField]
        public SfxType SfxType { get; private set; }
    }
}