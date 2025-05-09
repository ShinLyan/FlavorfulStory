using System;
using FlavorfulStory.Player;
using FlavorfulStory.ResourceContainer;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Собираемый одноразовый объект. </summary>
    /// <remarks> После сбора ресурса/предмета уничтожается после определенной задержки.
    /// Наследник от <see cref="HarvestableObject" />. </remarks>
    public class HarvestableOnce : HarvestableObject, IDestroyable
    {
        /// <summary> Задержка перед уничтожением. </summary>
        [Tooltip("Задержка перед уничтожением."), SerializeField]
        private float _destroyDelay;

        /// <summary> Уничтожен ли объект? </summary>
        public bool IsDestroyed { get; private set; }

        /// <summary> Событие уничтожения объекта. </summary>
        public event Action<IDestroyable> OnObjectDestroyed;

        /// <summary> Собрать ресурс. </summary>
        public override void BeginInteraction(PlayerController player)
        {
            base.BeginInteraction(player);
            Destroy(_destroyDelay);
        }

        /// <summary> Уничтожить объект. </summary>
        /// <param name="destroyDelay"> Задержка перед уничтожением. </param>
        public void Destroy(float destroyDelay)
        {
            IsDestroyed = true;
            OnObjectDestroyed?.Invoke(this);
            Destroy(gameObject, destroyDelay);
        }
    }
}