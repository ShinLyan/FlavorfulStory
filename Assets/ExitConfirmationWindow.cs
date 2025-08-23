using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory
{
    public class ExitConfirmationWindow : BaseWindow
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        
        protected override void OnOpened()
        {
            base.OnOpened();
            _yesButton.onClick.AddListener(ExitGame);
            _noButton.onClick.AddListener(Close);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _yesButton.onClick.RemoveListener(ExitGame);
            _noButton.onClick.RemoveListener(Close);
        }
        
        private void ExitGame() => Application.Quit();
    }
}