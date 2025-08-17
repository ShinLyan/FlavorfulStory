using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Player;
using FlavorfulStory.ResourceContainer;
using FlavorfulStory.TooltipSystem.ActionTooltips;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.InteractionSystem
{
    /// <summary> Контроллер взаимодействия с интерактивными объектами через триггер-зону. </summary>
    public class InteractionController : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> PlayerController родительского объекта. </summary>
        private PlayerController _playerController;

        /// <summary> Список объектов, доступных для взаимодействия. </summary>
        private readonly List<IInteractable> _availableInteractables = new();

        /// <summary> Ближайший объект, с которым можно взаимодействовать. </summary>
        private IInteractable _closestInteractable;

        /// <summary> Активный объект, с которым сейчас взаимодействуют. </summary>
        private IInteractable _activeInteractable;

        /// <summary> Делегат действия, вызываемый при начале взаимодействия. </summary>
        public Action StartInteractionAction;

        /// <summary> Делегат действия, вызываемый при завершении взаимодействия. </summary>
        public Action EndInteractionAction;

        /// <summary> Шина сигналов для отправки уведомлений другим системам. </summary>
        private SignalBus _signalBus;

        #endregion

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="playerController"> Контроллер игрока. </param>
        /// <param name="signalBus"> Система событий (SignalBus). </param>
        [Inject]
        private void Construct(PlayerController playerController, SignalBus signalBus)
        {
            _playerController = playerController;
            _signalBus = signalBus;
        }

        /// <summary> Определяет ближайший объект и обрабатывает ввод на взаимодействие. </summary>
        private void Update()
        {
            if (_availableInteractables.Count > 0) UpdateClosestInteractable();

            if (_closestInteractable == null || !InputWrapper.GetButtonDown(InputButton.Interact)) return;

            BeginInteraction();
        }

        /// <summary> Обновить ближайший интерактивный объект. </summary>
        private void UpdateClosestInteractable()
        {
            var newClosest = FindClosestInteractable();
            if (newClosest == _closestInteractable) return;

            _closestInteractable = newClosest;
            _signalBus.Fire(new ClosestInteractableChangedSignal { ClosestInteractable = _closestInteractable });
        }

        /// <summary> Определяет ближайший объект для взаимодействия из доступных. </summary>
        /// <returns> Ближайший объект, с которым можно взаимодействовать, или null. </returns>
        private IInteractable FindClosestInteractable() => _availableInteractables
            .Where(interactable => interactable.IsInteractionAllowed)
            .OrderBy(interactable => interactable.GetDistanceTo(transform))
            .FirstOrDefault();

        /// <summary> Добавляет объект в список доступных для взаимодействия при входе в триггер. </summary>
        /// <param name="other"> Коллайдер объекта, вошедшего в триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

            _availableInteractables.Add(interactable);
            interactable.OnInteractionTriggerEnter();

            if (other.TryGetComponent<IDestroyable>(out var destroyable))
                destroyable.OnObjectDestroyed += RemoveInteractable;
        }

        /// <summary> Удаляет объект из списка доступных для взаимодействия при выходе из триггера. </summary>
        /// <param name="other"> Коллайдер объекта, покинувшего триггер. </param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

            _availableInteractables.Remove(interactable);
            interactable.OnInteractionTriggerExit();
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

        /// <summary> Начинает взаимодействие с ближайшим интерактивным объектом. </summary>
        private void BeginInteraction()
        {
            StartInteractionAction?.Invoke();
            _activeInteractable = _closestInteractable;
            _activeInteractable?.BeginInteraction(_playerController);
        }

        /// <summary> Завершает взаимодействия с объектом. </summary>
        /// <remarks> Метод подписан на событие в анимации игрока (Gather_interaction). </remarks>
        public void EndInteraction()
        {
            EndInteractionAction?.Invoke();
            _activeInteractable?.EndInteraction(_playerController);
            _activeInteractable = null;
        }
    }
}