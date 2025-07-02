using UnityEngine;

namespace FlavorfulStory.AI
{
    public interface ICharacterCollisionHandler
    {
        void OnTriggerEntered(Collider other);
        void OnTriggerExited(Collider other);
    }
}