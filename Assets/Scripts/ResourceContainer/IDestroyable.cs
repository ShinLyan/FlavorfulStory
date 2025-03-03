using System;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Интерфейс разрушаемого объекта. </summary>
    public interface IDestroyable
    {
        /// <summary> Уничтожен ли объект. </summary>
        bool IsDestroyed { get; }

        /// <summary> Событие уничтожения объекта. </summary>
        event Action<IDestroyable> OnObjectDestroyed;

        /// <summary> Уничтожить объект. </summary>
        void Destroy();
    }
}