using UnityEngine;
using Zenject;

namespace FlavorfulStory.GridSystem
{
    /// <summary> Сервис для отображения индикатора выбранной клетки грида. </summary>
    public class GridSelectionService : IInitializable
    {
        /// <summary> Индикатор для визуализации выбранной клетки. </summary>
        private readonly GameObject _gridIndicator;

        /// <summary> Рендерер индикатора, используется для изменения цвета и текстуры. </summary>
        private Renderer _gridIndicatorRenderer;

        /// <summary> Цвет индикатора при валидной позиции размещения. </summary>
        public static readonly Color ValidColor = new(1f, 1f, 1f, 0.5f);

        /// <summary> Цвет индикатора при невалидной позиции размещения. </summary>
        public static readonly Color InvalidColor = new(1f, 0f, 0f, 0.5f);

        /// <summary> Конструктор, принимающий объект-индикатор. </summary>
        /// <param name="gridIndicator"> Префаб или объект-индикатор для отображения на гриде. </param>
        public GridSelectionService(GameObject gridIndicator) => _gridIndicator = gridIndicator;

        /// <summary> Инициализация сервиса – получение рендерера и скрытие индикатора. </summary>
        public void Initialize()
        {
            _gridIndicatorRenderer = _gridIndicator.GetComponentInChildren<Renderer>();
            HideGridIndicator();
        }

        /// <summary> Показать индикатор на выбранной позиции с указанным размером и валидностью. </summary>
        /// <param name="worldPosition"> Позиция в мире, где должен появиться индикатор. </param>
        /// <param name="size"> Размер (в клетках грида), который должен отобразиться. </param>
        /// <param name="isValid"> Является ли текущая позиция допустимой для размещения. </param>
        public void ShowGridIndicator(Vector3 worldPosition, Vector2Int size, bool isValid)
        {
            _gridIndicator.transform.position = worldPosition;
            _gridIndicator.transform.localScale = new Vector3(size.x, 1f, size.y);
            _gridIndicatorRenderer.material.mainTextureScale = size;
            _gridIndicatorRenderer.material.color = isValid ? ValidColor : InvalidColor;

            _gridIndicator.SetActive(true);
        }

        /// <summary> Скрывает индикатор. </summary>
        public void HideGridIndicator() => _gridIndicator.SetActive(false);
    }
}