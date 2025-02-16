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
        
        /// <summary> Разрушен ли объект? </summary>
        private bool IsDestroyed { get; set; }

        /// <summary> Задержка перед окончательным уничтожением объекта. </summary>
        private const float DestroyDelay = 4f;

        /// <summary> Событие, вызываемое при разрушении объекта. </summary>
        public event Action<DestructibleObject> OnObjectDestroyed;

        /// <summary> Взаимодействовать. </summary>
        /// <param name="player"> Контроллер игрока. </param>
        public override void Interact(PlayerController player)
        {
            if (player.CurrentItem is not Tool tool || 
                tool.ToolType != _requiredTool ||
                IsDestroyed) return;

            _currentHits++;

            if (_currentHits == _hitsToDestroy) DestroyObject();
        }

        /// <summary> Уничтожить объект и сгенерировать выпадающие предметы. </summary>
        private void DestroyObject()
        {
            IsDestroyed = true;
            Debug.Log($"Object destroyed: {gameObject.name}");

            OnObjectDestroyed?.Invoke(this);
            OnDestroyed();
            StartCoroutine(DestroyCoroutine());
        }

        /// <summary> Действие, вызываемое при разрушении (например, прокачка навыков). </summary>
        protected abstract void OnDestroyed();

        /// <summary> Удаление объекта после задержки. </summary>
        private System.Collections.IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(DestroyDelay);
            DropItems();
            Destroy(gameObject);
        }
        
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