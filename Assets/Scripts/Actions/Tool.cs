using FlavorfulStory.Control;
using FlavorfulStory.InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.Actions
{
    /// <summary> ����������.</summary>
    /// <remarks> ����� ���� ����������� ������� ��� �������������� � ���������.</remarks>
    [CreateAssetMenu(menuName = ("FlavorfulStory/Inventory/Tool"))]
    public class Tool : ActionItem
    {
        /// <summary> ��� �����������.</summary>
        [field: Tooltip("��� �����������.")]
        [field: SerializeField] public ToolType ToolType { get; private set; }

        /// <summary> ������������ ��������� ��������������.</summary>
        private const float MaxInteractionDistance = 2f;

        /// <summary> ������ ������������� �����������.</summary>
        private const float UseRadius = 1.5f;

        /// <summary> ������������.</summary>
        /// <param name="player"> ���������� ������.</param>
        public override void Use(PlayerController player)
        {
            // �������� �� ���� �� UI
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

            var targetPosition = PlayerController.GetCursorPosition();
            player.RotateTowards(targetPosition);
            player.TriggerAnimation($"Use{ToolType}");
            UseToolInDirection(player.transform.position, targetPosition, player);

            // TODO: ������� ������� ������ ��� ������������� �����������
        }

        /// <summary> ������������ ���������� � �������� �����������.</summary>
        /// <param name="origin"> ����� ������ ��������������.</param>
        /// <param name="targetPosition"> ������� �������, � ����������� ������� ������������ ����������.</param>
        /// <param name="player"> ���������� ������.</param>
        private void UseToolInDirection(Vector3 origin, Vector3 targetPosition, PlayerController player)
        {
            Vector3 direction = (targetPosition - origin).normalized;
            Vector3 interactionCenter = origin + direction * (MaxInteractionDistance / 2);
            Collider[] hitColliders = Physics.OverlapSphere(interactionCenter, UseRadius);
            foreach (var collider in hitColliders)
            {
                if (collider.TryGetComponent<InteractableObject>(out var interactableObject))
                {
                    interactableObject.Interact(player);
                    Debug.Log($"Interacted with object: {interactableObject.name}");
                }
            }
            Debug.DrawLine(origin, interactionCenter, Color.red, 5f);
        }
    }
}