using System.Collections;
using FlavorfulStory.Control;
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

        /// <summary> Перезарядка сбора. </summary>
        [Tooltip("Перезарядка сбора ресурса/предмета"), SerializeField]
        private float _harvestCooldown;

        /// <summary> Флаг возможности взаимодействия с объектом. </summary>
        private bool _isInteractionAllowed = true;

        /// <summary> Возможность взаимодействия с объектом. </summary>
        /// <remarks> Автоматически включает/выключает отображение плода. </remarks>
        public override bool IsInteractionAllowed
        {
            get => _isInteractionAllowed;
            protected set
            {
                _isInteractionAllowed = value;
                if (_fruit) _fruit.SetActive(_isInteractionAllowed);
            }
        }

        /// <summary> Собрать предмет/ресурс. </summary>
        public override void Interact(PlayerController player)
        {
            base.Interact(player);
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