using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Игровая локация. </summary>
    public class Location : MonoBehaviour
    {
        [field: Tooltip("Название игровой локации"), SerializeField]
        public LocationName LocationName { get; private set; }
    }
}