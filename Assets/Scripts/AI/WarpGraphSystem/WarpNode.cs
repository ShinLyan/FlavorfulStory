using System.Collections.Generic;

namespace FlavorfulStory.AI.WarpGraphSystem
{
    /// <summary> Узел графа варпов, представляющий варп и его связи. </summary>
    public class WarpNode
    {
        /// <summary> Варп, связанный с этим узлом. </summary>
        public WarpPortal SourceWarp { get; }

        /// <summary> Список ребер, связывающих этот узел с другими узлами. </summary>
        public List<WarpEdge> Edges { get; private set; }

        /// <summary> Инициализирует новый узел графа варпов. </summary>
        /// <param name="warp"> Варп, связанный с этим узлом. </param>
        public WarpNode(WarpPortal warp)
        {
            SourceWarp = warp;
            Edges = new List<WarpEdge>();
        }
    }
}