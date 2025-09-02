using FlavorfulStory.InputSystem;
using UnityEngine;
using Zenject;

namespace FlavorfulStory.Windows.UI
{
    /// <summary> Обработчик нажатия ESC / кнопки меню. Открывает или закрывает окна. </summary>
    public class EscapeWindowCloser : MonoBehaviour
    {
        /// <summary> Сервис управления UI-окнами. </summary>
        private IWindowService _windowService;

        /// <summary> Координатор fade-интерфейса, нужен только в геймплейной сцене. </summary>
        private UIOverlayFadeCoordinator _fadeCoordinator;

        /// <summary> Внедрение зависимостей. </summary>
        /// <param name="windowService"> Сервис управления UI-окнами. </param>
        /// <param name="fadeCoordinator"> Координатор fade-интерфейса, нужен только в геймплейной сцене. </param>
        [Inject]
        private void Construct(IWindowService windowService, [InjectOptional] UIOverlayFadeCoordinator fadeCoordinator)
        {
            _windowService = windowService;
            _fadeCoordinator = fadeCoordinator;
        }

        /// <summary> Проверяет ввод и открывает/закрывает окна при нажатии кнопки меню. </summary>
        private void Update()
        {
            if (!InputWrapper.GetButtonDown(InputButton.SwitchGameMenu)) return;

            if (!_windowService.HasOpenWindows && _fadeCoordinator != null)
                _windowService.OpenWindow<GameMenuWindow>();
            else
                _windowService.CloseTopWindow();
        }
    }
}