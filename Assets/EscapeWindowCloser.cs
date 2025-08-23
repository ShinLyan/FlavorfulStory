using FlavorfulStory.InputSystem;
using FlavorfulStory.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public class EscapeWindowCloser : MonoBehaviour
    {
        private IWindowService _windowService;

        [InjectOptional] private UIOverlayFadeCoordinator _fadeCoordinator;
        
        [Inject]
        private void Construct(IWindowService windowService) => _windowService = windowService;

        private void Update()
        {
            if (InputWrapper.GetButtonDown(InputButton.SwitchGameMenu))
            {
                if (!_windowService.HasOpenWindows && _fadeCoordinator != null)
                    _windowService.OpenWindow<GameMenuWindow>();
                else
                    _windowService.CloseTopWindow();
            }
        }
    }
}