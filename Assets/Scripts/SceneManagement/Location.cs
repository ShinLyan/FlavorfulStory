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

        /// <summary> Является ли локация помещением. </summary>
        [Tooltip("Является ли локация помещением?"), SerializeField]
        private bool _isRoom;

        /// <summary> Глобальные источники освещения. </summary>
        private GameObject[] _globalLights;

        /// <summary> Найти все источники освещения. </summary>
        private void Awake() => _globalLights = GameObject.FindGameObjectsWithTag("GlobalLight");

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

            if (_globalLights == null || _objectsToDisable.Length == 0) return;

            foreach (var globalLight in _globalLights) globalLight.SetActive(_isRoom);
        }

        /// <summary> Находится ли заданная позиция внутри границ локации? </summary>
        /// <param name="position"> Позиция в мировом пространстве. </param>
        /// <returns> <c>true</c>, если позиция внутри локации; иначе — <c>false</c>. </returns>
        public bool IsPositionInLocation(Vector3 position) =>
            TryGetComponent(out Collider component) && component.bounds.Contains(position);
    }
}