using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.DialogueSystem.UI
{
    public class DialogueChoiceButton : UIButton
    {
        [SerializeField] private TMP_Text _choiceText;
        [SerializeField] private Image _hoverImage;

        public void SetText(string text) => _choiceText.text = text;

        protected override void HoverStart() => _hoverImage.gameObject.SetActive(true);

        protected override void HoverEnd() => _hoverImage.gameObject.SetActive(false);
    }
}