using System.Collections;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Собираемый многоразовый объект. </summary>
    /// <remarks> После сбора ресурса/предмета уходит на кулдаун.
    /// Наследник от <see cref="HarvestableObject" />. </remarks>
    public class HarvestableWithCooldown : HarvestableObject
    {
        /// <summary> Плод собираемого объекта. </summary>
        /// <remarks> Является включенным только если многоразовый объект доступен к сбору. </remarks>
        [Tooltip("Плод собираемого объекта."), SerializeField]
        private GameObject _fruit;

        /// <summary> Кулдаун сбора. </summary>
        [Tooltip("Кулдаун сбора ресурса/предмета"), SerializeField]
        private float _harvestCooldown;

        /// <summary> Флаг возможности взаимодействия с объектом. </summary>
        private bool _isInteractionAllowed = true;

        /// <summary> Возможность взаимодействия с объектом. </summary>
        /// <remarks> Автоматически включает/выключает отображение плода. </remarks>
        public override bool IsInteractionAllowed
        {
            get => _isInteractionAllowed;
            set
            {
                _isInteractionAllowed = value;
                if (_fruit) _fruit.SetActive(_isInteractionAllowed);
            }
        }

        /// <summary> Собрать предмет/ресурс. </summary>
        public override void Interact()
        {
            base.Interact();
            StartCoroutine(EnableInteractionAfterCooldown());
        }

        /// <summary> Включить возможность взаимодействия по прошествию кулдауна. </summary>
        /// <returns> Возвращает <see cref="IEnumerator" /> корутины. </returns>
        private IEnumerator EnableInteractionAfterCooldown()
        {
            yield return new WaitForSeconds(_harvestCooldown);
            IsInteractionAllowed = true;
        }
    }
}