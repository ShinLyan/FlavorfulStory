using System;
using UnityEngine;

namespace FlavorfulStory.Player
{
    /// <summary> Статический класс - модель игрока. </summary>
    public static class PlayerModel
    {
        /// <summary> Задержка между использованиями инструмента (в секундах). </summary>
        public const float ToolCooldown = 1.5f;

        /// <summary> Максимальное расстояние для взаимодействия с объектами. </summary>
        private const float InteractionDistance = 1.8f;

        /// <summary> Делегат, возвращающий текущую позицию игрока. </summary>
        private static Func<Vector3> _getPlayerPosition;

        /// <summary> Установить источник получения позиции игрока. </summary>
        /// <param name="getPosition"> Делегат, возвращающий позицию игрока. </param>
        public static void SetPositionProvider(Func<Vector3> getPosition) => _getPlayerPosition = getPosition;

        /// <summary> Находится ли игрок в пределах радиуса взаимодействия? </summary>
        /// <param name="targetPosition"> Позиция цели для проверки. </param>
        /// <returns> <c>true</c>, если игрок находится в зоне взаимодействия; иначе <c>false</c>. </returns>
        public static bool IsPlayerInRange(Vector3 targetPosition) =>
            Vector3.Distance(_getPlayerPosition(), targetPosition) <= InteractionDistance;
    }
}