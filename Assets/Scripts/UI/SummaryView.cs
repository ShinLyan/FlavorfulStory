using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FlavorfulStory.InputSystem;
using FlavorfulStory.UI.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.UI
{
    /// <summary> Отображение сводки дня с анимациями. </summary>
    public class SummaryView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _summaryText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private GameObject _camera;

        public const string DefaultSummaryText = "BEST SUMMARY EVER";

        private CanvasGroupFader _hudFader;

        private CanvasGroupFader _fader;

        public Action OnContinuePressed;

        [Inject]
        public void Construct([Inject(Id = "HUD")] CanvasGroupFader hudFader) { _hudFader = hudFader; }

        private void Awake()
        {
            _fader = GetComponent<CanvasGroupFader>();
            _continueButton.onClick.AddListener(() => OnContinuePressed?.Invoke());
        }

        public void SetSummary(string text) => _summaryText.text = text;

        /// <summary> Показывает UI сводки с анимацией. </summary>
        public async UniTask ShowWithAnimation()
        {
            InputWrapper.BlockAllInput();

            gameObject.SetActive(true);
            _camera.SetActive(true);

            _fader.Show();
            await _hudFader.Hide().AsyncWaitForCompletion();
        }

        /// <summary> Ждёт нажатия кнопки продолжения с анимацией. </summary>
        public async UniTask WaitForContinue(CancellationToken token = default)
        {
            bool continuePressed = false;
            OnContinuePressed = () => continuePressed = true;

            await UniTask.WaitUntil(() => continuePressed, cancellationToken: token);
            await _hudFader.Hide().AsyncWaitForCompletion();
        }

        /// <summary> Скрывает UI сводки с анимацией. </summary>
        public async UniTask HideWithAnimation()
        {
            await _fader.Hide().AsyncWaitForCompletion();
            gameObject.SetActive(false);
            _camera.SetActive(false);
            InputWrapper.UnblockAllInput();
            await _hudFader.Show().AsyncWaitForCompletion();
        }
    }
}