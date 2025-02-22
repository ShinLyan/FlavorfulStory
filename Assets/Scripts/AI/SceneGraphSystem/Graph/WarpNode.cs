using System.Collections.Generic;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class WarpNode
    {
        public Warp SourceWarp { get; }
        public List<WarpEdge> Edges { get; } = new List<WarpEdge>();

        public WarpNode(Warp warp)
        {
            SourceWarp = warp;
        }
    }
}