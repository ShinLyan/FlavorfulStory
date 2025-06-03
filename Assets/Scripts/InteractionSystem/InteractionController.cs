using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Player;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.TooltipSystem;
using UnityEngine;

namespace FlavorfulStory.InteractionSystem
{
    /// <summary> Контроллер взаимодействия с интерактивными объектами через триггер-зону. </summary>
    public class InteractionController : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> UI-объект для отображения тултипа взаимодействия. </summary>
        [SerializeField] private InteractableObjectTooltip _tooltipView;

        /// <summary> PlayerController родительского объекта. </summary>
        private PlayerController _playerController;

        /// <summary> Список объектов, доступных для взаимодействия. </summary>
        private readonly List<IInteractable> _availableInteractables = new();

        /// <summary> Ближайший объект, с которым можно взаимодействовать. </summary>
        private IInteractable _closestInteractable;

        /// <summary> Активный объект, с которым сейчас взаимодействуют. </summary>
        private IInteractable _activeInteractable;

        /// <summary> Делегат действия, вызываемый при начале взаимодействия. </summary>
        private Action _startInteractionAction;

        /// <summary> Делегат действия, вызываемый при завершении взаимодействия. </summary>
        private Action _endInteractionAction;

        #endregion

        /// <summary> Инициализация компонента. </summary>
        /// <remarks> Подписка на событие OnInteractionEnded (PlayerController.cs). </remarks>
        private void Awake() => _playerController = GetComponentInParent<PlayerController>();

        /// <summary> Проверяет нажатие кнопки взаимодействия и вызывает метод Interact()
        /// для ближайшего объекта. </summary>
        private void Update()
        {
            if (_availableInteractables.Count > 0) UpdateClosestInteractable();

            if (_closestInteractable == null || !InputWrapper.GetButtonDown(InputButton.Interact)) return;

            BeginInteraction();
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
                _tooltipView.Show(_closestInteractable);
            else
                _tooltipView.Hide();
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
            _availableInteractables.Remove(destroyable as IInteractable);
            UpdateClosestInteractable();
        }

        /// <summary> Начало взаимодействия с объектом. </summary>
        private void BeginInteraction()
        {
            _startInteractionAction?.Invoke();
            _activeInteractable = _closestInteractable;
            _activeInteractable?.BeginInteraction(_playerController);
        }

        /// <summary> Завершение взаимодействия с объектом. </summary>
        /// <remarks> Метод подписан на событие в анимации игрока (Gather_interaction). </remarks>
        public void EndInteraction()
        {
            _endInteractionAction?.Invoke();
            _activeInteractable?.EndInteraction(_playerController);
            _activeInteractable = null;
        }

        /// <summary> Устанавливает действия, вызываемые при начале и завершении взаимодействия. </summary>
        /// <param name="onStart"> Действие при начале взаимодействия. </param>
        /// <param name="onEnd"> Действие при завершении взаимодействия. </param>
        public void SetInteractionActions(Action onStart, Action onEnd)
        {
            _startInteractionAction = onStart;
            _endInteractionAction = onEnd;
        }
    }
}