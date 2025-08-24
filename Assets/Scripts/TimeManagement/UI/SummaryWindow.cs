using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlavorfulStory.TimeManagement.UI
{
    /// <summary> Отображение сводки дня с анимациями. </summary>
    public class SummaryWindow : BaseWindow
    {
        /// <summary> Текст для отображения сводки. </summary>
        [SerializeField] private TMP_Text _summaryText;
        /// <summary> Кнопка закрытия окна. </summary>
        [SerializeField] private Button _closeButton;
        /// <summary> Камера для отображения сводки. </summary>
        [SerializeField] private GameObject _camera;

        /// <summary> Текст сводки по умолчанию. </summary>
        public const string DefaultSummaryText = "BEST SUMMARY EVER";
        
        private void OnEnable() => _closeButton.onClick.AddListener(Close);
        private void OnDisable() => _closeButton.onClick.RemoveListener(Close);

        /// <summary> Устанавливает текст сводки. </summary>
        /// <param name="text"> Текст для отображения. </param>
        public void SetSummary(string text) => _summaryText.text = text;

        protected override void OnOpened()
        {
            base.OnOpened();
            _camera.SetActive(true);
        }
        
        protected override void OnClosed()
        {
            base.OnClosed();
            _camera.SetActive(false);
        }
    }
}