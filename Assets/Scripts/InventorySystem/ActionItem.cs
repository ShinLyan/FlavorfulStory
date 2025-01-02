using FlavorfulStory.Actions.ActionItems;
using FlavorfulStory.Control;
using UnityEngine;

namespace FlavorfulStory.InventorySystem
{
    /// <summary> ������� ���������, ������� ����� ������������.</summary>
    /// <remarks> ���� ����� ������� ������������ � �������� ��������. ��������� ������ ������������� ����� `Use`.</remarks>
    [CreateAssetMenu(menuName = ("FlavorfulStory/Inventory/Action Item"))]
    public class ActionItem : InventoryItem, IUsable
    {
        /// <summary> ����������� �� ������� ��� �������������?</summary>
        [field: Tooltip("����������� �� ������� ��� �������������?")]
        [field: SerializeField] public bool IsConsumable { get; private set; }

        /// <summary> ����������� �� ������� ��� �������������?</summary>
        [field: Tooltip("��� �������� ��� ������������� �������� (��� ��� ���).")]
        [field: SerializeField] public UseActionType UseActionType { get; private set; }

        /// <summary> ������������� ��������.</summary>
        /// <remarks> �������������� ��� ����������� ����������������.</remarks>
        /// <param name="player"> ���������� ������.</param>
        public virtual void Use(PlayerController player)
        {
            Debug.Log($"Using action: {this}");
        }
    }
}