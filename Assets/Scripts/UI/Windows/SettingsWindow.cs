using UnityEngine;
using UnityEngine.UI;
using FlavorfulStory.UI.Settings;

namespace FlavorfulStory.UI.Windows
{
    /// <summary> Окно настроек с кнопкой закрытия. </summary>
    [RequireComponent(typeof(ScreenSettings))]
    public class SettingsWindow : BaseWindow
    {
        /// <summary> Кнопка для закрытия окна. </summary>
        [SerializeField] private Button _closeButton;

        /// <summary> Подписка на кнопку при открытии окна. </summary>
        protected override void OnOpened()
        {
            base.OnOpened();
            _closeButton.onClick.AddListener(Close);
        }
        
        /// <summary> Отписка от кнопки при закрытии окна. </summary>
        protected override void OnClosed()
        {
            base.OnOpened();
            _closeButton.onClick.RemoveListener(Close);
        }
    }
}