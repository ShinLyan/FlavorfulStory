using System;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Интерфейс разрушаемого объекта. </summary>
    public interface IDestroyable
    {
        /// <summary> Уничтожен ли объект. </summary>
        public bool IsDestroyed { get; }

        /// <summary> Событие уничтожения объекта. </summary>
        public event Action<IDestroyable> OnObjectDestroyed;

        /// <summary> Уничтожить объект. </summary>
        public void Destroy();
    }
}