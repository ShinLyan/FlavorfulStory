using TMPro;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public class LocalizedLabel : MonoBehaviour
    {
        [SerializeField] private string _localizationKey;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            LocalizationService.OnLanguageChanged += UpdateText;
            UpdateText();
        }

        private void OnDestroy()
        {
            if (LocalizationService.IsInitialized) LocalizationService.OnLanguageChanged -= UpdateText;
        }

        private void UpdateText()
        {
            string newText = LocalizationService.GetLocalizedString(_localizationKey);
            if (string.IsNullOrEmpty(newText)) return;
            _text.text = newText;
        }
    }
}