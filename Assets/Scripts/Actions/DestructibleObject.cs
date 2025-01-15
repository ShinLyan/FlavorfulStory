using FlavorfulStory.Control;
using FlavorfulStory.InventorySystem.DropSystem;
using System;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Разрушаемый объект. </summary>
    [RequireComponent(typeof(ItemDropper))]
    public abstract class DestructibleObject : InteractableObject
    {
        /// <summary> Количество ударов для разрушения объекта. </summary>
        [Tooltip("Количество ударов для разрушения объекта."), Range(1, 5)]
        [SerializeField] private int _hitsToDestroy;

        /// <summary> Тип инструмента, необходимого для разрушения. </summary>
        [Tooltip("Тип инструмента, необходимого для разрушения.")]
        [SerializeField] private ToolType _requiredTool;

        /// <summary> Список предметов, которые выпадут при разрушении. </summary>
        [Tooltip("Список предметов, которые выпадут при разрушении.")]
        [SerializeField] private DropItem[] _dropItems;

        /// <summary> Текущее количество ударов по объекту. </summary>
        private int _currentHits;

        private const float DestroyDelay = 4f;

        /// <summary> Событие, вызываемое при разрушении объекта. </summary>
        public event Action<DestructibleObject> OnObjectDestroyed;

        /// <summary> Взаимодействовать. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public override void Interact(PlayerController player)
        {
            if (player.CurrentItem is not Tool tool || tool.ToolType != _requiredTool) return;

            _currentHits++;
            Debug.Log($"Object hit! Remaining hits: {_hitsToDestroy - _currentHits}");

            if (_currentHits >= _hitsToDestroy) DestroyObject();
        }

        /// <summary> Уничтожить объект и сгенерировать выпадающие предметы. </summary>
        private void DestroyObject()
        {
            Debug.Log($"Object destroyed: {gameObject.name}");

            OnObjectDestroyed?.Invoke(this);
            OnDestroyed();
            DropItems();
            Destroy(gameObject, DestroyDelay);
        }

        /// <summary> Действие, вызываемое при разрушении (например, прокачка навыков). </summary>
        protected abstract void OnDestroyed();

        /// <summary> Выбросить предметы, настроенные в конкретных подклассах. </summary>
        protected virtual void DropItems()
        {
            var itemDropper = GetComponent<ItemDropper>();
            foreach (var dropItem in _dropItems)
            {
                itemDropper.DropItem(dropItem.ItemPrefab, dropItem.Quantity);
            }
        }
    }
}