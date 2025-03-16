namespace FlavorfulStory.AI.WarpGraphSystem
{
    /// <summary> Ребро графа варпов, представляющее связь между двумя узлами (варпами). </summary>
    public class WarpEdge
    {
        /// <summary> Инициализирует новое ребро графа варпов. </summary>
        /// <param name="target">Целевой узел, к которому ведет это ребро.</param>
        public WarpEdge(WarpNode target)
        {
            TargetNode = target;
        }

        /// <summary> Целевой узел, к которому ведет это ребро. </summary>
        public WarpNode TargetNode { get; }
    }
}