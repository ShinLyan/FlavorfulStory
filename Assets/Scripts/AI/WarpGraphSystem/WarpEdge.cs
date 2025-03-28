namespace FlavorfulStory.AI.WarpGraphSystem
{
    /// <summary> Связь (ребро) между двумя узлами графа телепортов. </summary>
    public struct WarpEdge
    {
        /// <summary> Исходный узел. </summary>
        public WarpNode SourceNode { get; }

        /// <summary> Целевой узел. </summary>
        public WarpNode TargetNode { get; }

        /// <summary> Создает новое ребро между двумя узлами. </summary>
        /// <param name="source"> Исходный узел. </param>
        /// <param name="target"> Целевой узел. </param>
        public WarpEdge(WarpNode source, WarpNode target)
        {
            SourceNode = source;
            TargetNode = target;
        }
    }
}