using System.Collections.Generic;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TooltipSystem;
using JetBrains.Annotations;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Реализует возможность взаимодействия с объектами, используя триггеры. </summary>
    [RequireComponent(typeof(Animator))]
    public class InteractFeature : MonoBehaviour
    {
        /// <summary> UI-объект для отображения тултипа взаимодействия. </summary>
        [SerializeField] private InteractableObjectToolTip _interactableObjectTooltip;

        /// <summary> Список объектов, доступных для взаимодействия. </summary>
        private readonly List<IInteractable> _reachableInteractables = new();

        /// <summary> Ближайший объект, с которым можно взаимодействовать. </summary>
        [CanBeNull] private IInteractable _nearestAllowedInteractable;

        /// <summary> Добавляет объект в список доступных для взаимодействия при входе в триггер. </summary>
        /// <param name="other"> Коллайдер объекта, вошедшего в триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                _reachableInteractables.Add(interactable);
            }
        }

        /// <summary> Обновляет ближайший объект для взаимодействия и отображение тултипа. </summary>
        /// <param name="other"> Коллайдер объекта, находящегося в триггере. </param>
        private void OnTriggerStay(Collider other)
        {
            _nearestAllowedInteractable = GetNearestAllowedInteractable();

            _interactableObjectTooltip.gameObject.SetActive(_nearestAllowedInteractable != null);

            if (_nearestAllowedInteractable == null) return;

            _interactableObjectTooltip.SetTitleAndDescription(_nearestAllowedInteractable);
            _interactableObjectTooltip.SetPositionWithOffset(_nearestAllowedInteractable);
        }

        /// <summary> Удаляет объект из списка доступных для взаимодействия при выходе из триггера. </summary>
        /// <param name="other"> Коллайдер объекта, покинувшего триггер. </param>
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                _reachableInteractables.Remove(interactable);
            }
        }

        /// <summary> Проверяет нажатие кнопки взаимодействия и вызывает метод Interact() для ближайшего объекта. </summary>
        private void Update()
        {
            if (InputWrapper.GetButtonDown(InputButton.Interact))
            {
                _nearestAllowedInteractable?.Interact();
            }
        }

        /// <summary> Определяет ближайший объект для взаимодействия из доступных. </summary>
        /// <returns> Ближайший объект, с которым можно взаимодействовать, или null. </returns>
        [CanBeNull]
        private IInteractable GetNearestAllowedInteractable()
        {
            IInteractable result = null;
            foreach (var interactable in _reachableInteractables)
            {
                if (result == null && interactable.IsInteractionAllowed())
                {
                    result = interactable;
                    continue;
                }

                if (interactable.GetDistanceTo(transform) < result?.GetDistanceTo(transform) &&
                    interactable.IsInteractionAllowed())
                {
                    result = interactable;
                }
            }

            return result;
        }
    }
}