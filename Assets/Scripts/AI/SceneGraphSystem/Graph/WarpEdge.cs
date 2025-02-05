namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class WarpEdge
    {
        public WarpNode TargetNode { get; private set; }
        public int Duration { get; private set; }

        public WarpEdge(WarpNode targetNode, int duration)
        {
            TargetNode = targetNode;
            Duration = duration;
        }
    }
}