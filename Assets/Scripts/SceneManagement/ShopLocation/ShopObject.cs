using UnityEngine;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    public class ShopObject : MonoBehaviour
    {
        [SerializeField] protected Transform[] _accessiblePositions;

        public bool IsOccupied { get; private set; }

        public void SetOccupied(bool isOccupied) => IsOccupied = isOccupied;

        public virtual Vector3 GetAccessiblePoint()
        {
            return _accessiblePositions[Random.Range(0, _accessiblePositions.Length)].position;
        }
    }
}