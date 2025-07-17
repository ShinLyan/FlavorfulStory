using System.Collections.Generic;
using FlavorfulStory.Infrastructure;
using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Отвечает за отображение тултипа для объектов взаимодействия. </summary>
    public class ActionTooltipShower : MonoBehaviour, IActionTooltipShower
    {
        /// <summary> Префаб отдельной строчки действия в тултипе. </summary>
        [SerializeField] private ActionTooltipView _actionTooltipViewPrefab;

        /// <summary> Родительский трансформ, куда помещаются UI-элементы действий. </summary>
        [SerializeField] private Transform _parentTransform;

        /// <summary> Пул для переиспользуемых элементов тултипа. </summary>
        private ObjectPool<ActionTooltipView> _pool;

        /// <summary> Список активных (отображаемых) действий. </summary>
        private readonly List<ActionTooltipData> _activeTooltips = new();

        /// <summary> Список UI-элементов, созданных и отображённых на экране. </summary>
        private readonly List<ActionTooltipView> _spawnedViews = new();

        /// <summary> Инициализация пула и скрытие тултипа при запуске. </summary>
        private void Start()
        {
            _pool = new ObjectPool<ActionTooltipView>(
                () => Instantiate(_actionTooltipViewPrefab, _parentTransform),
                tooltipActionView => tooltipActionView.Show(),
                tooltipActionView => tooltipActionView.Hide()
            );

            gameObject.SetActive(false);
        }

        /// <summary> Добавляет действие во всплывающую подсказку. </summary>
        /// <param name="action"> Данные действия (клавиша + описание). </param>
        public void Add(ActionTooltipData action)
        {
            if (_activeTooltips.Contains(action)) return;

            _activeTooltips.Add(action);
            RefreshTooltipViews();
        }

        /// <summary> Удаляет действие из тултипа. </summary>
        /// <param name="action"> Данные действия, которые нужно удалить. </param>
        public void Remove(ActionTooltipData action)
        {
            if (_activeTooltips.Remove(action)) RefreshTooltipViews();
        }

        /// <summary> Обновляет отображение: скрывает старые и отображает актуальные действия. </summary>
        private void RefreshTooltipViews()
        {
            foreach (var view in _spawnedViews) _pool.Release(view);

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
        /// <param name="actionTooltip"> Структура данных действия. </param>
        /// <returns> Строка формата: "Harvest Bread". </returns>
        private static string CreateActionDescription(ActionTooltipData actionTooltip) =>
            $"{actionTooltip.Action} {actionTooltip.Target}";
    }
}