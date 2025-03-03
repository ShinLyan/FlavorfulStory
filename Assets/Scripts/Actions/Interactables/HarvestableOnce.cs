using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Собираемый одноразовый объект. </summary>
    /// <remarks>
    ///     После сбора ресурса/предмета уничтожается после определенной задержки.
    ///     Наследник от <see cref="AbstractHarvestableObject" />.
    /// </remarks>
    public class HarvestableOnce : AbstractHarvestableObject
    {
        /// <summary> Задержа перед уничтожением. </summary>
        [Tooltip("Задержка перед уничтожением.")] [SerializeField]
        private float _destroyDelay;

        /// <summary> Собрать ресурс. </summary>
        public override void Interact()
        {
            base.Interact();
            Destroy(gameObject, _destroyDelay);
        }
    }
}