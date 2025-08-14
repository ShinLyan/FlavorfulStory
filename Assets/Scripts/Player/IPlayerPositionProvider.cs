using UnityEngine;

namespace FlavorfulStory.Player
{
    /// <summary> Провайдер позиции игрока. </summary>
    public interface IPlayerPositionProvider
    {
        /// <summary> Получить позицию игрока в мировых координатах. </summary>
        /// <returns> Позиция игрока. </returns>
        Vector3 GetPlayerPosition();

        /// <summary> Получить расстояние от игрока до целевой позиции. </summary>
        /// <param name="targetPosition"> Позиция цели. </param>
        /// <returns> Расстояние от игрока до целевой позиции. </returns>
        float GetDistanceTo(Vector3 targetPosition);
    }
}