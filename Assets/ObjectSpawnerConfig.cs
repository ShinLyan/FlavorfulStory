using UnityEngine;

/// <summary> Конфигурация спавнера объектов. </summary>
[CreateAssetMenu(menuName = "FlavorfulStory/Environment/ObjectSpawnerConfig", fileName = "SpawnerConfig")]
public class ObjectSpawnerConfig : ScriptableObject
{
    /// <summary> Префаб объекта для спавна. </summary>
    [Tooltip("Префаб объекта для спавна.")]
    public GameObject Object;

    /// <summary> Область в пределах которой будут заспавнены объекты. </summary>
    [Tooltip("Область в пределах которого будут заспавнены объекты.")]
    public int Width, Length;

    /// <summary> Количество объектов для спавна. </summary>
    [Tooltip("Количество объектов")]
    [Range(1, 50)]
    public int Quantity;
    
    // <summary> Минимальное расстояние между объектами. </summary>
    [Tooltip("Минимальное расстояние между объектами.")]
    [Range(1, 50)]
    public int MinSpacing;
    
    /// <summary> Равномерное распределение объектов по сетке. </summary>
    [Tooltip("Равномерное распределение. Объекты заспавнятся по автоматической сетке.")]
    public bool EvenSpread;
}