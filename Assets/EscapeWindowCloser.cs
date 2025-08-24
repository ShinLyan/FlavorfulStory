using FlavorfulStory.InputSystem;
using FlavorfulStory.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public class EscapeWindowCloser : MonoBehaviour
    {
        private IWindowService _windowService;
        private UIOverlayFadeCoordinator _fadeCoordinator;

        [Inject]
        private void Construct(IWindowService windowService,[InjectOptional] UIOverlayFadeCoordinator fadeCoordinator)
        {
            _windowService = windowService;
            _fadeCoordinator = fadeCoordinator;
        } 

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