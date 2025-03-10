using System;
using FlavorfulStory.ResourceContainer;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Собираемый одноразовый объект. </summary>
    /// <remarks>
    ///     После сбора ресурса/предмета уничтожается после определенной задержки.
    ///     Наследник от <see cref="HarvestableObject" />.
    /// </remarks>
    public class HarvestableOnce : HarvestableObject, IDestroyable
    {
        /// <summary> Задержка перед уничтожением. </summary>
        [Tooltip("Задержка перед уничтожением.")] [SerializeField]
        private float _destroyDelay;

        public bool IsDestroyed { get; private set; }
        public event Action<IDestroyable> OnObjectDestroyed;

        /// <summary> Уничтожить объект. </summary>
        /// <remarks> Уничтожение сработает спустя задержку <see cref="_destroyDelay" />. </remarks>
        public void Destroy()
        {
            IsDestroyed = true;
            OnObjectDestroyed?.Invoke(this);
            Destroy(gameObject, _destroyDelay);
        }

        /// <summary> Собрать ресурс. </summary>
        public override void Interact()
        {
            base.Interact();
            Destroy();
        }
    }
}