using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    [CreateAssetMenu(fileName = "SceneWarpsSetup", menuName = "FlavorfulStory/AI Behavior/LocationWarpsSetup", order = 0)]
    public class LocationWarpsSetup : ScriptableObject
    {
        public Warp[] warps;
    }
}