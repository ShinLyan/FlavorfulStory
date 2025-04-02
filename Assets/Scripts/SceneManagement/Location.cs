using UnityEngine;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Игровая локация. </summary>
    public class Location : MonoBehaviour
    {
        /// <summary> Название игровой локации. </summary>
        [field: Tooltip("Название игровой локации."), SerializeField]
        public LocationName LocationName { get; private set; }

        /// <summary> Объекты, которые выключаются, когда игрок уходит с локации. </summary>
        [Tooltip("Объекты, которые выключаются, когда игрок уходит с локации."), SerializeField]
        private GameObject[] _objectsToDisable;

        private Bounds _locationBounds;

        private void Awake()
        {
            if (TryGetComponent(out Collider component))
                _locationBounds = component.bounds;
        }

        /// <summary> Включает все объекты локации, отмеченные для активации. </summary>
        public void Enable()
        {
            foreach (var objectToDisable in _objectsToDisable) objectToDisable.SetActive(true);
        }

        /// <summary> Выключает все объекты локации, отмеченные для деактивации. </summary>
        public void Disable()
        {
            foreach (var objectToDisable in _objectsToDisable) objectToDisable.SetActive(false);
        }

        public bool IsInside(Vector3 position) => _locationBounds.Contains(position);
    }
}