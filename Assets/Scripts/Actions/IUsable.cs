using FlavorfulStory.Control;

/// <summary> ��������� �������������.</summary>
public interface IUsable
{
    /// <summary> ������������.</summary>
    /// <param name="player"> ���������� ������.</param>
    public void Use(PlayerController player);
}