using FlavorfulStory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.QuestSystem
{
    public class QuestListButton : CustomButton
    {
        [SerializeField] private TMP_Text _questNameText;

        [SerializeField] private Image _hoverImage;

        public void Setup(Quest quest) { _questNameText.text = quest.QuestName; }

        public void SetText(string text) => _questNameText.text = text;

        protected override void OnInteractionEnabled() => _hoverImage.gameObject.SetActive(false);

        protected override void OnInteractionDisabled() => _hoverImage.gameObject.SetActive(true);

        protected override void Click()
        {
            base.Click();
            Interactable = false;
        }
    }
}