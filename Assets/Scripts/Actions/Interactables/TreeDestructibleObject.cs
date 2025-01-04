using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    public class TreeDestructibleObject : DestructibleObject
    {
        protected override void OnDestroyed()
        {
            gameObject.AddComponent(typeof(Rigidbody));
        }
    }
}