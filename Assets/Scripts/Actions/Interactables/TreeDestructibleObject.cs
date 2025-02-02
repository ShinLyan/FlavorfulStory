using System;
using FlavorfulStory.ObjectSpawner;
using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Разрушаемый объект типа "дерево". </summary>
    public class TreeDestructibleObject : DestructibleObject, ISpawnable
    {
        // TODO: Пофиксить дублирование.
        public event Action<ISpawnable> OnObjectDestroyed;

        /// <summary> Выполнение действий при уничтожении объекта. </summary>
        protected override void OnDestroyed()
        {
            // Добавление компонента Rigidbody для активации физики после разрушения.
            OnObjectDestroyed?.Invoke(this);
            gameObject.AddComponent(typeof(Rigidbody));
        }
    }
}