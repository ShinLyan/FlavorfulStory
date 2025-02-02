using System;
using FlavorfulStory.ObjectSpawner;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    public class TreeDestructibleObject : DestructibleObject, ISpawnable
    {
        // TODO: Пофиксить дублирование.
        public event Action<ISpawnable> OnObjectDestroyed;

        protected override void OnDestroyed()
        {
            OnObjectDestroyed?.Invoke(this);
            gameObject.AddComponent(typeof(Rigidbody));
        }
    }
}