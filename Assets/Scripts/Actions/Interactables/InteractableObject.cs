using System;
using FlavorfulStory.InputSystem;
using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    //TODO: Разбить на 2 класса с общим абстрактным родителем: ReusableInteractableObject и SingleUseInteractableObject
    /// <summary> Класс для объекта взаимодействия. </summary>
    /// <remarks> Реализует интерфейс IInteractable. </remarks>
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        /// <summary> Предмет, который будет добавлен в инвентарь при взаимодействии. </summary>
        [SerializeField] private DropItem _dropItem;

        [SerializeField] private GameObject _fruit;

        [SerializeField] private float _interactionCooldown;

        [SerializeField] private bool _singleUse;
        
        /// <summary> Название объекта, отображаемое в тултипе. </summary>
        // TODO: Возможно надо будет выпилить и сделать нормально, более универсально
        private const string Name = "Wending stump";

        /// <summary> Описание объекта, отображаемое в тултипе. </summary>
        // TODO: Возможно надо будет выпилить и сделать нормально, более универсально
        private const string Description = "Press E, if hungry";

        /// <summary> Флаг возможности взаимодействия с объектом. </summary>
        private bool _isInteractionAllowed = true;

        /// <summary> Проверяет, доступно ли взаимодействие с объектом. </summary>
        public bool IsInteractionAllowed
        {
            get => _isInteractionAllowed;
            set
            {
                _isInteractionAllowed = value;
                if (_fruit != null)
                    _fruit.SetActive(_isInteractionAllowed);
            }
        }

        private System.Collections.IEnumerator EnableInteractionAfterCooldown()
        {
            yield return new WaitForSeconds(_interactionCooldown);
            IsInteractionAllowed = true;
        }

        [field: SerializeField] public bool IsBlockingMovement { get; set; }

        /// <summary> Возвращает расстояние до другого трансформа. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние между объектами. </returns>
        public float GetDistanceTo(Transform otherTransform) =>
            Vector3.Distance(transform.position, otherTransform.position);

        /// <summary> Выполняет взаимодействие с объектом. </summary>
        public void Interact()
        {
            if (!IsInteractionAllowed) return;
            
            IsInteractionAllowed = false;
            Inventory.PlayerInventory.TryAddToFirstAvailableSlot(_dropItem.ItemPrefab, _dropItem.Quantity);
            
            StartCoroutine(EnableInteractionAfterCooldown());
            
            if (_singleUse) Destroy(gameObject, _interactionCooldown);
        }

        /// <summary> Возвращает название для тултипа. </summary>
        /// <returns> Название объекта. </returns>
        public string GetTooltipTitle() => Name;

        /// <summary> Возвращает описание для тултипа. </summary>
        /// <returns> Описание объекта. </returns>
        public string GetTooltipDescription() => Description;

        /// <summary> Возвращает позицию объекта в мировых координатах. </summary>
        /// <returns> Мировая позиция объекта. </returns>
        public Vector3 GetWorldPosition() => transform.position;
    }
}