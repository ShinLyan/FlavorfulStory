using System;

public interface IDestroyable
{
    bool IsDestroyed { get; }
    
    event Action<IDestroyable> OnObjectDestroyed;

    void Destroy();
}