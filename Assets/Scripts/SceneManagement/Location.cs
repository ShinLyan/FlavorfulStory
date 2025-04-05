using UnityEngine;

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

        /// <summary> Границы локации, вычисленные по Collider. </summary>
        private Bounds _locationBounds;

        /// <summary> Инициализация границ локации по компоненту Collider. </summary>
        private void Awake()
        {
            if (TryGetComponent(out Collider component)) _locationBounds = component.bounds;
        }

        /// <summary> Активирует объекты, связанные с этой локацией. </summary>
        public void Enable() => SetObjectsActive(true);

        /// <summary> Деактивирует объекты, связанные с этой локацией. </summary>
        public void Disable() => SetObjectsActive(false);

        /// <summary> Устанавливает активное состояние всех объектов из списка. </summary>
        /// <param name="isActive"> <c>true</c> — включить объекты; <c>false</c> — отключить. </param>
        private void SetObjectsActive(bool isActive)
        {
            if (_objectsToDisable == null || _objectsToDisable.Length == 0) return;

            foreach (var obj in _objectsToDisable) obj.SetActive(isActive);
        }

        /// <summary> Находится ли заданная позиция внутри границ локации? </summary>
        /// <param name="position"> Позиция в мировом пространстве. </param>
        /// <returns> <c>true</c>, если позиция внутри локации; иначе — <c>false</c>. </returns>
        public bool IsPositionInLocation(Vector3 position) => _locationBounds.Contains(position);
    }
}