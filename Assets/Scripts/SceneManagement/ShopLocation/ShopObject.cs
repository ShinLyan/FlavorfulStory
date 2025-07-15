using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.SceneManagement.ShopLocation
{
    /// <summary> Базовый класс для объектов магазина с функциональностью занятости и доступными позициями. </summary>
    public class ShopObject : MonoBehaviour
    {
        /// <summary> Массив доступных позиций для взаимодействия с объектом. </summary>
        [SerializeField] protected Transform[] _accessiblePositions;

        //TODO: для каждого объекта сделать анимацию взаимодейтсвия с ним
        // [SerializeField] private InteractableObjectAnimationType _interactableObjectAnimationType;

        /// <summary> Цвет гизмо для занятого объекта (красный с прозрачностью). </summary>
        private readonly Color _occupiedColor = new(1f, 0.3f, 0.3f, 1f); // Красный с прозрачностью

        /// <summary> Цвет гизмо для свободного объекта (зелёный с прозрачностью). </summary>
        private readonly Color _freeColor = new(0.3f, 1f, 0.3f, 1f); // Зелёный с прозрачностью

        /// <summary> Цвет гизмо для точек доступа (синий). </summary>
        private readonly Color _accessPointColor = new(0.3f, 0.3f, 1f, 1f); // Синий для точек доступа

        /// <summary> Размер основного гизмо объекта. </summary>
        private readonly float _mainGizmoSize = 3f;

        /// <summary> Размер гизмо для точек доступа. </summary>
        protected readonly float _accessPointSize = 0.3f;

        /// <summary> Указывает, занят ли объект в данный момент. </summary>
        public bool IsOccupied { get; private set; }

        /// <summary> Устанавливает состояние занятости объекта. </summary>
        /// <param name="isOccupied"> True, если объект занят, иначе false. </param>
        public void SetOccupied(bool isOccupied) => IsOccupied = isOccupied;

        /// <summary> Возвращает случайную доступную позицию для взаимодействия с объектом. </summary>
        /// <returns> Transform случайной доступной позиции. </returns>
        public virtual Transform GetAccessiblePoint()
        {
            return _accessiblePositions[Random.Range(0, _accessiblePositions.Length)];
        }

        /// <summary> Отрисовывает гизмо объекта при выборе в редакторе. </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = IsOccupied ? _occupiedColor : _freeColor;
            Gizmos.DrawWireSphere(transform.position, _mainGizmoSize);

            if (_accessiblePositions != null)
            {
                Gizmos.color = _accessPointColor;
                foreach (var point in _accessiblePositions)
                    if (point != null)
                        Gizmos.DrawLine(transform.position, point.position);
            }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = IsOccupied ? _occupiedColor : _freeColor;
            Handles.Label(transform.position + Vector3.up,
                IsOccupied ? "Occupied" : "Free",
                style);
        }
    }
}