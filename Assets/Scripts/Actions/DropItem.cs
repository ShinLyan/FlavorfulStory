using FlavorfulStory.InventorySystem;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> ���������� ������� � ��� ����������.</summary>
    [System.Serializable]
    public class DropItem
    {
        /// <summary> ������ ��������, ������� ����� ������.</summary>
        [Tooltip("������ ��������, ������� ����� ������.")]
        public InventoryItem ItemPrefab;

        /// <summary> ���������� ���������� ���������.</summary>
        [Tooltip("���������� ���������� ���������.")]
        [Range(1, 100)]
        public int Quantity;
    }
}