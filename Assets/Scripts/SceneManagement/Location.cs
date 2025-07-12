using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Игровая локация, которая может быть активирована/деактивирована 
    /// и проверяет, находится ли точка внутри её границ. </summary>
    public class Location : MonoBehaviour
    {
        /// <summary> Название игровой локации. </summary>
        [field: Tooltip("Название игровой локации."), SerializeField]
        public LocationName LocationName { get; private set; }

        /// <summary> Объекты, которые будут отключены при выходе игрока из локации. </summary>
        [Tooltip("Объекты, которые будут отключены при выходе игрока из локации."), SerializeField]
        private GameObject[] _objectsToDisable;

        /// <summary> Является ли локация помещением. </summary>
        [field: Tooltip("Является ли локация помещением?"), SerializeField]
        public bool IsRoom { get; private set; }

        [SerializeField] private NavMeshSurface _navMeshSurface;

        /// <summary> Устанавливает активное состояние всех объектов из списка. </summary>
        /// <param name="isActive"> <c>true</c> — включить объекты; <c>false</c> — отключить. </param>
        public void SetActive(bool isActive)
        {
            if (_objectsToDisable == null || _objectsToDisable.Length == 0) return;

            foreach (var objectToDisable in _objectsToDisable) objectToDisable.SetActive(isActive);
        }

        /// <summary> Находится ли заданная позиция внутри границ локации? </summary>
        /// <param name="position"> Позиция в мировом пространстве. </param>
        /// <returns> <c>true</c>, если позиция внутри локации; иначе — <c>false</c>. </returns>
        public bool IsPositionInLocation(Vector3 position) =>
            TryGetComponent(out Collider component) && component.bounds.Contains(position);


        public virtual Vector3 GetRandomPointOnNavMesh(int maxAttempts = 20)
        {
            Bounds searchBounds;

            if (_navMeshSurface)
            {
                searchBounds = new Bounds(_navMeshSurface.transform.position, _navMeshSurface.size);
            }
            else if (TryGetComponent(out Collider col))
            {
                searchBounds = col.bounds;
            }
            else
            {
                Debug.LogWarning("Не удалось найти точку на NavMesh.");
                return Vector3.zero;
            }


            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(searchBounds.min.x, searchBounds.max.x),
                    searchBounds.center.y,
                    Random.Range(searchBounds.min.z, searchBounds.max.z)
                );

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas)) return hit.position;
            }

            Debug.LogWarning("Не удалось найти точку на NavMesh.");
            return Vector3.zero;
        }
    }
}