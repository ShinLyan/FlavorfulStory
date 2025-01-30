using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    public class TreeDestructibleObject : DestructibleObject, ISpawnable
    {
        public event Action<ISpawnable> OnObjectDestroyed;
        protected override void OnDestroyed()
        {
            OnObjectDestroyed?.Invoke(this);
            gameObject.AddComponent(typeof(Rigidbody));
        }
    }
}