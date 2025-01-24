using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TooltipSystem;
using JetBrains.Annotations;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Реализует возможность взаимодействия с объектами, используя триггеры. </summary>
    public class InteractFeature : MonoBehaviour
    {
        /// <summary> UI-объект для отображения тултипа взаимодействия. </summary>
        [SerializeField] private InteractableObjectToolTip _interactableObjectTooltip;

        /// <summary> Список объектов, доступных для взаимодействия. </summary>
        private readonly List<IInteractable> _reachableInteractables = new();

        /// <summary> Ближайший объект, с которым можно взаимодействовать. </summary>
        [CanBeNull] private IInteractable _nearestAllowedInteractable;

        /// <summary> Флаг, указывающий, происходит ли в данный момент взаимодействие. </summary>
        private bool _isInteracting = false;
        
        /// <summary> Аниматор для управления анимациями в процессе взаимодействия. </summary>
        private Animator _animator;
        
        /// <summary> Хэш для анимации сбора. </summary>
        private static readonly int gather = Animator.StringToHash("Gather");

        /// <summary> Событие, вызываемое при начале взаимодействия. Используется для звуковых эффектов и других действий. </summary>
        public event Action OnInteractionStarted; // На будущее - для звуков и тд
        
        /// <summary> Событие, вызываемое при завершении взаимодействия. Используется для звуковых эффектов и других действий. </summary>
        public event Action OnInteractionEnded; // На будущее - для звуков и тд
        
        /// <summary> Инициализация компонента. </summary>
        /// <remarks> Захват ссылки на аниматор. </remarks>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

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
            if (InputWrapper.GetButtonDown(InputButton.Interact) 
                && _nearestAllowedInteractable != null
                && !_isInteracting)
            {
                BeginInteraction();
                _nearestAllowedInteractable?.Interact();
            }
        }

        /// <summary> Определяет ближайший объект для взаимодействия из доступных. </summary>
        /// <returns> Ближайший объект, с которым можно взаимодействовать, или null. </returns>
        [CanBeNull]
        private IInteractable GetNearestAllowedInteractable()
        {
            return _reachableInteractables
                .Where(interactable => interactable.IsInteractionAllowed())
                .OrderBy(interactable => interactable.GetDistanceTo(transform))
                .FirstOrDefault();
        }

        /// <summary> Начать взаимодействие. </summary>
        private void BeginInteraction()
        {
            OnInteractionStarted?.Invoke();
            _isInteracting = true;
            _animator.SetTrigger(gather);
            InputWrapper.BlockInput(new[] { InputButton.Horizontal, InputButton.Vertical });
        }

        /// <summary> Закончить взаимодействие. </summary>
        /// <remarks> Метол подписан на событие в анимации игрока (Gather_interaction). </remarks>
        private void EndInteraction()
        {
            OnInteractionEnded?.Invoke();
            _isInteracting = false;
            _animator.ResetTrigger(gather);
            InputWrapper.UnblockInput(new[] { InputButton.Horizontal, InputButton.Vertical });
        }
    }
}