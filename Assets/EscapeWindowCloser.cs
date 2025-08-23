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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!_windowService.HasOpenWindows)
                    _windowService.OpenWindow<GameMenuWindow>();
                else
                    _windowService.CloseTopWindow();
            }
        }
    }
}