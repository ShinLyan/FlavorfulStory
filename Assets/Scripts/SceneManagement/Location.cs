using FlavorfulStory.AI.BaseNpc;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Игровая локация, которая может быть активирована/деактивирована 
    /// и проверяет, находится ли точка внутри её границ. </summary>
    [RequireComponent(typeof(BoxCollider))]
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

        /// <summary> Поверхность NavMesh для данной локации. </summary> 
        [SerializeField] private NavMeshSurface _navMeshSurface;

        /// <summary> Коллайдер, определяющий границы локации. </summary>
        private BoxCollider _collider;

        /// <summary> Выполняет инициализацию компонентов. </summary>
        private void Awake() => _collider = GetComponent<BoxCollider>();

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
        public bool IsPositionInLocation(Vector3 position) => _collider.bounds.Contains(position);

        /// <summary> Находит случайную точку на NavMesh в пределах локации. </summary>
        /// <param name="maxAttempts"> Максимальное количество попыток поиска точки. По умолчанию 20. </param>
        /// <returns> Случайная позиция на NavMesh. Возвращает Vector3.zero, если точка не найдена. </returns>
        public virtual NpcDestinationPoint GetRandomPointOnNavMesh(int maxAttempts = 20)
        {
            Bounds searchBounds;

            if (_navMeshSurface)
            {
                searchBounds = new Bounds(_navMeshSurface.transform.position, _navMeshSurface.size);
            }
            else
            {
                Debug.LogWarning("Не удалось найти точку на NavMesh.");
                return new NpcDestinationPoint();
            }

            for (int i = 0; i < maxAttempts; i++)
            {
                var randomPoint = new Vector3(
                    Random.Range(searchBounds.min.x, searchBounds.max.x),
                    searchBounds.center.y,
                    Random.Range(searchBounds.min.z, searchBounds.max.z)
                );

                if (NavMesh.SamplePosition(randomPoint, out var hit, 2f, NavMesh.AllAreas))
                    return new NpcDestinationPoint(hit.position, Quaternion.identity);
            }

            Debug.LogWarning("Не удалось найти точку на NavMesh.");
            return new NpcDestinationPoint();
        }
    }
}