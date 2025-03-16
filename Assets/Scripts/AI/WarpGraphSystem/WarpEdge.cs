namespace FlavorfulStory.AI.WarpGraphSystem
{
    public struct WarpEdge
    {
        public WarpEdge(WarpNode source, WarpNode target)
        {
            SourceNode = source;
            TargetNode = target;
        }

        public WarpNode SourceNode { get; }
        public WarpNode TargetNode { get; }
    }
}