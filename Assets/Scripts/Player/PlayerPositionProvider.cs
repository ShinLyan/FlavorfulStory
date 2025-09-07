using UnityEngine;

namespace FlavorfulStory.Player
{
    /// <summary> Провайдер позиции игрока. </summary>
    public class PlayerPositionProvider : IPlayerPositionProvider
    {
        /// <summary> Контроллер игрока. </summary>
        private readonly PlayerController _playerController;

        /// <summary> Создать провайдер позиции игрока. </summary>
        /// <param name="playerController"> Контроллер игрока. </param>
        public PlayerPositionProvider(PlayerController playerController) =>
            _playerController = playerController;

        /// <summary> Получить позицию игрока в мировых координатах. </summary>
        /// <returns> Позиция игрока. </returns>
        public Vector3 GetPlayerPosition() => _playerController.transform.position;

        /// <summary> Получить расстояние от игрока до целевой позиции. </summary>
        /// <param name="targetPosition"> Позиция цели. </param>
        /// <returns> Расстояние от игрока до целевой позиции. </returns>
        public float GetDistanceTo(Vector3 targetPosition) =>
            Vector3.Distance(_playerController.transform.position, targetPosition);
    }
}