using FlavorfulStory.Control;
using FlavorfulStory.InventorySystem.DropSystem;
using System;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> Разрушаемый объект, взаимодействующий с игроком. </summary>
    [RequireComponent(typeof(ItemDropper))]
    public abstract class DestructibleObject : InteractableObject
    {
        /// <summary> Количество ударов, необходимое для разрушения объекта. </summary>
        [Tooltip("Количество ударов, необходимое для разрушения объекта."), Range(1, 5)]
        [SerializeField] private int _hitsToDestroy;

        /// <summary> Тип инструмента, требуемого для разрушения объекта. </summary>
        [Tooltip("Тип инструмента, требуемого для разрушения объекта.")]
        [SerializeField] private ToolType _requiredTool;

        /// <summary> Список предметов, выпадающих при разрушении объекта. </summary>
        [Tooltip("Список предметов, выпадающих при разрушении объекта.")]
        [SerializeField] private DropItem[] _dropItems;

        /// <summary> Текущее количество ударов, нанесенных объекту. </summary>
        private int _currentHits;

        /// <summary> Задержка перед удалением объекта после разрушения. </summary>
        private const float DestroyDelay = 4f;

        /// <summary> Событие, уведомляющее о разрушении объекта. </summary>
        public event Action<DestructibleObject> OnObjectDestroyed;

        /// <summary> Взаимодействие с объектом. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public override void Interact(PlayerController player)
        {
            if (player.CurrentItem is not Tool tool || tool.ToolType != _requiredTool) return;

            _currentHits++;

            if (_currentHits >= _hitsToDestroy) DestroyObject();
        }

        /// <summary> Разрушение объекта с вызовом связанных действий. </summary>
        private void DestroyObject()
        {
            OnObjectDestroyed?.Invoke(this);
            OnDestroyed();
            DropItems();
            Destroy(gameObject, DestroyDelay);
        }

        /// <summary> Действия, выполняемые при разрушении объекта. </summary>
        protected abstract void OnDestroyed();

        /// <summary> Выпадение предметов, связанных с объектом. </summary>
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