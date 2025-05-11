using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace FlavorfulStory
{
    /// <summary> UI-элемент уведомления о подобранном предмете. </summary>
    public class PickupNotification : MonoBehaviour
    {
        /// <summary> Иконка предмета. </summary>
        [SerializeField] private Image icon;
        /// <summary> Текст с названием и количеством. </summary>
        [SerializeField] private TMP_Text message;
        /// <summary> Управляет прозрачностью и появлением. </summary>
        [SerializeField] private CanvasGroup canvasGroup;

        /// <summary> RectTransform для управления позицией уведомления. </summary>
        private RectTransform _rectTransform;
        /// <summary> Tween для анимации появления/исчезновения. </summary>
        private Tween _fadeTween;
        /// <summary> Tween для плавной анимации количества. </summary>
        private Tween _countTween;
        /// <summary> Исходная X-позиция уведомления (на экране). </summary>
        private float _startX;

        /// <summary> Уникальный идентификатор предмета. </summary>
        public string ItemID { get; private set; }
        /// <summary> Имя предмета для отображения. </summary>
        private string _itemName;
        /// <summary> Текущее фактическое количество предмета. </summary>
        private int _currentAmount;
        /// <summary> Отображаемое количество (анимируется). </summary>
        private int _displayedAmount;

        /// <summary> Текущая высота уведомления (для позиционирования). </summary>
        public float Height => _rectTransform.rect.height;
        /// <summary> RectTransform уведомления (для доступа извне). </summary>
        public RectTransform RectTransform => _rectTransform;

        /// <summary> Кэширует RectTransform и позицию по X при создании. </summary>
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startX = _rectTransform.anchoredPosition.x;
        }

        /// <summary> Инициализирует уведомление визуалом, ID, количеством и позицией. </summary>
        public void Initialize(Sprite itemSprite, int amount, string itemId, string itemName)
        {
            icon.sprite = itemSprite;
            ItemID = itemId;
            _itemName = itemName;
            _currentAmount = amount;
            _displayedAmount = amount;
            canvasGroup.alpha = 0f;

            UpdateMessage();

            float offscreenX = _startX + Screen.width;
            _rectTransform.anchoredPosition = new Vector2(offscreenX, _rectTransform.anchoredPosition.y);
        }

        /// <summary> Добавляет количество и анимирует увеличение числа. </summary>
        public void AddAmount(int amount)
        {
            int oldAmount = _currentAmount;
            _currentAmount += amount;

            _countTween?.Kill(true);
            _countTween = DOVirtual.Int(oldAmount, _currentAmount, 0.4f, value =>
            {
                _displayedAmount = value;
                UpdateMessage();
            });
        }

        /// <summary> Запускает анимацию появления уведомления. </summary>
        public void Show(float fadeDuration)
        {
            _fadeTween?.Kill(true);

            DOTween.Sequence()
                .Join(canvasGroup.DOFade(1f, fadeDuration))
                .Join(_rectTransform.DOAnchorPosX(_startX, fadeDuration).SetEase(Ease.OutCubic));
        }

        /// <summary> Возвращает стартовую X-позицию для выравнивания. </summary>
        public float GetStartX() => _startX;

        /// <summary> Плавно перемещает уведомление в указанную позицию. </summary>
        public void SetPosition(Vector2 anchoredPosition, float duration)
        {
            _rectTransform
                .DOAnchorPos(anchoredPosition, duration)
                .SetEase(Ease.OutCubic);
        }

        /// <summary> Плавно исчезает и уничтожает объект. </summary>
        public void FadeAndDestroy(float duration)
        {
            _fadeTween?.Kill(true);
            _countTween?.Kill(true);

            float offscreenX = _startX + Screen.width;

            DOTween.Sequence()
                .Join(canvasGroup.DOFade(0f, duration))
                .Join(_rectTransform.DOAnchorPosX(offscreenX, duration).SetEase(Ease.InCubic))
                .OnComplete(() => Destroy(gameObject));
        }
        
        /// <summary> Обновляет отображаемый текст. </summary>
        private void UpdateMessage() => message.text = $"x{_displayedAmount} {_itemName}";
    }
}