using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Класс для объекта взаимодействия. </summary>
    /// <remarks> Реализует интерфейс IInteractable. </remarks>
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        /// <summary> Предмет, который будет добавлен в инвентарь при взаимодействии. </summary>
        [SerializeField] private DropItem _dropItem;

        /// <summary> Название объекта, отображаемое в тултипе. </summary>
        private const string Name = "Wending stump";
        
        /// <summary> Описание объекта, отображаемое в тултипе. </summary>
        private const string Description = "Press E, if hungry";

        /// <summary> Флаг, определяющий возможность взаимодействия с объектом. </summary>
        private bool _canInteract;

        /// <summary> Инициализация объекта. </summary>
        private void Awake()
        {
            _canInteract = true;
        }

        /// <summary> Возвращает расстояние до другого трансформа. </summary>
        /// <param name="otherTransform"> Трансформ объекта, до которого вычисляется расстояние. </param>
        /// <returns> Расстояние между объектами. </returns>
        public float GetDistanceTo(Transform otherTransform)
        {
            return Vector3.Distance(transform.position, otherTransform.position);
        }

        /// <summary> Проверяет, доступно ли взаимодействие с объектом. </summary>
        /// <returns> Возвращает true, если взаимодействие разрешено. </returns>
        public bool IsInteractionAllowed()
        {
            return _canInteract;
        }

        /// <summary> Выполняет взаимодействие с объектом. </summary>
        public void Interact()
        {
            if (_canInteract)
            {
                Inventory.PlayerInventory.TryAddToFirstEmptySlot(_dropItem.ItemPrefab, _dropItem.Quantity);
            }
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