using System.Collections.Generic;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    /// <summary> Узел графа варпов, представляющий варп и его связи. </summary>
    public class WarpNode
    {
        /// <summary> Варп, связанный с этим узлом. </summary>
        public Warp SourceWarp { get; }

        /// <summary> Список ребер, связывающих этот узел с другими узлами. </summary>
        public List<WarpEdge> Edges { get; } = new();

        /// <summary> Инициализирует новый узел графа варпов. </summary>
        /// <param name="warp"> Варп, связанный с этим узлом. </param>
        public WarpNode(Warp warp) => SourceWarp = warp;
    }
}