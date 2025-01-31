using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    public class TreeDestructibleObject : DestructibleObject, IDestroyable
    {
        public event Action<IDestroyable> OnObjectDestroyed;
        protected override void OnDestroyed()
        {
            OnObjectDestroyed?.Invoke(this);
            gameObject.AddComponent(typeof(Rigidbody));
        }
    }
}