using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Абстрактный базовый класс, который обрабатывает создание
    /// префаба тултипа в правильном положении на экране относительно курсора. </summary>
    /// <remarks> Переопределите абстрактные методы для своего спавнера тултипа. </remarks>
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Префаб тултипа, который нужно заспавнить. </summary>
        protected GameObject TooltipPrefab { private get; set; }

        /// <summary> Заспавненный тултип. </summary>
        private GameObject _tooltip;

        /// <summary> Задержка перед появлением тултипа. </summary>
        private const float TooltipDelay = 0.5f;

        /// <summary> Токен отмены для текущего запроса показа тултипа. </summary>
        private CancellationTokenSource _tooltipCts;

        #region Abstract Methods

        /// <summary> Можно ли создать тултип? </summary>
        /// <returns> <c>true</c>, если тултип можно создать, иначе <c>false</c>. </returns>
        protected abstract bool CanCreateTooltip();

        /// <summary> Обновляет содержимое тултипа на основе текущего предмета. </summary>
        /// <param name="tooltip"> Заспавненный префаб тултипа для обновления. </param>
        protected abstract void UpdateTooltip(GameObject tooltip);

        #endregion

        /// <summary> Вызывается при уничтожении объекта. </summary>
        private void OnDestroy() => ClearTooltip();

        /// <summary> Вызывается при отключении объекта. </summary>
        private void OnDisable() => ClearTooltip();

        /// <summary> Вызывается при уводе курсора с объекта UI. </summary>
        /// <param name="eventData"> Данные события взаимодействия с UI. </param>
        public void OnPointerExit(PointerEventData eventData) => ClearTooltip();

        /// <summary> Очистить тултип. </summary>
        private void ClearTooltip()
        {
            if (_tooltip)
            {
                Destroy(_tooltip);
                _tooltip = null;
            }

            _tooltipCts?.Cancel();
            _tooltipCts?.Dispose();
            _tooltipCts = null;
        }

        /// <summary> Вызывается при наведении курсора на объект UI. </summary>
        /// <remarks> Создает или обновляет тултип при необходимости. </remarks>
        /// <param name="eventData"> Данные события взаимодействия с UI. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            _tooltipCts?.Cancel();
            _tooltipCts?.Dispose();

            _tooltipCts = new CancellationTokenSource();
            ShowTooltipWithDelayAsync(_tooltipCts.Token).Forget();
        }

        /// <summary> Асинхронно отображает тултип с задержкой. </summary>
        /// <param name="token"> Токен отмены, позволяющий прервать отображение. </param>
        private async UniTaskVoid ShowTooltipWithDelayAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(TooltipDelay), cancellationToken: token);

            if (token.IsCancellationRequested) return;

            if (_tooltip && !CanCreateTooltip())
            {
                ClearTooltip();
                return;
            }

            if (!_tooltip && CanCreateTooltip())
            {
                var parentCanvas = GetComponentInParent<Canvas>();
                _tooltip = Instantiate(TooltipPrefab, parentCanvas.transform);
            }

            if (_tooltip)
            {
                UpdateTooltip(_tooltip);
                PositionTooltip();
            }
        }

        /// <summary> Располагает тултип на экране в зависимости от положения UI-элемента,
        /// чтобы тултип не выходил за границы экрана. </summary>
        private void PositionTooltip()
        {
            // Обновляем положение всех Canvas-элементов для учета изменений в UI.
            Canvas.ForceUpdateCanvases();

            // Получаем мировые координаты углов тултипа.
            var tooltipCorners = new Vector3[4];
            _tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);

            // Получаем мировые координаты углов текущего объекта (слота).
            var slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            // Определяем местоположение относительно середины экрана.
            bool below = transform.position.y > Screen.height / 2;
            bool right = transform.position.x < Screen.width / 2;

            // Выбираем индекс угла текущего объекта (слота) и тултипа.
            int slotCornerIndex = GetCornerIndex(below, right);
            int tooltipCornerIndex = GetCornerIndex(!below, !right);

            // Устанавливаем новое положение тултипа, чтобы он был выровнен с нужным углом.
            _tooltip.transform.position = slotCorners[slotCornerIndex] - tooltipCorners[tooltipCornerIndex] +
                                          _tooltip.transform.position;
        }

        /// <summary> Возвращает индекс нужного угла UI-элемента для позиционирования тултипа. </summary>
        /// <param name="below"> Находится ли UI-элемент ниже центра экрана? </param>
        /// <param name="right"> Находится ли UI-элемент правее центра экрана? </param>
        /// <returns> Индекс угла UI-элемента. </returns>
        /// <remarks> Индекс угла: 0 - левый нижний, 1 - левый верхний, 2 - правый верхний, 3 - правый нижний. </remarks>
        private static int GetCornerIndex(bool below, bool right) => (below, right) switch
        {
            (true, false) => 0, // Левый нижний угол
            (false, false) => 1, // Левый верхний угол
            (false, true) => 2, // Правый верхний угол
            (true, true) => 3 // Правый нижний угол
        };
    }
}