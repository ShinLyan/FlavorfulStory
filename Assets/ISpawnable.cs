using System;
using System.Runtime.Serialization;

public interface ISpawnable
{
    public event Action<ISpawnable> OnObjectDestroyed;
}