namespace FlavorfulStory.AI.WarpGraphSystem
{
    /// <summary> Связь (ребро) между двумя узлами графа телепортов. </summary>
    public struct WarpEdge
    {
        /// <summary> Целевой узел. </summary>
        public WarpNode TargetNode { get; }

        /// <summary> Создает новое ребро между двумя узлами. </summary>
        /// <param name="source"> Исходный узел. </param>
        /// <param name="target"> Целевой узел. </param>
        public WarpEdge(WarpNode target)
        {
            TargetNode = target;
        }
    }
}