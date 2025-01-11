using FlavorfulStory.Control;
using FlavorfulStory.InventorySystem.DropSystem;
using System;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> ����������� ������. </summary>
    [RequireComponent(typeof(ItemDropper))]
    public abstract class DestructibleObject : InteractableObject
    {
        /// <summary> ���������� ������ ��� ���������� �������. </summary>
        [Tooltip("���������� ������ ��� ���������� �������."), Range(1, 5)]
        [SerializeField] private int _hitsToDestroy;

        /// <summary> ��� �����������, ������������ ��� ����������. </summary>
        [Tooltip("��� �����������, ������������ ��� ����������.")]
        [SerializeField] private ToolType _requiredTool;

        /// <summary> ������ ���������, ������� ������� ��� ����������. </summary>
        [Tooltip("������ ���������, ������� ������� ��� ����������.")]
        [SerializeField] private DropItem[] _dropItems;

        /// <summary> ������� ���������� ������ �� �������. </summary>
        private int _currentHits;

        private const float DestroyDelay = 4f;

        /// <summary> �������, ���������� ��� ���������� �������. </summary>
        public event Action<DestructibleObject> OnObjectDestroyed;

        /// <summary> �����������������. </summary>
        /// <param name="player"> ���������� ������. </param>
        public override void Interact(PlayerController player)
        {
            if (player.CurrentItem is not Tool tool || tool.ToolType != _requiredTool) return;

            _currentHits++;
            Debug.Log($"Object hit! Remaining hits: {_hitsToDestroy - _currentHits}");

            if (_currentHits >= _hitsToDestroy) DestroyObject();
        }

        /// <summary> ���������� ������ � ������������� ���������� ��������. </summary>
        private void DestroyObject()
        {
            Debug.Log($"Object destroyed: {gameObject.name}");

            OnObjectDestroyed?.Invoke(this);
            OnDestroyed();
            DropItems();
            Destroy(gameObject, DestroyDelay);
        }

        /// <summary> ��������, ���������� ��� ���������� (��������, �������� �������). </summary>
        protected abstract void OnDestroyed();

        /// <summary> ��������� ��������, ����������� � ���������� ����������. </summary>
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