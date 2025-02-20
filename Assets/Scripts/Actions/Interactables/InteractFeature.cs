using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Control;
using FlavorfulStory.InputSystem;
using FlavorfulStory.TooltipSystem;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Реализует возможность взаимодействия с объектами, используя триггеры. </summary>
    public class InteractFeature : MonoBehaviour
    {
        /// <summary> UI-объект для отображения тултипа взаимодействия. </summary>
        [SerializeField] private InteractableObjectTooltip _interactableObjectTooltip;

        /// <summary> Аниматор для управления анимациями в процессе взаимодействия. </summary>
        [SerializeField] private Animator _animator;

        /// <summary> Ближайший объект, с которым можно взаимодействовать. </summary>
        private IInteractable _nearestAllowedInteractable;

        /// <summary> Список объектов, доступных для взаимодействия. </summary>
        private readonly List<IInteractable> _reachableInteractables = new();

        /// <summary> PlayerController родительского объекта. </summary>
        private PlayerController _playerController;

        /// <summary> Флаг, указывающий, происходит ли в данный момент взаимодействие. </summary>
        private bool _isInteracting;

        /// <summary> Хэш для анимации сбора. </summary>
        private readonly int _gather = Animator.StringToHash("Gather");

        /// <summary> Событие, вызываемое при начале взаимодействия.
        /// Используется для звуковых эффектов и других действий. </summary>
        public event Action OnInteractionStarted; // На будущее - для звуков и тд

        /// <summary> Событие, вызываемое при завершении взаимодействия.
        /// Используется для звуковых эффектов и других действий. </summary>
        public event Action OnInteractionEnded; // На будущее - для звуков и тд

        /// <summary> Инициализация компонента. </summary>
        /// <remarks> Подписка на событие OnInteractionEnded (PlayerController.cs). </remarks>
        private void Awake()
        {
            _playerController = GetComponentInParent<PlayerController>();
            _playerController.OnInteractionEnded += EndInteraction;
        }

        /// <summary> Отписка от события OnInteractionEnded (PlayerController.cs). </summary>
        private void OnDestroy() => _playerController.OnInteractionEnded -= EndInteraction;

        /// <summary> Проверяет нажатие кнопки взаимодействия и вызывает метод Interact() для ближайшего объекта. </summary>
        private void Update()
        {
            if (InputWrapper.GetButtonDown(InputButton.Interact) &&
                _nearestAllowedInteractable != null && !_isInteracting)
            {
                BeginInteraction();
                _nearestAllowedInteractable?.Interact();
            }
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

        /// <summary> Определяет ближайший объект для взаимодействия из доступных. </summary>
        /// <returns> Ближайший объект, с которым можно взаимодействовать, или null. </returns>
        private IInteractable GetNearestAllowedInteractable() =>
            _reachableInteractables.Where(interactable => interactable.IsInteractionAllowed)
                .OrderBy(interactable => interactable.GetDistanceTo(transform))
                .FirstOrDefault();

        /// <summary> Начать взаимодействие. </summary>
        private void BeginInteraction()
        {
            OnInteractionStarted?.Invoke();
            _isInteracting = true;
            _animator.SetTrigger(_gather);
            InputWrapper.BlockPlayerMovement();
        }

        /// <summary> Закончить взаимодействие. </summary>
        /// <remarks> Метол подписан на событие в анимации игрока (Gather_interaction). </remarks>
        private void EndInteraction()
        {
            OnInteractionEnded?.Invoke();
            _isInteracting = false;
            _animator.ResetTrigger(_gather);
            InputWrapper.UnblockPlayerMovement();
        }
    }
}