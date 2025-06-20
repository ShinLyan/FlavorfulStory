using FlavorfulStory.Lightning;
using UnityEngine;
using Zenject;

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

        /// <summary> Глобальная система освещения. </summary>
        private GlobalLightSystem _globalLightSystem;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="globalLightSystem"> Глобальная система освещения.</param>
        [Inject]
        private void Construct(GlobalLightSystem globalLightSystem) => _globalLightSystem = globalLightSystem;

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

            if (!_globalLightSystem || _objectsToDisable.Length == 0) return;
            _globalLightSystem.gameObject.SetActive(_isRoom);
        }

        /// <summary> Находится ли заданная позиция внутри границ локации? </summary>
        /// <param name="position"> Позиция в мировом пространстве. </param>
        /// <returns> <c>true</c>, если позиция внутри локации; иначе — <c>false</c>. </returns>
        public bool IsPositionInLocation(Vector3 position) =>
            TryGetComponent(out Collider component) && component.bounds.Contains(position);
    }
}