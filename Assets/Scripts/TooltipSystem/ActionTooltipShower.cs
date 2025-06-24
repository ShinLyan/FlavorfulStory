using System.Collections.Generic;
using FlavorfulStory.Utils;
using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Отвечает за отображение тултипа для объектов взаимодействия. </summary>
    public class ActionTooltipShower : MonoBehaviour, IActionTooltipShower
    {
        /// <summary> Префаб отдельной строчки действия в тултипе. </summary>
        [SerializeField] private TooltipActionView _tooltipActionPrefab;
        /// <summary> Родительский трансформ, куда помещаются UI-элементы действий. </summary>
        [SerializeField] private Transform _parentTransform;
        
        /// <summary> Пул для переиспользуемых элементов тултипа. </summary>
        private ObjectPool<TooltipActionView> _pool;
        
        /// <summary> Список активных (отображаемых) действий. </summary>
        private readonly List<TooltipActionData> _activeTooltips = new();
        /// <summary> Список UI-элементов, созданных и отображённых на экране. </summary>
        private readonly List<TooltipActionView> _spawnedViews = new();
        
        /// <summary> Инициализация пула и скрытие тултипа при запуске. </summary>
        private void Start()
        {
            _pool = new ObjectPool<TooltipActionView>(
                createFunc: () => Instantiate(_tooltipActionPrefab, _parentTransform),
                initFunc: t => t.Show(),
                deInitFunc: t => t.Hide()
            );

            gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public void Add(TooltipActionData action)
        {
            if (_activeTooltips.Contains(action)) return;

            _activeTooltips.Add(action);
            RefreshTooltipViews();
        }

        /// <inheritdoc/>
        public void Remove(TooltipActionData action)
        {
            if (_activeTooltips.Remove(action))
                RefreshTooltipViews();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _activeTooltips.Clear();
            RefreshTooltipViews();
        }

        /// <summary> Обновляет отображение: скрывает старые и отображает актуальные действия. </summary>
        private void RefreshTooltipViews()
        {
            foreach (var view in _spawnedViews)
                _pool.Release(view);

            _spawnedViews.Clear();
            
            if (_activeTooltips.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            foreach (var action in _activeTooltips)
            {
                var view = _pool.Get();
                view.Setup(action.KeyText, CreateActionDescription(action));
                _spawnedViews.Add(view);
            }
        }

        /// <summary> Формирует финальный текст действия, объединяя глагол и цель. </summary>
        /// <param name="tooltip"> Структура данных действия. </param>
        /// <returns> Строка формата: "Harvest Bread". </returns>
        private static string CreateActionDescription(TooltipActionData tooltip) =>
            $"{tooltip.ActionDescription.Action} {tooltip.ActionDescription.Target}";
    }
}