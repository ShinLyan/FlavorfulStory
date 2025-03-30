using System;

namespace FlavorfulStory.ResourceContainer
{
    /// <summary> Интерфейс разрушаемого объекта. </summary>
    public interface IDestroyable
    {
        /// <summary> Уничтожен ли объект? </summary>
        public bool IsDestroyed { get; }

        /// <summary> Событие уничтожения объекта. </summary>
        public event Action<IDestroyable> OnObjectDestroyed;

        /// <summary> Уничтожить объект. </summary>
        /// <param name="destroyDelay"> Задержка перед уничтожением. </param>
        public void Destroy(float destroyDelay = 0f);
    }
}