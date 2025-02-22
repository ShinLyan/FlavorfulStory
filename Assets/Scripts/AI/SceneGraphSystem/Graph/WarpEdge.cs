namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class WarpEdge
    {
        public WarpNode TargetNode { get; }
        public int Duration { get; }

        public WarpEdge(WarpNode target, int duration)
        {
            TargetNode = target;
            Duration = duration;
        }
    }
}