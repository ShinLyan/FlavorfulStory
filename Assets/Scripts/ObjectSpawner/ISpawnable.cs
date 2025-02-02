using System;

namespace FlavorfulStory.ObjectSpawner
{
    /// <summary> Интерфейс, определяющий объект, который может быть уничтожен. </summary>
    public interface ISpawnable
    {
        /// <summary> Событие, возникающее при уничтожении объекта. </summary>
        public event Action<ISpawnable> OnObjectDestroyed;
    }
}