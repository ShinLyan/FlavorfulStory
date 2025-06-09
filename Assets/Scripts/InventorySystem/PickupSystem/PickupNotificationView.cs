using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    /// <summary> UI-элемент уведомления о подобранном предмете. </summary>
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class PickupNotificationView : MonoBehaviour
    {
        #region Fields and Properties

        /// <summary> Иконка предмета. </summary>
        [SerializeField] private Image _itemIconImage;

        /// <summary> Текст с количеством и названием предмета. </summary>
        [SerializeField] private TMP_Text _amountMessageText;

        /// <summary> CanvasGroup для управления прозрачностью и взаимодействием. </summary>
        private CanvasGroup _canvasGroup;

        /// <summary> Tween-анимация для появления/исчезновения. </summary>
        private Tween _fadeTween;

        /// <summary> Tween-анимация для плавного обновления количества. </summary>
        private Tween _countTween;

        /// <summary> Название предмета. </summary>
        private string _itemName;

        /// <summary> Актуальное количество предметов. </summary>
        private int _currentAmount;

        /// <summary> Отображаемое количество на экране (анимируется). </summary>
        private int _displayedAmount;

        /// <summary> Начальная X-позиция, на которой должно появляться уведомление. </summary>
        public float StartXPosition { get; private set; }

        /// <summary> Идентификатор предмета (для группировки и повторного обновления). </summary>
        public string ItemID { get; private set; }

        /// <summary> Текущая высота уведомления (для позиционирования). </summary>
        public float Height => RectTransform.rect.height;

        /// <summary> RectTransform, используемый для позиционирования уведомления. </summary>
        public RectTransform RectTransform { get; private set; }

        /// <summary> X-позиция за пределами экрана (для анимации входа/выхода). </summary>
        private float OffscreenXPosition => StartXPosition + Screen.width;

        #endregion

        /// <summary> Инициализация полей класса. </summary>
        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            StartXPosition = RectTransform.anchoredPosition.x;
        }

        /// <summary> Инициализировать уведомление визуалом, ID, количеством и позицией. </summary>
        /// <param name="itemSprite"> Иконка предмета. </param>
        /// <param name="amount"> Количество предметов. </param>
        /// <param name="itemId"> Уникальный идентификатор предмета. </param>
        /// <param name="itemName"> Название предмета. </param>
        public void Initialize(Sprite itemSprite, int amount, string itemId, string itemName)
        {
            _itemIconImage.sprite = itemSprite;
            ItemID = itemId;
            _itemName = itemName;

            _currentAmount = amount;
            _displayedAmount = amount;
            _canvasGroup.alpha = 0f;

            UpdateMessage();

            RectTransform.anchoredPosition = new Vector2(OffscreenXPosition, RectTransform.anchoredPosition.y);
        }

        /// <summary> Обновить текстовое сообщение с количеством и названием предмета. </summary>
        private void UpdateMessage() => _amountMessageText.text = $"x{_displayedAmount} {_itemName}";

        /// <summary> Добавить количество к уже отображаемому значению и воспроизвести анимацию. </summary>
        /// <param name="amount"> Добавляемое количество. </param>
        public void AddAmount(int amount)
        {
            int oldAmount = _currentAmount;
            _currentAmount += amount;

            _countTween?.Kill(true);
            _countTween = DOVirtual.Int(oldAmount, _currentAmount, 0.4f, newAmount =>
            {
                _displayedAmount = newAmount;
                UpdateMessage();
            });
        }

        /// <summary> Запустить анимацию появления уведомления. </summary>
        /// <param name="fadeDuration"> Длительность анимации появления. </param>
        public void Show(float fadeDuration)
        {
            _fadeTween?.Kill(true);

            DOTween.Sequence()
                .Join(_canvasGroup.DOFade(1f, fadeDuration))
                .Join(RectTransform.DOAnchorPosX(StartXPosition, fadeDuration).SetEase(Ease.OutCubic));
        }

        /// <summary> Плавно переместить уведомление в указанную позицию. </summary>
        /// <param name="anchoredPosition"> Целевая позиция (в координатах UI). </param>
        /// <param name="duration"> Длительность перемещения. </param>
        public void SetPosition(Vector2 anchoredPosition, float duration)
        {
            RectTransform.DOAnchorPos(anchoredPosition, duration).SetEase(Ease.OutCubic);
        }

        /// <summary> Плавно скрыть уведомление и удалить объект. </summary>
        /// <param name="duration"> Длительность анимации исчезновения. </param>
        public void FadeAndDestroy(float duration)
        {
            _fadeTween?.Kill(true);
            _countTween?.Kill(true);

            DOTween.Sequence()
                .Join(_canvasGroup.DOFade(0f, duration))
                .Join(RectTransform.DOAnchorPosX(OffscreenXPosition, duration).SetEase(Ease.InCubic))
                .OnComplete(() => Destroy(gameObject));
        }
    }
}