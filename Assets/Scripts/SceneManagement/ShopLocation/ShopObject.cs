using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    public class ShopObject : MonoBehaviour
    {
        [SerializeField] protected Transform[] _accessiblePositions;

        //TODO: для каждого объекта сделать анимацию взаимодейтсвия с ним
        // [SerializeField] private InteractableObjectAnimationType _interactableObjectAnimationType;

        protected readonly Color _occupiedColor = new(1f, 0.3f, 0.3f, 1f); // Красный с прозрачностью
        protected readonly Color _freeColor = new(0.3f, 1f, 0.3f, 1f); // Зелёный с прозрачностью
        protected readonly Color _accessPointColor = new(0.3f, 0.3f, 1f, 1f); // Синий для точек доступа

        protected readonly float _mainGizmoSize = 3f;
        protected readonly float _accessPointSize = 0.3f;


        public bool IsOccupied { get; private set; }

        public void SetOccupied(bool isOccupied) => IsOccupied = isOccupied;

        public virtual Transform GetAccessiblePoint()
        {
            return _accessiblePositions[Random.Range(0, _accessiblePositions.Length)];
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = IsOccupied ? _occupiedColor : _freeColor;
            Gizmos.DrawWireSphere(transform.position, _mainGizmoSize);

            if (_accessiblePositions != null)
            {
                Gizmos.color = _accessPointColor;
                foreach (var point in _accessiblePositions)
                    if (point != null)
                        Gizmos.DrawLine(transform.position, point.position);
            }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = IsOccupied ? _occupiedColor : _freeColor;
            Handles.Label(transform.position + Vector3.up,
                IsOccupied ? "Occupied" : "Free",
                style);
        }
    }
}