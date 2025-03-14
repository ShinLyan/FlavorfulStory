using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Control;
using FlavorfulStory.InputSystem;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.TooltipSystem;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Реализует возможность взаимодействия с объектами, используя триггеры. </summary>
    public class InteractFeature : MonoBehaviour
    {
        /// <summary> UI-объект для отображения тултипа взаимодействия. </summary>
        [SerializeField] private InteractableObjectTooltip _tooltip;

        /// <summary> PlayerController родительского объекта. </summary>
        private PlayerController _playerController;

        /// <summary> Аниматор для управления анимациями в процессе взаимодействия. </summary>
        private Animator _animator;

        /// <summary> Хэш для анимации сбора. </summary>
        private readonly int _gather = Animator.StringToHash("Gather");

        /// <summary> Список объектов, доступных для взаимодействия. </summary>
        private readonly List<IInteractable> _reachableInteractables = new();

        /// <summary> Ближайший объект, с которым можно взаимодействовать. </summary>
        private IInteractable _closestAllowedInteractable;

        /// <summary> Флаг, указывающий, происходит ли в данный момент взаимодействие. </summary>
        public bool IsInteracting { get; private set; }

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
            if (_playerController) _playerController.OnInteractionEnded += EndInteraction;

            _animator = GetComponentInParent<Animator>();
        }

        /// <summary> Проверяет нажатие кнопки взаимодействия и вызывает метод Interact()
        /// для ближайшего объекта. </summary>
        private void Update()
        {
            _closestAllowedInteractable = GetClosestAllowedInteractable();
            UpdateTooltip();

            if (IsInteracting || _closestAllowedInteractable == null ||
                !InputWrapper.GetButtonDown(InputButton.Interact)) return;

            BeginInteraction();
            _closestAllowedInteractable?.Interact();
        }

        /// <summary> Определяет ближайший объект для взаимодействия из доступных. </summary>
        /// <returns> Ближайший объект, с которым можно взаимодействовать, или null. </returns>
        private IInteractable GetClosestAllowedInteractable() => _reachableInteractables
            .Where(interactable => interactable.IsInteractionAllowed)
            .OrderBy(interactable => interactable.GetDistanceTo(transform))
            .FirstOrDefault();

        /// <summary> Обновить тултип. </summary>
        private void UpdateTooltip()
        {
            if (_closestAllowedInteractable != null) _tooltip.Show(_closestAllowedInteractable);
            else _tooltip.Hide();
        }

        /// <summary> Добавляет объект в список доступных для взаимодействия при входе в триггер. </summary>
        /// <param name="other"> Коллайдер объекта, вошедшего в триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

            _reachableInteractables.Add(interactable);

            if (other.TryGetComponent<IDestroyable>(out var destroyable))
                destroyable.OnObjectDestroyed += RemoveObjectFromList;
        }

        /// <summary> Удаляет объект из списка доступных для взаимодействия при выходе из триггера. </summary>
        /// <param name="other"> Коллайдер объекта, покинувшего триггер. </param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

            _reachableInteractables.Remove(interactable);

            if (other.TryGetComponent<IDestroyable>(out var destroyable))
                destroyable.OnObjectDestroyed -= RemoveObjectFromList;
        }

        /// <summary> Удаляет объект из списка заспавненных объектов. </summary>
        /// <param name="destroyable"> Объект, который необходимо удалить из списка. </param>
        private void RemoveObjectFromList(IDestroyable destroyable)
        {
            if (destroyable is not IInteractable interactable) return;

            destroyable.OnObjectDestroyed -= RemoveObjectFromList;
            _reachableInteractables.Remove(interactable);
        }

        // TODO: Убрать анимацию для ремонта
        /// <summary> Начать взаимодействие. </summary>
        private void BeginInteraction()
        {
            OnInteractionStarted?.Invoke();
            IsInteracting = true;
            if (_animator) _animator.SetTrigger(_gather);
            InputWrapper.BlockPlayerMovement();
        }

        /// <summary> Закончить взаимодействие. </summary>
        /// <remarks> Метод подписан на событие в анимации игрока (Gather_interaction). </remarks>
        private void EndInteraction()
        {
            OnInteractionEnded?.Invoke();
            IsInteracting = false;
            if (_animator) _animator.ResetTrigger(_gather);
            InputWrapper.UnblockPlayerMovement();
        }

        /// <summary> Отписка от события OnInteractionEnded (PlayerController.cs). </summary>
        private void OnDestroy()
        {
            if (_playerController) _playerController.OnInteractionEnded -= EndInteraction;
        }
    }
}