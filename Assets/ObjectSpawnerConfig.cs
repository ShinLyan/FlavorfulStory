using UnityEngine;

[CreateAssetMenu(menuName = "FlavorfulStory/Environment/ObjectSpawnerConfig", fileName = "SpawnerConfig")]
public class ObjectSpawnerConfig : ScriptableObject
{
    [Tooltip("Префаб объекта для спавна.")]
    public GameObject Object;

    [Tooltip("Область в пределах которого будут заспавнены объекты.")]
    public int Width, Length;

    [Tooltip("Количество объектов")]
    [Range(1, 50)]
    public int Quantity;
    
    [Tooltip("Минимальное расстояние между объектами.")]
    [Range(1, 50)]
    public int MinSpacing;
    
    [Tooltip("Равномерное распределение. Объекты заспавнятся по автоматической сетке.")]
    public bool EvenSpread;
}