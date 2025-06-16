using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace FlavorfulStory.DialogueSystem.UI
{
    /// <summary> Отвечает за анимации панели, текста и модели диалога. </summary>
    public class DialogueViewAnimator
    {
        private readonly RectTransform _panel;
        private readonly TMP_Text _dialogueText;

        private readonly Vector2 _visiblePosition;
        private readonly Vector2 _hiddenPosition;

        private const float PanelTransitionDuration = 0.7f;
        private const float TextRevealDuration = 0.6f;
        private const float ModelTransitionDuration = 0.7f;
        private const float ModelSlideOffsetX = 0.2f;
        private const float HiddenPanelYOffset = 400f;

        public DialogueViewAnimator(RectTransform panel, TMP_Text dialogueText)
        {
            _panel = panel;
            _dialogueText = dialogueText;

            _visiblePosition = _panel.anchoredPosition;
            _hiddenPosition = _visiblePosition - new Vector2(0f, HiddenPanelYOffset);
            _panel.anchoredPosition = _hiddenPosition;
        }

        public Tween AnimatePanelIn() =>
            _panel.DOAnchorPos(_visiblePosition, PanelTransitionDuration).SetEase(Ease.OutBack);

        public Tween AnimatePanelOut() =>
            _panel.DOAnchorPos(_hiddenPosition, PanelTransitionDuration).SetEase(Ease.InBack);

        public Tween AnimateText(string fullText)
        {
            _dialogueText.text = fullText;
            _dialogueText.maxVisibleCharacters = 0;

            return DOTween.To(() => _dialogueText.maxVisibleCharacters,
                x => _dialogueText.maxVisibleCharacters = x,
                fullText.Length, TextRevealDuration).SetEase(Ease.Linear);
        }

        public Tween AnimateModelAppearance(GameObject model, bool isAppearing)
        {
            var sequence = DOTween.Sequence();
            var renderers = model.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            foreach (var material in renderer.materials)
            {
                if (!material.HasProperty("_Color")) continue;

                EnableMaterialTransparency(material);

                if (isAppearing)
                {
                    var color = material.color;
                    color.a = 0f;
                    material.color = color;

                    sequence.Join(material.DOFade(1f, ModelTransitionDuration).SetEase(Ease.InOutSine));
                }
                else
                {
                    sequence.Join(material.DOFade(0f, ModelTransitionDuration).SetEase(Ease.InOutSine));
                }
            }

            var modelTransform = model.transform;
            var originalPosition = modelTransform.localPosition;
            var offset = new Vector3(ModelSlideOffsetX, 0f, 0f);

            if (isAppearing)
            {
                modelTransform.localPosition = originalPosition + offset;
                sequence.Join(modelTransform.DOLocalMove(originalPosition, ModelTransitionDuration)
                    .SetEase(Ease.OutCubic));
            }
            else
            {
                sequence.Join(modelTransform.DOLocalMove(originalPosition + offset, ModelTransitionDuration)
                    .SetEase(Ease.InCubic));
                sequence.OnComplete(() => Object.Destroy(model));
            }

            return sequence;
        }

        private static void EnableMaterialTransparency(Material material)
        {
            const float TransparentSurfaceValue = 1f;
            const float AlphaBlendMode = 0f;

            material.SetFloat("_Surface", TransparentSurfaceValue);
            material.SetFloat("_Blend", AlphaBlendMode);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
    }
}