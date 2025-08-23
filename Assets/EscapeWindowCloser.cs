using FlavorfulStory.InputSystem;
using FlavorfulStory.UI;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public class EscapeWindowCloser : MonoBehaviour
    {
        private IWindowService _windowService;
        
        [Inject]
        private void Construct(IWindowService windowService) => _windowService = windowService;

        private void Update()
        {
            if (InputWrapper.GetButtonDown(InputButton.SwitchGameMenu))
            {
                if (!_windowService.HasOpenWindows)
                    _windowService.OpenWindow<GameMenuWindow>();
                else
                    _windowService.CloseTopWindow();
            }
        }
    }
}