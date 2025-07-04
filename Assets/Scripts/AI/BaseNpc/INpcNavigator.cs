namespace FlavorfulStory.AI.BaseNpc
{
    public interface INpcNavigator
    {
        void Update();
        void Stop(bool warpToSpawn = false);
    }
}