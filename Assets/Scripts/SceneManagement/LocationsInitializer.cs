using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    public class LocationsInitializer : MonoBehaviour
    {
        private void Awake()
        {
            LocationChanger.InitializeLocations();
        }
    }
}