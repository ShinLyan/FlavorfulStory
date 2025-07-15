using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Интерфейс для обработки коллизий персонажа при входе и выходе из триггеров. </summary>
    public interface ICharacterCollisionHandler
    {
        /// <summary> Вызывается при входе другого коллайдера в триггер. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        void OnTriggerEntered(Collider other);

        /// <summary> Вызывается при выходе другого коллайдера из триггера. </summary>
        /// <param name="other"> Коллайдер, покинувший триггер. </param>
        void OnTriggerExited(Collider other);
    }
}