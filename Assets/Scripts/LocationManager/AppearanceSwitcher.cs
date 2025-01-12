using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.LocationManager
{
    /// <summary> Сменщик внешнего вида. </summary>
    public class AppearanceSwitcher : MonoBehaviour, ISaveable
    {
        /// <summary> Смена внешнего вида зациклена. </summary>
        [SerializeField] private bool _isCycled;
        
        /// <summary> Игровые объекты. </summary>
        private InteractableObject2[] _gameObjects;
        
        /// <summary> Номер текущего состояния. </summary>
        private int _currentStateIndex;
        
        /// <summary> Получение объектов. </summary>
        private void Awake()
        {
            _gameObjects = GetComponentsInChildren<InteractableObject2>(true);
            _currentStateIndex = 0;
        }

        /// <summary> Смена внешнего вида. </summary>
        public void ChangeAppearance()
        {
            int objectsCount = _gameObjects.Length;
            if (_isCycled)
                _currentStateIndex = (_currentStateIndex + 1) % objectsCount;
            else
                _currentStateIndex = Mathf.Clamp(_currentStateIndex + 1, 0, objectsCount - 1);
            SetAppearance(_currentStateIndex);
        }

        /// <summary> Установка внешнего вида. </summary>
        private void SetAppearance(int stateIndex)
        {
            foreach (var state in _gameObjects)
            {
                state.gameObject.SetActive(false);
            }
            
            _gameObjects[stateIndex].gameObject.SetActive(true);
        }

        #region Saving
        /// <summary> Сохранение состояния. </summary>
        public object CaptureState() => _currentStateIndex;

        /// <summary> Восстановление состояния. </summary>
        public void RestoreState(object state)
        {
            _currentStateIndex = (int)state;
            SetAppearance(_currentStateIndex);
        }
        #endregion
    }
}