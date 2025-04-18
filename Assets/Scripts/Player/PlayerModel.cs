using UnityEngine;

namespace FlavorfulStory.Player
{
    public class PlayerModel : MonoBehaviour
    {
        private static PlayerModel _instance;

        private void Awake()
        {
            _instance = this;
        }

        /// <summary> Находится ли объект в радиусе досягаемости игрока? </summary>
        public static bool IsPlayerInRange(Vector3 targetPosition)
        {
            const float InteractionDistance = 1.8f;
            return Vector3.Distance(_instance.transform.position, targetPosition) <= InteractionDistance;
        }
    }
}