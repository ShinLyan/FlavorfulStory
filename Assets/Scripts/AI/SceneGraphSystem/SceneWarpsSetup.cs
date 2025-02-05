using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    [CreateAssetMenu(fileName = "SceneWarpsSetup", menuName = "FlavorfulStory/AI Behavior/SceneWarpsSetup", order = 0)]
    public class SceneWarpsSetup : ScriptableObject
    {
        public Warp[] warps;
    }
}