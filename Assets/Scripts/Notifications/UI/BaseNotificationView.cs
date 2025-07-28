using DG.Tweening;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Notifications.UI
{
    /// <summary> Базовый UI-элемент для отображения уведомлений. </summary>
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class BaseNotificationView : MonoBehaviour
    {
        /// <summary> Контейнер с визуальной частью уведомления. </summary>        
        [SerializeField] private RectTransform _contentRect;

        /// <summary> Текстовое поле с сообщением. </summary>
        [SerializeField] protected TMP_Text _label;

        /// <summary> CanvasGroup, управляющий прозрачностью уведомления. </summary>
        private CanvasGroup _canvasGroup;

        /// <summary> Высота элемента уведомления. </summary>
        public float Height => RectTransform.rect.height;

        /// <summary> Начальная позиция по оси X. </summary>
        public float StartXPosition { get; private set; }

        /// <summary> RectTransform текущего объекта. </summary>
        private RectTransform RectTransform { get; set; }

        /// <summary> Инициализация компонентов. </summary>
        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            StartXPosition = RectTransform.anchoredPosition.x;
        }

        /// <summary> Инициализирует уведомление на основе переданных данных. </summary>
        /// <param name="data"> Данные уведомления. </param>
        public abstract void Initialize(INotificationData data);

        /// <summary> Устанавливает позицию и якоря уведомления с учетом отступов. </summary>
        /// <param name="position"> Позиция уведомления. </param>
        /// <param name="padding"> Отступы от края. </param>
        public void SetupPosition(NotificationPosition position, Vector2 padding)
        {
            var anchor = position switch
            {
                NotificationPosition.TopLeft => new Vector2(0f, 1f),
                NotificationPosition.TopRight => new Vector2(1f, 1f),
                NotificationPosition.BottomLeft => new Vector2(0f, 0f),
                NotificationPosition.BottomRight => new Vector2(1f, 0f),
                _ => Vector2.zero
            };

            ApplyToRect(RectTransform, anchor);
            ApplyToRect(_contentRect, anchor);

            float xOffset = anchor.x == 0f ? padding.x : -padding.x;
            RectTransform.anchoredPosition = new Vector2(xOffset, 0f);

            StartXPosition = RectTransform.anchoredPosition.x;
        }

        /// <summary> Применяет якорь и pivot к RectTransform. </summary>
        /// <param name="rect"> RectTransform для изменения. </param>
        /// <param name="anchor"> Новое значение anchor/pivot. </param>
        private static void ApplyToRect(RectTransform rect, Vector2 anchor) =>
            rect.anchorMin = rect.anchorMax = rect.pivot = anchor;

        /// <summary> Показывает уведомление с анимацией появления. </summary>
        /// <param name="fadeDuration"> Длительность анимации. </param>
        /// <param name="easing"> Easing-функция для анимации движения. </param>
        public void Show(float fadeDuration, Ease easing)
        {
            _canvasGroup.DOFade(1f, fadeDuration);
            RectTransform.DOAnchorPosX(StartXPosition, fadeDuration).SetEase(easing);
        }

        /// <summary> Прячет уведомление с анимацией и уничтожает объект. </summary>
        /// <param name="fadeDuration"> Длительность анимации исчезновения. </param>
        /// <param name="easing"> Easing-функция для движения. </param>
        public void HideAndDestroy(float fadeDuration, Ease easing) => DOTween.Sequence()
            .Join(_canvasGroup.DOFade(0f, fadeDuration))
            .Join(RectTransform.DOAnchorPosX(-StartXPosition, fadeDuration).SetEase(easing))
            .OnComplete(() => Destroy(gameObject));

        /// <summary> Перемещает уведомление к целевой позиции с анимацией. </summary>
        /// <param name="target"> Целевая позиция. </param>
        /// <param name="duration"> Длительность перемещения. </param>
        /// <param name="easing"> Easing-функция. </param>
        public void MoveTo(Vector2 target, float duration, Ease easing) =>
            RectTransform.DOAnchorPos(target, duration).SetEase(easing);
    }
}