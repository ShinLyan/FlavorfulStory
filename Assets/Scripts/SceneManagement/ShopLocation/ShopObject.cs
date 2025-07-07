using UnityEngine;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    public class ShopObject : MonoBehaviour
    {
        [SerializeField] private Transform[] _accessiblePositions;

        public bool IsOccupied { get; private set; }

        public void SetOccupied(bool isOccupied) => IsOccupied = isOccupied;

        public Vector3 GetAccessiblePoint()
        {
            return Vector3.zero;
            // TODO: реализовать метод
        }
    }
}