using FlavorfulStory.Saving;
using UnityEngine;

namespace FlavorfulStory.LocationManager
{
    /// <summary> Управляет сменой внешнего вида игровых объектов. </summary>
    public class AppearanceSwitcher : MonoBehaviour, ISaveable
    {
        /// <summary> Указывает, является ли смена внешнего вида циклической. </summary>
        [SerializeField] private bool _isCycled;

        /// <summary> Массив игровых объектов, управляемых сменщиком внешнего вида. </summary>
        private InteractableObject2[] _gameObjects;

        /// <summary> Индекс текущего состояния. </summary>
        private int _currentStateIndex;

        /// <summary> Инициализация массива объектов и начального состояния. </summary>
        private void Awake()
        {
            _gameObjects = GetComponentsInChildren<InteractableObject2>(true);
            _currentStateIndex = 0;
        }

        /// <summary> Переключение на следующий внешний вид. </summary>
        public void ChangeAppearance()
        {
            int objectsCount = _gameObjects.Length;
            if (_isCycled)
            {
                _currentStateIndex = (_currentStateIndex + 1) % objectsCount;
            }
            else
            {
                _currentStateIndex = Mathf.Clamp(_currentStateIndex + 1, 0, objectsCount - 1);
            }

            SetAppearance(_currentStateIndex);
        }

        /// <summary> Устанавливает внешний вид на основе указанного индекса состояния. </summary>
        /// <param name="stateIndex"> Индекс состояния для установки. </param>
        private void SetAppearance(int stateIndex)
        {
            foreach (var state in _gameObjects)
            {
                state.gameObject.SetActive(false);
            }

            _gameObjects[stateIndex].gameObject.SetActive(true);
        }

        #region Saving

        /// <summary> Сохраняет текущий индекс состояния. </summary>
        /// <returns> Индекс текущего состояния. </returns>
        public object CaptureState() => _currentStateIndex;

        /// <summary> Восстанавливает состояние внешнего вида на основе сохраненного индекса. </summary>
        /// <param name="state"> Сохраненное состояние, представляющее индекс. </param>
        public void RestoreState(object state)
        {
            _currentStateIndex = (int)state;
            SetAppearance(_currentStateIndex);
        }

        #endregion
    }
}