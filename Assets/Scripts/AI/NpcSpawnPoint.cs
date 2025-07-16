using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.AI
{
    /// <summary> Точка спавна NPC с возможностью генерации случайных позиций в прямоугольной области. </summary>
    [RequireComponent(typeof(Collider))]
    public class NpcSpawnPoint : MonoBehaviour
    {
        [Header("Spawn Area Settings")] [SerializeField]
        private Vector3 _spawnBoxSize = new(4f, 0f, 4f);

        [SerializeField] private Vector3 _spawnBoxOffset = new(3f, 0f, 0f);

        /// <summary> Возвращает случайную позицию в прямоугольной области спавна. </summary>
        public Vector3 GetRandomPosition()
        {
            Vector3 localRandomPoint = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0f,
                Random.Range(-0.5f, 0.5f)
            );

            localRandomPoint = Vector3.Scale(localRandomPoint, _spawnBoxSize);
            Vector3 worldPoint = transform.TransformPoint(_spawnBoxOffset + localRandomPoint);

            worldPoint.y = transform.position.y;
            return worldPoint;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NonInteractableNpc.NonInteractableNpc>(out var npc))
                npc.OnReachedDespawnPoint?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;

            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Gizmos.DrawCube(_spawnBoxOffset, _spawnBoxSize);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_spawnBoxOffset, _spawnBoxSize);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Vector3.zero, _spawnBoxOffset);

            Gizmos.matrix = originalMatrix;
        }
    }
}