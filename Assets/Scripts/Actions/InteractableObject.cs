using FlavorfulStory.Control;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Базовый класс для объектов, с которыми можно взаимодействовать. </summary>
    public abstract class InteractableObject : MonoBehaviour
    {
        /// <summary> Сообщение для отображения при взаимодействии. </summary>
        [Tooltip("Сообщение, которое отображается при наведении на объект.")]
        [field: SerializeField] public string InteractionMessage { get; protected set; }

        /// <summary> Взаимодействие с объектом. </summary>
        /// <param name="player"> Игрок, инициировавший взаимодействие. </param>
        public abstract void Interact(PlayerController player);
    }
}