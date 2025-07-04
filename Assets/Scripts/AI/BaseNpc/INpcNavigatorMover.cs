namespace FlavorfulStory.AI.BaseNpc
{
    public interface INpcNavigatorMover<in T>
    {
        void MoveTo(T target);
    }
}