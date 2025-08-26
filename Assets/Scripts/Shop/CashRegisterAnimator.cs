using DG.Tweening;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Shop
{
    /// <summary> Управляет визуальными эффектами кассы. </summary>
    [RequireComponent(typeof(Collider))]
    public class CashRegisterAnimator : MonoBehaviour
    {
        /// <summary> Текст с количеством денег. </summary>
        [SerializeField] private TMP_Text _moneyAmountText;

        /// <summary> Объект монетки. </summary>
        [SerializeField] public CoinAnimator _coinAnimator;

        /// <summary> Ссылка на кассу. </summary>
        private CashRegister _cashRegister;

        /// <summary> Анимация прозрачности текста. </summary>
        private Tween _textFadeTween;

        /// <summary> Анимация масштаба текста. </summary>
        private Tween _textScaleTween;

        /// <summary> Флаг нахождения игрока в триггере. </summary>
        private bool _playerInTrigger;

        /// <summary> Инициализация компонента. </summary>
        private void Awake()
        {
            _cashRegister = GetComponent<CashRegister>();
            InitializeTextTransparency(0f);

            _cashRegister.OnAmountChanged += UpdateAmountText;
            _cashRegister.OnAmountChanged += UpdateCoinState;
        }

        private void OnDestroy()
        {
            _cashRegister.OnAmountChanged -= UpdateAmountText;
            _cashRegister.OnAmountChanged -= UpdateCoinState;
        }

        /// <summary> Обновляет текст с количеством денег. </summary>
        /// <param name="amount"> Новое количество денег. </param>
        private void UpdateAmountText(int amount) => _moneyAmountText.text = amount.ToString();

        /// <summary> Обновляет состояние монетки. </summary>
        /// <param name="amount"> Текущее количество денег. </param>
        private void UpdateCoinState(int amount) => _coinAnimator.ToggleCoin(amount > 0 && !_playerInTrigger);

        /// <summary> Обрабатывает вход в триггер. </summary>
        /// <param name="other"> Коллайдер вошедшего объекта. </param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _playerInTrigger = true;
            _coinAnimator.ToggleCoin(false);
            ShowText(true);
        }

        /// <summary> Обрабатывает выход из триггера. </summary>
        /// <param name="other"> Коллайдер вышедшего объекта. </param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _playerInTrigger = false;
            ShowText(false);
            UpdateCoinState(_cashRegister.Amount);
        }

        /// <summary> Устанавливает прозрачность текста. </summary>
        /// <param name="alpha"> Значение прозрачности. </param>
        private void InitializeTextTransparency(float alpha)
        {
            var color = _moneyAmountText.color;
            _moneyAmountText.color = new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary> Показывает/скрывает текст с анимацией. </summary>
        /// <param name="show"> Показать текст. </param>
        /// <param name="duration"> Длительность анимации. </param>
        private void ShowText(bool show, float duration = 0.3f)
        {
            _textFadeTween?.Kill();
            _textScaleTween?.Kill();

            const float MinScaleValue = 0.8f;
            if (show)
            {
                _moneyAmountText.gameObject.SetActive(true);

                _moneyAmountText.color = new Color(1f, 1f, 1f, 0f);
                _moneyAmountText.transform.localScale = Vector3.one * MinScaleValue;

                _textFadeTween = _moneyAmountText.DOFade(1f, duration).SetEase(Ease.OutSine);
                _textScaleTween = _moneyAmountText.transform.DOScale(1f, duration).SetEase(Ease.OutBack);
            }
            else
            {
                _textFadeTween = _moneyAmountText.DOFade(0f, duration).SetEase(Ease.InSine);
                _textScaleTween = _moneyAmountText.transform.DOScale(MinScaleValue, duration).SetEase(Ease.InBack)
                    .OnComplete(() => { _moneyAmountText.gameObject.SetActive(false); });
            }
        }

        /// <summary> Переключает видимость монетки. </summary>
        /// <param name="show"> Показать монетку. </param>
        public void ToggleCoin(bool show) => _coinAnimator.ToggleCoin(show);
    }
}