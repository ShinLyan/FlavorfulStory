using System;

/// <summary> Интерфейс, определяющий объект, который может быть уничтожен. </summary>
public interface IDestroyable
{
    /// <summary> Событие, возникающее при уничтожении объекта. </summary>
    public event Action<IDestroyable> OnObjectDestroyed;
}