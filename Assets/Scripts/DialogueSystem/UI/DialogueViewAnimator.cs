using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.DialogueSystem.UI
{
    /// <summary> Отвечает за анимации панели, текста и модели диалога. </summary>
    public class DialogueViewAnimator
    {
        #region Fields and Consts

        /// <summary> Панель диалога. </summary>
        private readonly RectTransform _panel;

        /// <summary> Контейнер с кнопками выбора. </summary>
        private readonly RectTransform _choiceContainer;

        /// <summary> Текст диалога. </summary>
        private readonly TMP_Text _dialogueText;

        /// <summary> Превью модели персонажа. </summary>
        private readonly RawImage _characterPreview;

        /// <summary> Положение панели при отображении. </summary>
        private readonly Vector2 _visiblePanelPosition;

        /// <summary> Положение панели при скрытии. </summary>
        private readonly Vector2 _hiddenPanelPosition;

        /// <summary> Исходная позиция превью. </summary>
        private readonly Vector2 _previewOriginalPosition;

        /// <summary> Длительность анимации панели. </summary>
        private const float PanelTransitionDuration = 0.7f;

        /// <summary> Смещение панели вниз при скрытии. </summary>
        private const float HiddenPanelYOffset = 400f;

        /// <summary> Длительность появления текста. </summary>
        private const float TextRevealDuration = 0.6f;

        /// <summary> Длительность анимации превью. </summary>
        private const float PreviewTransitionDuration = 0.7f;

        /// <summary> Смещение превью по X при анимации. </summary>
        private const float PreviewSlideOffsetX = 60f;

        #endregion

        /// <summary> Конструктор. Кэширует компоненты, сохраняет состояния, рассчитывает позиции. </summary>
        /// <param name="panel"> Панель диалога. </param>
        /// <param name="choiceContainer"> Контейнер с кнопками выбора. </param>
        /// <param name="dialogueText"> Текст диалога. </param>
        /// <param name="characterPreview"> Превью модели персонажа. </param>
        public DialogueViewAnimator(RectTransform panel, RectTransform choiceContainer, TMP_Text dialogueText,
            RawImage characterPreview)
        {
            _panel = panel;
            _choiceContainer = choiceContainer;
            _dialogueText = dialogueText;
            _characterPreview = characterPreview;

            _visiblePanelPosition = _panel.anchoredPosition;
            _hiddenPanelPosition = _visiblePanelPosition - new Vector2(0f, HiddenPanelYOffset);
            _panel.anchoredPosition = _hiddenPanelPosition;

            _previewOriginalPosition = characterPreview.rectTransform.anchoredPosition;
        }

        /// <summary> Анимация входа панели и превью. </summary>
        public Sequence AnimateEntrance() => DOTween.Sequence()
            .Join(AnimatePreview(true))
            .Join(AnimatePanel(true));

        /// <summary> Анимация выхода панели и превью. </summary>
        public Sequence AnimateExit() => DOTween.Sequence()
            .Join(AnimatePreview(false))
            .Join(AnimatePanel(false));

        /// <summary> Анимирует постепенное появление текста. </summary>
        /// <param name="fullText"> Полный текст реплики. </param>
        /// <returns> Tween с анимацией текста. </returns>
        public Tween AnimateText(string fullText)
        {
            _dialogueText.text = fullText;
            _dialogueText.maxVisibleCharacters = 0;

            return DOTween.To(() => _dialogueText.maxVisibleCharacters,
                x => _dialogueText.maxVisibleCharacters = x,
                fullText.Length, TextRevealDuration).SetEase(Ease.Linear);
        }

        /// <summary> Анимация появления или скрытия панели. </summary>
        /// <param name="isAppearing"> true — показать, false — скрыть. </param>
        /// <returns> Tween движения панели. </returns>
        private Tween AnimatePanel(bool isAppearing)
        {
            var fromPosition = isAppearing ? _hiddenPanelPosition : _visiblePanelPosition;
            var toPosition = isAppearing ? _visiblePanelPosition : _hiddenPanelPosition;
            var easeType = isAppearing ? Ease.OutCubic : Ease.InCubic;

            _panel.anchoredPosition = fromPosition;
            return _panel.DOAnchorPos(toPosition, PanelTransitionDuration).SetEase(easeType);
        }

        /// <summary> Анимация появления или скрытия превью модели. </summary>
        /// <param name="isAppearing"> true — показать, false — скрыть. </param>
        /// <returns> Tween движения и прозрачности превью. </returns>
        private Tween AnimatePreview(bool isAppearing)
        {
            var offset = new Vector2(PreviewSlideOffsetX, 0f);
            var rect = _characterPreview.rectTransform;

            var fromPosition = _previewOriginalPosition + (isAppearing ? -offset : Vector2.zero);
            var toPosition = _previewOriginalPosition + (isAppearing ? Vector2.zero : -offset);
            float fromAlpha = isAppearing ? 0f : 1f;
            float toAlpha = isAppearing ? 1f : 0f;

            rect.anchoredPosition = fromPosition;
            _characterPreview.color = new Color(_characterPreview.color.r, _characterPreview.color.g,
                _characterPreview.color.b, fromAlpha);

            return DOTween.Sequence()
                .Join(rect.DOAnchorPos(toPosition, PreviewTransitionDuration).SetEase(Ease.InOutCubic))
                .Join(_characterPreview.DOFade(toAlpha, PreviewTransitionDuration).SetEase(Ease.InOutSine));
        }

        /// <summary> Анимация появления контейнера выбора с затуханием и сдвигом. </summary>
        public async UniTask AnimateChoicesContainer()
        {
            const float FadeDuration = 0.5f;
            const float MoveOffsetX = 200f;

            var canvasGroup = _choiceContainer.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            var originalPosision = _choiceContainer.anchoredPosition;
            _choiceContainer.anchoredPosition = originalPosision + new Vector2(MoveOffsetX, 0);

            var fade = canvasGroup.DOFade(1f, FadeDuration).SetEase(Ease.OutSine);
            var move = _choiceContainer.DOAnchorPos(originalPosision, FadeDuration).SetEase(Ease.OutQuad);

            await DOTween.Sequence().Join(fade).Join(move).AsyncWaitForCompletion();

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        /// <summary> Анимация выбора варианта и скрытие остальных кнопок. </summary>
        /// <param name="selectedButton"> Выбранная кнопка. </param>
        /// <param name="allButtons"> Все кнопки в текущем выборе. </param>
        public static async UniTask AnimateChoiceSelection(DialogueChoiceButton selectedButton,
            List<DialogueChoiceButton> allButtons)
        {
            const float fadeDuration = 0.5f;

            var fadeOutTasks = new List<UniTask>();

            foreach (var button in allButtons)
            {
                button.Interactable = false;

                if (button == selectedButton) continue;

                var canvasGroup = button.GetComponent<CanvasGroup>();
                if (canvasGroup)
                    fadeOutTasks.Add(canvasGroup.DOFade(0f, fadeDuration).AsyncWaitForCompletion().AsUniTask());
                else
                    button.gameObject.SetActive(false);
            }

            await UniTask.WhenAll(fadeOutTasks);

            var selectedCanvasGroup = selectedButton.GetComponent<CanvasGroup>();
            if (selectedCanvasGroup)
                await selectedCanvasGroup.DOFade(0f, fadeDuration).AsyncWaitForCompletion();
            else
                selectedButton.gameObject.SetActive(false);
        }
    }
}