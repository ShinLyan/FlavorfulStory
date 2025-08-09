using DG.Tweening;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.Shop
{
    /// <summary> Отвечает за визуальную часть кассы: анимации текста, показ монетки, триггер игрока. </summary>
    [RequireComponent(typeof(Collider))]
    public class CashRegisterAnimator : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyAmountText;

        [SerializeField] public GameObject _coin;

        private CashRegister _cashRegister;
        private Tween _textFadeTween;
        private Tween _textScaleTween;
        private bool _playerInTrigger;

        private void Awake()
        {
            _cashRegister = GetComponent<CashRegister>();
            _cashRegister.OnAmountChanged += UpdateAmountText;
            _cashRegister.OnAmountChanged += UpdateCoinState;

            InitializeTextTransparency(0f);
        }

        private void UpdateAmountText(int amount) => _moneyAmountText.text = amount.ToString();

        private void UpdateCoinState(int amount) => _coin.SetActive(amount > 0 && !_playerInTrigger);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _playerInTrigger = true;
            _coin.SetActive(false);
            ShowText(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _playerInTrigger = false;
            ShowText(false);
            UpdateCoinState(_cashRegister.Amount);
        }

        private void InitializeTextTransparency(float alpha)
        {
            var color = _moneyAmountText.color;
            _moneyAmountText.color = new Color(color.r, color.g, color.b, alpha);
        }

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
                _textScaleTween = _moneyAmountText.transform
                    .DOScale(1f, duration)
                    .SetEase(Ease.OutBack);
            }
            else
            {
                _textFadeTween = _moneyAmountText.DOFade(0f, duration).SetEase(Ease.InSine);
                _textScaleTween = _moneyAmountText.transform
                    .DOScale(MinScaleValue, duration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => { _moneyAmountText.gameObject.SetActive(false); });
            }
        }

        public void ToggleCoin(bool show) => _coin.SetActive(show);
    }
}