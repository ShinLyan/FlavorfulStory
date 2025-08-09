using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.Infrastructure;
using FlavorfulStory.Utils.FlavorfulStory.Utils;
using UnityEngine;

namespace FlavorfulStory.TooltipSystem.ActionTooltips
{
    /// <summary> Отвечает за отображение всплывающих подсказок действий на экране. </summary>
    public class ActionTooltipViewSpawner : MonoBehaviour, IActionTooltipViewSpawner
    {
        /// <summary> Префаб отображения тултипа действия. </summary>
        [SerializeField] private ActionTooltipView _actionTooltipViewPrefab;

        /// <summary> Пул переиспользуемых тултипов. </summary>
        private ObjectPool<ActionTooltipView> _pool;

        /// <summary> Активные тултипы по источникам (например, Interactable, Toolbar и т.д.). </summary>
        private readonly Dictionary<ActionTooltipSource, ActionTooltipData> _activeTooltips = new();

        /// <summary> Все созданные и отображаемые в данный момент view-элементы. </summary>
        private readonly List<ActionTooltipView> _spawnedViews = new();

        /// <summary> Инициализация пула и скрытие тултипов при запуске. </summary>
        private void Start()
        {
            _pool = new ObjectPool<ActionTooltipView>(
                () => Instantiate(_actionTooltipViewPrefab, transform),
                view => view.Show(),
                view => view.Hide()
            );

            gameObject.SetActive(false);
        }

        /// <summary> Добавляет действие во всплывающую подсказку. </summary>
        /// <param name="action"> Данные действия (клавиша + описание). </param>
        public void Add(ActionTooltipData action)
        {
            _activeTooltips[action.Source] = action;
            UpdateView();
        }

        /// <summary> Удаляет действие из тултипа. </summary>
        /// <param name="action"> Данные действия, которые нужно удалить. </param>
        public void Remove(ActionTooltipData action)
        {
            if (_activeTooltips.Remove(action.Source)) UpdateView();
        }

        /// <summary> Обновляет отображение: скрывает старые и отображает актуальные действия. </summary>
        private void UpdateView()
        {
            foreach (var view in _spawnedViews) _pool.Release(view);
            _spawnedViews.Clear();

            if (_activeTooltips.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            foreach (var tooltip in GetTooltipsSortedByPriority())
            {
                var view = _pool.Get();
                view.Setup(tooltip.KeyText, GetActionTooltipDescription(tooltip));
                view.transform.SetAsLastSibling();
                _spawnedViews.Add(view);
            }
        }

        /// <summary> Получить отсортированный список тултипов по приоритету источника. </summary>
        /// <returns> Отсортированный список тултипов по приоритету источника. </returns>
        private IEnumerable<ActionTooltipData> GetTooltipsSortedByPriority() =>
            _activeTooltips.Values.OrderBy(actionTooltipData => GetPriority(actionTooltipData.Source));

        /// <summary> Получить приоритет отображения источников: чем ниже число, тем выше в списке. </summary>
        /// <param name="source"> Источник действия. </param>
        /// <returns> Приоритет отображения. </returns>
        private static int GetPriority(ActionTooltipSource source) => source switch
        {
            ActionTooltipSource.Toolbar => 1,
            ActionTooltipSource.Interactable => 2,
            _ => int.MaxValue
        };

        /// <summary> Получить описание действия (клавиша + цель). </summary>
        /// <param name="actionTooltipData"> Данные действия. </param>
        /// <returns> Строка описания. </returns>
        private static string GetActionTooltipDescription(ActionTooltipData actionTooltipData) =>
            $"{actionTooltipData.Action.ToDisplayName()} {actionTooltipData.Target}";
    }
}