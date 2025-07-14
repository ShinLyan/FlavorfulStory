using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Обработчик коллизий NPC, вызывающий соответствующие методы при входе и выходе игрока из триггера. </summary>
    public class NpcCollisionHandler
    {
        /// <summary> Интерфейс обработчика коллизий персонажа, реализующий логику взаимодействия. </summary>
        private readonly ICharacterCollisionHandler _handler;

        /// <summary> Конструктор, сохраняющий ссылку на обработчик коллизий. </summary>
        /// <param name="handler"> Объект, реализующий обработку коллизий. </param>
        public NpcCollisionHandler(ICharacterCollisionHandler handler) => _handler = handler;

        /// <summary> Обрабатывает вход игрока в триггер коллизии. </summary>
        /// <param name="other"> Коллайдер, вошедший в триггер. </param>
        public void HandleTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _handler?.OnTriggerEntered(other);
        }

        /// <summary> Обрабатывает выход игрока из триггера коллизии. </summary>
        /// <param name="other"> Коллайдер, покинувший триггер. </param>
        public void HandleTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _handler?.OnTriggerExited(other);
        }
    }
}