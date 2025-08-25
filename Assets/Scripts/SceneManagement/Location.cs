using UnityEngine;

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

        /// TODO: ВЫНЕСТИ В ShopLocation после мерджа с NPC
        [field: SerializeField] public Transform EntryPoint { get; private set; }
        
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
    }
}