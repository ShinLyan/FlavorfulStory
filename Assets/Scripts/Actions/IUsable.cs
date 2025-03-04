using FlavorfulStory.Control;

/// <summary> Интерфейс для объектов, которые можно использовать. </summary>
public interface IUsable
{
    /// <summary> Использование объекта. </summary>
    /// <param name="player"> Игрок, использующий объект. </param>
    public void Use(PlayerController player);
}