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
        #region Fields and Properties

        /// <summary> UI-объект для отображения тултипа взаимодействия. </summary>
        [SerializeField] private InteractableObjectTooltip _tooltip;

        /// <summary> PlayerController родительского объекта. </summary>
        private PlayerController _playerController;

        /// <summary> Аниматор для управления анимациями в процессе взаимодействия. </summary>
        private Animator _animator;

        //TODO: Когда будут определные все анимации => вынести в Enum(public Enum PlayerAnimation {} ).
        /// <summary> Хэш для анимации сбора. </summary>
        private readonly int _gatherAnimationHash = Animator.StringToHash("Gather");

        /// <summary> Список объектов, доступных для взаимодействия. </summary>
        private readonly List<IInteractable> _availableInteractables = new();

        /// <summary> Ближайший объект, с которым можно взаимодействовать. </summary>
        private IInteractable _closestInteractable;

        /// <summary> Делегат действия, вызываемый при завершении взаимодействия. </summary>
        private Action _endInteractionAction;

        /// <summary> Делегат действия, вызываемый при начале взаимодействия. </summary>
        private Action _startInteractionAction;

        /// <summary> Происходит ли в данный момент взаимодействие? </summary>
        public bool IsInteracting { get; private set; }

        /// <summary> Событие, вызываемое при начале взаимодействия.
        /// Используется для звуковых эффектов и других действий. </summary>
        /// TODO: На будущее - для звуков и тд
        public event Action OnInteractionStarted;

        /// <summary> Событие, вызываемое при завершении взаимодействия.
        /// Используется для звуковых эффектов и других действий. </summary>
        /// TODO: На будущее - для звуков и тд
        public event Action OnInteractionEnded;

        #endregion

        /// <summary> Устанавливает действия, вызываемые при начале и завершении взаимодействия. </summary>
        /// <param name="startInteractionAction"> Действие при начале взаимодействия. </param>
        /// <param name="endInteractionAction"> Действие при завершении взаимодействия. </param>
        public void SetInteractionActions(Action startInteractionAction, Action endInteractionAction)
        {
            _startInteractionAction = startInteractionAction;
            _endInteractionAction = endInteractionAction;
        }

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
            if (_availableInteractables.Count != 0) UpdateClosestInteractable();

            if (IsInteracting || _closestInteractable == null ||
                !InputWrapper.GetButtonDown(InputButton.Interact))
                return;

            BeginInteraction();
            _closestInteractable?.Interact();
        }

        /// <summary> Обновить ближайший интерактивный объект. </summary>
        private void UpdateClosestInteractable()
        {
            _closestInteractable = FindClosestInteractable();
            UpdateTooltip();
        }

        /// <summary> Определяет ближайший объект для взаимодействия из доступных. </summary>
        /// <returns> Ближайший объект, с которым можно взаимодействовать, или null. </returns>
        private IInteractable FindClosestInteractable() => _availableInteractables
            .Where(interactable => interactable.IsInteractionAllowed)
            .OrderBy(interactable => interactable.GetDistanceTo(transform))
            .FirstOrDefault();

        /// <summary> Обновить тултип. </summary>
        private void UpdateTooltip()
        {
            if (_closestInteractable != null)
                _tooltip.Show(_closestInteractable);
            else
                _tooltip.Hide();
        }

        /// <summary> Добавляет объект в список доступных для взаимодействия при входе в триггер. </summary>
        /// <param name="other"> Коллайдер объекта, вошедшего в триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

            _availableInteractables.Add(interactable);

            if (other.TryGetComponent<IDestroyable>(out var destroyable))
                destroyable.OnObjectDestroyed += RemoveInteractable;
        }

        /// <summary> Удаляет объект из списка доступных для взаимодействия при выходе из триггера. </summary>
        /// <param name="other"> Коллайдер объекта, покинувшего триггер. </param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

            _availableInteractables.Remove(interactable);
            UpdateClosestInteractable();

            if (other.TryGetComponent<IDestroyable>(out var destroyable))
                destroyable.OnObjectDestroyed -= RemoveInteractable;
        }

        /// <summary> Удаляет объект из списка заспавненных объектов. </summary>
        /// <param name="destroyable"> Объект, который необходимо удалить из списка. </param>
        private void RemoveInteractable(IDestroyable destroyable)
        {
            if (destroyable is not IInteractable interactable) return;

            destroyable.OnObjectDestroyed -= RemoveInteractable;
            _availableInteractables.Remove(interactable);
            UpdateClosestInteractable();
        }

        /// <summary> Начать взаимодействие. </summary>
        private void BeginInteraction()
        {
            _startInteractionAction();
            OnInteractionStarted?.Invoke();
            InputWrapper.BlockPlayerMovement();

            //TODO: Не проигрывать анимацию для ремонта
            if (_animator) _animator.SetTrigger(_gatherAnimationHash);
        }

        /// <summary> Закончить взаимодействие. </summary>
        /// <remarks> Метод подписан на событие в анимации игрока (Gather_interaction). </remarks>
        private void EndInteraction()
        {
            OnInteractionEnded?.Invoke();
            _endInteractionAction();

            if (_closestInteractable is BuildingRepair.BuildingRepair && !IsInteracting) return;

            InputWrapper.UnblockPlayerMovement();

            if (_animator) _animator.ResetTrigger(_gatherAnimationHash);
        }

        /// <summary> Отписка от события OnInteractionEnded (PlayerController.cs). </summary>
        private void OnDestroy()
        {
            if (_playerController) _playerController.OnInteractionEnded -= EndInteraction;
        }

        /// <summary> Устанавливает текущее состояние взаимодействия. </summary>
        /// <param name="state"> Новое состояние (true — идёт взаимодействие). </param>
        public void SetInteractionState(bool state) => IsInteracting = state;
    }
}