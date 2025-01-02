using FlavorfulStory.Control;

/// <summary> Интерфейс использования.</summary>
public interface IUsable
{
    /// <summary> Использовать.</summary>
    /// <param name="player"> Контроллер игрока.</param>
    public void Use(PlayerController player);
}