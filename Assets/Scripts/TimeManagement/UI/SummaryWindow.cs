using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Zenject;
using FlavorfulStory.UI.Animation;
using FlavorfulStory.InputSystem;

namespace FlavorfulStory.TimeManagement.UI
{
    //TODO: Довестьи до ума - не проебаться
    /// <summary> Отображение сводки дня с анимациями. </summary>
    public class SummaryWindow : MonoBehaviour
    {
        /// <summary> Текст для отображения сводки. </summary>
        [SerializeField] private TMP_Text _summaryText;
        
        /// <summary> Кнопка закрытия окна. </summary>
        /// <remarks> При закрытия окна - игра продолжается. </remarks>
        [SerializeField] private Button _closeButton;

        /// <summary> Камера для отображения сводки. </summary>
        [SerializeField] private GameObject _camera;

        /// <summary> Текст сводки по умолчанию. </summary>
        public const string DefaultSummaryText = "BEST SUMMARY EVER";

        /// <summary> Фейдер HUD интерфейса. </summary>
        private CanvasGroupFader _hudFader;

        /// <summary> Фейдер сводки. </summary>
        private CanvasGroupFader _fader;

        /// <summary> Событие нажатия кнопки продолжения. </summary>
        public Action OnContinuePressed;

        /// <summary> Инициализация зависимостей. </summary>
        /// <param name="hudFader"> Фейдер HUD интерфейса. </param>
        [Inject]
        public void Construct([Inject(Id = "HUD")] CanvasGroupFader hudFader) { _hudFader = hudFader; }

        /// <summary> Настройка компонентов при старте. </summary>
        private void Awake()
        {
            _fader = GetComponent<CanvasGroupFader>();
            _closeButton.onClick.AddListener(() => OnContinuePressed?.Invoke());
        }

        /// <summary> Устанавливает текст сводки. </summary>
        /// <param name="text"> Текст для отображения. </param>
        public void SetSummary(string text) => _summaryText.text = text;

        /// <summary> Показывает UI сводки с анимацией. </summary>
        /// <returns> Задача анимации. </returns>
        public async UniTask ShowWithAnimation()
        {
            InputWrapper.BlockAllInput();
            gameObject.SetActive(true);
            _camera.SetActive(true);
            _fader.Show();
            await _hudFader.Hide().AsyncWaitForCompletion();
        }

        /// <summary> Ожидает нажатия кнопки продолжения. </summary>
        /// <returns> Задача ожидания. </returns>
        public async UniTask WaitForContinue()
        {
            bool continuePressed = false;
            OnContinuePressed = () => continuePressed = true;
            await UniTask.WaitUntil(() => continuePressed);
            await _hudFader.Hide().AsyncWaitForCompletion();
        }

        /// <summary> Скрывает UI сводки с анимацией. </summary>
        /// <returns> Задача анимации. </returns>
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