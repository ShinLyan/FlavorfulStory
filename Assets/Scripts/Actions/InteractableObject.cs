using FlavorfulStory.Control;
using UnityEngine;

namespace FlavorfulStory.Actions
{
    /// <summary> ������������� ������ - ����������� �����. </summary>
    public abstract class InteractableObject : MonoBehaviour
    {
        /// <summary> ������������ ��������� ��� ��������������. </summary>
        [Tooltip("���������, ������� ������������ ��� ��������� �� ������.")]
        [field: SerializeField] public string InteractionMessage { get; protected set; }

        /// <summary> �����, ���������� ��� ��������������. </summary>
        /// <param name="player"> �����, �������������� ��������������. </param>
        public abstract void Interact(PlayerController player);
    }
}