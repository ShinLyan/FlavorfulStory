using UnityEngine;
using Random = UnityEngine.Random;

namespace FlavorfulStory.AI
{
    /// <summary> Точка спавна NPC с возможностью генерации случайных позиций в прямоугольной области. </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class NpcSpawnPoint : MonoBehaviour
    {
        /// <summary> Размер прямоугольной области спавна. </summary>
        [Header("Spawn Area Settings")] [SerializeField]
        private Vector3 _spawnBoxSize = new(4f, 0f, 4f);

        /// <summary> Смещение области спавна относительно центра объекта. </summary>
        [SerializeField] private Vector3 _spawnBoxOffset = new(3f, 0f, 0f);

        /// <summary> Возвращает случайную позицию в прямоугольной области спавна. </summary>
        /// <returns> Случайная мировая позиция в области спавна. </returns>
        public Vector3 GetRandomPosition()
        {
            var localRandomPoint = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0f,
                Random.Range(-0.5f, 0.5f)
            );

            localRandomPoint = Vector3.Scale(localRandomPoint, _spawnBoxSize);
            var worldPoint = transform.TransformPoint(_spawnBoxOffset + localRandomPoint);

            worldPoint.y = transform.position.y;
            return worldPoint;
        }

        /// <summary> Обработчик входа в триггер. Вызывает событие достижения точки деспавна для NPC. </summary>
        /// <param name="other"> Коллайдер объекта, вошедшего в триггер. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NonInteractableNpc.NonInteractableNpc>(out var npc))
                npc.OnReachedDespawnPoint?.Invoke();
        }

#if UNITY_EDITOR
        /// <summary> Отрисовка гизмо в редакторе для визуализации области спавна. </summary>
        private void OnDrawGizmosSelected()
        {
            var originalMatrix = Gizmos.matrix;

            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Gizmos.DrawCube(_spawnBoxOffset, _spawnBoxSize);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_spawnBoxOffset, _spawnBoxSize);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Vector3.zero, _spawnBoxOffset);

            Gizmos.matrix = originalMatrix;
        }
#endif
    }
}