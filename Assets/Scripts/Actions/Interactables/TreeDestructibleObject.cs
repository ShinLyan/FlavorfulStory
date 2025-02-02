using UnityEngine;

namespace FlavorfulStory.Actions.Interactables
{
    /// <summary> Разрушаемый объект типа "дерево". </summary>
    public class TreeDestructibleObject : DestructibleObject
    {
        /// <summary> Выполнение действий при уничтожении объекта. </summary>
        protected override void OnDestroyed()
        {
            // Добавление компонента Rigidbody для активации физики после разрушения.
            gameObject.AddComponent(typeof(Rigidbody));
        }
    }
}