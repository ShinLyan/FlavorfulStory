namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Тип режима размещения объекта. </summary>
    public enum PlacementModeType
    {
        /// <summary> Размещение нового объекта. </summary>
        Place, 

        /// <summary> Удаление существующего объекта. </summary>
        Remove 

        // future: Rotate, Inspect, Move, etc.
    }
}