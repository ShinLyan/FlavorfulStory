using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Точка деспавна NPC. </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class NpcDespawnPoint : MonoBehaviour
    {
        /// <summary> Обрабатывает вход в триггер. </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NonInteractableNpc.NonInteractableNpc>(out var npc))
                npc.OnReachedDespawnPoint?.Invoke();
        }
    }
}