using FlavorfulStory.Control;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Интерфейс для объектов, которые можно использовать. </summary>
    public interface IUsable
    {
        /// <summary> Кнопка мыши для использования предмета. </summary>
        /// <returns> Флаг успешности совершения действия. </returns>
        UseActionType UseActionType { get; }

        /// <summary> Использование объекта. </summary>
        /// <param name="player"> Игрок, использующий объект. </param>
        /// <param name="hitableLayers"> Слои, которые можно бить. </param>
        bool Use(PlayerController player, LayerMask hitableLayers);
    }
}