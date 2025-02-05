using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
        
    [CreateAssetMenu(fileName = "AllWarpsSetup", menuName = "FlavorfulStory/AI Behavior/AllWarpsSetup", order = 0)]
    public class AllWarpsSetup : ScriptableObject
    {
        public SceneWarpsSetup[] SceneWarpsSetup;
    }
}