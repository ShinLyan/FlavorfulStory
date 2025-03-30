using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.DialogueSystem.UI
{
    /// <summary> Кнопка для отображения варианта выбора в диалоге. </summary>
    public class DialogueChoiceButton : UIButton
    {
        /// <summary> Текстовое поле для отображения текста варианта. </summary>
        [SerializeField] private TMP_Text _choiceText;

        /// <summary> Изображение, отображающее эффект наведения на кнопку. </summary>
        [SerializeField] private Image _hoverImage;

        /// <summary> Устанавливает текст варианта в кнопку. </summary>
        /// <param name="text"> Текст варианта диалога. </param>
        public void SetText(string text) => _choiceText.text = text;

        /// <summary> Включить эффект наведения. </summary>
        protected override void HoverStart() => _hoverImage.gameObject.SetActive(true);

        /// <summary> Выключить эффект наведения. </summary>
        protected override void HoverEnd() => _hoverImage.gameObject.SetActive(false);
    }
}