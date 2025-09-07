using FlavorfulStory.Windows.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.TimeManagement.UI
{
    /// <summary> Отображение сводки дня с анимациями. </summary>
    public class SummaryWindow : BaseWindow
    {
        /// <summary> Текст для отображения сводки. </summary>
        [SerializeField] private TMP_Text _summaryText;

        /// <summary> Кнопка старта следующего дня. </summary>
        [SerializeField] private Button _nextDayButton;

        /// <summary> Камера для отображения сводки. </summary>
        [SerializeField] private GameObject _camera;

        /// <summary> Текст сводки по умолчанию. </summary>
        public const string DefaultSummaryText = "BEST SUMMARY EVER";

        /// <summary> Устанавливает окно сводки дня. </summary>
        /// <param name="text"> Текст для отображения. </param>
        public void Setup(string text) => _summaryText.text = text;

        /// <summary> Активирует камеру при открытии окна. </summary>
        protected override void OnOpened()
        {
            _nextDayButton.onClick.AddListener(Close);
            _camera.SetActive(true);
        }

        /// <summary> Отключает камеру при закрытии окна. </summary>
        protected override void OnClosed()
        {
            _nextDayButton.onClick.RemoveListener(Close);
            _camera.SetActive(false);
        }
    }
}