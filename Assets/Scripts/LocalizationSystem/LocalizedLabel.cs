using TMPro;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Компонент для автоматического обновления текста при изменении языка локализации. </summary>
    public class LocalizedLabel : MonoBehaviour
    {
        /// <summary> Ключ локализации для текстового поля. </summary>
        [SerializeField] private string _localizationKey;

        /// <summary> Ссылка на компонент текста. </summary>
        private TMP_Text _text;

        /// <summary> Инициализирует компонент и подписывается на события изменения языка. </summary>
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            UpdateText();
            LocalizationService.OnLanguageChanged += UpdateText;
        }

        /// <summary> Отписывается от событий при уничтожении объекта. </summary>
        private void OnDestroy()
        {
            if (LocalizationService.IsInitialized) LocalizationService.OnLanguageChanged -= UpdateText;
        }

        /// <summary> Обновляет текст в соответствии с текущим языком локализации. </summary>
        private void UpdateText()
        {
            string newText = LocalizationService.GetLocalizedString(_localizationKey);
            if (string.IsNullOrEmpty(newText))
            {
                _text.text = $"[{_localizationKey}]";
                return;
            }

            _text.text = newText;
        }
    }
}