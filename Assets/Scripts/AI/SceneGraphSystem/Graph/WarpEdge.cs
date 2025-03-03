namespace FlavorfulStory.AI.SceneGraphSystem
{
    /// <summary> Ребро графа варпов, представляющее связь между двумя узлами (варпами). </summary>
    public class WarpEdge
    {
        /// <summary> Целевой узел, к которому ведет это ребро. </summary>
        public WarpNode TargetNode { get; }

        /// <summary> Длительность перехода между узлами (в секундах). </summary>
        public int Duration { get; }

        /// <summary> Инициализирует новое ребро графа варпов. </summary>
        /// <param name="target">Целевой узел, к которому ведет это ребро.</param>
        /// <param name="duration">Длительность перехода между узлами (в секундах).</param>
        public WarpEdge(WarpNode target, int duration)
        {
            TargetNode = target;
            Duration = duration;
        }
    }
}