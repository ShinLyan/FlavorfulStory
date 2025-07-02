using UnityEngine;

namespace FlavorfulStory.AI
{
    [RequireComponent(typeof(Collider))]
    public class NpcCollisionHandler : MonoBehaviour
    {
        private ICharacterCollisionHandler _handler;

        public void Initialize(ICharacterCollisionHandler handler) => _handler = handler;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _handler?.OnTriggerEntered(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _handler?.OnTriggerExited(other);
        }
    }
}