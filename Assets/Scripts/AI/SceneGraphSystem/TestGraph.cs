using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class TestGraph : MonoBehaviour
    {
        [SerializeField] private AllWarpsSetup _allWarpsSetup;
        private WarpGraph _warpGraph;

        private void Start()
        {
            WarpGraph _warpGraph = WarpGraphBuilder.BuildGraph(_allWarpsSetup);
            _warpGraph.PrintGraph();
        }
        
    }
}