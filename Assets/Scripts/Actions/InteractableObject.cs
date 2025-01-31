using FlavorfulStory.Control;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Интерактивный объект - абстрактный класс. </summary>
    public abstract class InteractableObject : MonoBehaviour
    {
        /// <summary> Отображаемое сообщение при взаимодействии. </summary>
        [Tooltip("Сообщение, которое отображается при наведении на объект.")]
        [field: SerializeField] public string InteractionMessage { get; protected set; }

        /// <summary> Метод, вызываемый при взаимодействии. </summary>
        /// <param name="player"> Игрок, инициировавший взаимодействие. </param>
        public abstract void Interact(PlayerController player);
    }
}