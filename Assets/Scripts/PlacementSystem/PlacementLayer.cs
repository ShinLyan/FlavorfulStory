namespace FlavorfulStory.PlacementSystem
{
    /// <summary> Типы слоёв для размещения объектов в игровом мире. </summary>
    public enum PlacementLayer
    {
        /// <summary> Слой для размещения мебели (столы, стулья, кровати и т.д.). </summary>
        Furniture,

        /// <summary> Слой для объектов, размещаемых на полу (например, ковры, плитка). </summary>
        Floor,

        /// <summary> Слой для декоративных объектов (например, растения, статуэтки). </summary>
        Decoration,

        /// <summary> Слой для объектов, размещаемых на стене (например, картины, полки). </summary>
        Wall
    }
}