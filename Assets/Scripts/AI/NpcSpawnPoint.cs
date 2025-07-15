using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Точка спавна NPC с возможностью генерации случайных позиций в радиусе. </summary>
    public class NpcSpawnPoint : MonoBehaviour
    {
        /// <summary> Радиус спавна для генерации случайных позиций вокруг точки. </summary>
        [SerializeField] private float _spawnRadius = 2f;

        /// <summary> Возвращает случайную позицию в радиусе спавна с сохранением высоты исходной точки. </summary>
        /// <returns> Случайная позиция в пределах радиуса спавна. </returns>
        public Vector3 GetRandomPosition()
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * _spawnRadius;
            randomPos.y = transform.position.y;
            return randomPos;
        }

        /// <summary> Отрисовывает визуальные элементы точки спавна в редакторе. </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);
            Gizmos.DrawIcon(transform.position + Vector3.up * 2, "SpawnPointIcon", true);
        }
    }
}