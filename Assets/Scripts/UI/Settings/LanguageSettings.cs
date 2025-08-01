using System.Linq;
using FlavorfulStory.LocalizationSystem;
using TMPro;
using UnityEngine;

namespace FlavorfulStory
{
    public class LanguageSettings : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _languageDropdown;

        private void OnEnable()
        {
            _languageDropdown.onValueChanged.AddListener(LanguageOptionChanged);
            LocalizationService.OnLanguageChanged += UpdateSelectedLanguage;
        }

        private void OnDisable()
        {
            _languageDropdown.onValueChanged.RemoveListener(LanguageOptionChanged);
            LocalizationService.OnLanguageChanged -= UpdateSelectedLanguage;
        }

        private void Start()
        {
            InitializeAvailableLanguages();
            UpdateSelectedLanguage();
        }

        private void LanguageOptionChanged(int selectedIndex)
        {
            string selectedLanguage = _languageDropdown.options[selectedIndex].text;
            LocalizationService.SetLanguageStatic(selectedLanguage);
        }

        private void InitializeAvailableLanguages()
        {
            _languageDropdown.ClearOptions();

            if (LocalizationService.IsInitialized)
            {
                var languages = LocalizationService.GetAllLanguages().ToList();
                _languageDropdown.AddOptions(languages);
            }
            else
            {
                Debug.LogWarning("LocalizationService is not initialized!");
            }
        }

        private void UpdateSelectedLanguage()
        {
            if (!LocalizationService.IsInitialized) return;

            string currentLang = LocalizationService.CurrentLanguage;
            int langIndex = _languageDropdown.options.FindIndex(option => option.text == currentLang);

            if (langIndex >= 0) _languageDropdown.value = langIndex;
        }
    }
}