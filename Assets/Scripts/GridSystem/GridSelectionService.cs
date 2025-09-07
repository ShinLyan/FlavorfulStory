using System.Collections.Generic;
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

        /// <summary> Словарь цветов, используемых для различных состояний индикатора грида. </summary>
        private static readonly Dictionary<GridIndicatorState, Color> _stateColors = new()
        {
            { GridIndicatorState.ValidTarget, new Color(1f, 1f, 1f, 0.5f) },
            { GridIndicatorState.InvalidTarget, new Color(245f / 255f, 152f / 255f, 159f / 255f, 0.8f) }
        };

        /// <summary> Конструктор, принимающий объект-индикатор. </summary>
        /// <param name="gridIndicator"> Префаб или объект-индикатор для отображения на гриде. </param>
        public GridSelectionService(GameObject gridIndicator) => _gridIndicator = gridIndicator;

        /// <summary> Инициализация сервиса – получение рендерера и скрытие индикатора. </summary>
        public void Initialize()
        {
            _gridIndicatorRenderer = _gridIndicator.GetComponentInChildren<Renderer>();
            HideGridIndicator();
        }

        /// <summary> Показать индикатор на выбранной позиции с указанным размером и цветом. </summary>
        /// <param name="worldPosition"> Позиция в мире, где должен появиться индикатор. </param>
        /// <param name="size"> Размер (в клетках грида), который должен отобразиться. </param>
        /// <param name="state"> Состояние, определяющее цвет индикатора (валидная или невалидная цель). </param>
        public void ShowGridIndicator(Vector3 worldPosition, Vector2Int size, GridIndicatorState state)
        {
            var position = worldPosition;
            position.y = 0.015f;
            _gridIndicator.transform.position = position;
            _gridIndicator.transform.localScale = new Vector3(size.x, 1f, size.y);
            _gridIndicatorRenderer.material.mainTextureScale = size;
            _gridIndicatorRenderer.material.color = GetColorForState(state);
            _gridIndicator.SetActive(true);
        }

        /// <summary> Скрывает индикатор. </summary>
        public void HideGridIndicator() => _gridIndicator.SetActive(false);

        /// <summary> Получить цвет для указанного состояния индикатора. </summary>
        /// <param name="state"> Состояние, определяющее цвет индикатора (валидная или невалидная цель). </param>
        /// <returns> Цвет, соответствующий переданному состоянию, либо белый по умолчанию. </returns>
        public static Color GetColorForState(GridIndicatorState state) =>
            _stateColors.TryGetValue(state, out var color) ? color : Color.white;
    }
}