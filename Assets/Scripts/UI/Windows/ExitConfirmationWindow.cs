using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.UI.Windows
{
    /// <summary> Окно подтверждения выхода из игры. </summary>
    public class ExitConfirmationWindow : BaseWindow
    {
        /// <summary> Кнопка подтверждения выхода. </summary>
        [SerializeField] private Button _yesButton;
        /// <summary> Кнопка отмены и закрытия окна. </summary>
        [SerializeField] private Button _noButton;
        
        /// <summary> Подписывает кнопки при открытии окна. </summary>
        protected override void OnOpened()
        {
            base.OnOpened();
            _yesButton.onClick.AddListener(ExitGame);
            _noButton.onClick.AddListener(Close);
        }

        /// <summary> Удаляет подписки при закрытии окна. </summary>
        protected override void OnClosed()
        {
            base.OnClosed();
            _yesButton.onClick.RemoveListener(ExitGame);
            _noButton.onClick.RemoveListener(Close);
        }
        
        /// <summary> Завершает работу приложения. </summary>
        private void ExitGame() => Application.Quit();
    }
}