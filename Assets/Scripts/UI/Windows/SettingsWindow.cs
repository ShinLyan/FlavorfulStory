using FlavorfulStory.UI.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory
{
    [RequireComponent(typeof(ScreenSettings))]
    public class SettingsWindow : BaseWindow
    {
        [SerializeField] private Button _closeButton;

        protected override void OnOpened()
        {
            base.OnOpened();
            _closeButton.onClick.AddListener(Close);
        }
        
        protected override void OnClosed()
        {
            base.OnOpened();
            _closeButton.onClick.RemoveListener(Close);
        }
    }
}