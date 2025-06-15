using FlavorfulStory.AI.WarpGraphSystem;
using FlavorfulStory.SceneManagement;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.AI
{
    public class test : MonoBehaviour
    {
        private WarpGraph _warpGraph;

        [Inject]
        private void Construct(WarpGraph warpGraph) { _warpGraph = warpGraph; }

        private void Start()
        {
            Debug.Log("Start test");
            Debug.Log(_warpGraph.FindClosestWarp(transform.position, LocationName.RockyIsland));
        }
    }
}