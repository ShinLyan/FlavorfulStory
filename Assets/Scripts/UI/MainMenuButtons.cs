using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;
using FlavorfulStory.Infrastructure.Services.WindowService;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.UI.Windows;

namespace FlavorfulStory.UI
{
    /// <summary> Логика кнопок главного меню. Управляет окнами и запуском игры. </summary>
    public class MainMenuButtons : MonoBehaviour
    {
        /// <summary> Кнопка начала новой игры. </summary>
        [SerializeField] private Button _newGameButton;
        /// <summary> Кнопка продолжения игры. </summary>
        /// <remarks> Отображается при условии существующего файла сохранения. </remarks>
        [SerializeField] private Button _continueGameButton;
        /// <summary> Кнопка загрузки игры. </summary>
        /// <remarks> Отображается при условии существующего файла сохранения. </remarks>
        [SerializeField] private Button _loadGameButton;
        /// <summary> Кнопка открытия окна настроек. </summary>
        [SerializeField] private Button _settingsButton;
        /// <summary> Кнопка выхода из игры. </summary>
        [SerializeField] private Button _exitGameButton;
        
        /// <summary> Сервис управления окнами. </summary>
        [Inject] private readonly IWindowService _windowService;
        
        /// <summary> Ссылка на компонент, управляющий системой сохранений. </summary>
        private SavingWrapper _savingWrapper;
        
        /// <summary> Внедрение зависимости компонента сохранений. </summary>
        /// <param name="savingWrapper"> Экземпляр SavingWrapper. </param>
        [Inject]
        public void Construct(SavingWrapper savingWrapper) => _savingWrapper = savingWrapper;

        /// <summary> Настраивает кнопки и скрывает ненужные при отсутствии сохранений. </summary>
        private void Start()
        {
            bool saveExists = SavingWrapper.SaveFileExists;
            _continueGameButton.gameObject.SetActive(saveExists);
            _loadGameButton.gameObject.SetActive(saveExists);
            
            _newGameButton.onClick.AddListener(OpenNewGameWindow);
            _continueGameButton.onClick.AddListener(ContinueGame);
            _settingsButton.onClick.AddListener(OpenSettingsWindow);
            _exitGameButton.onClick.AddListener(ShowExitConfirmationWindow);
        }

        /// <summary> Отписка от событий при уничтожении. </summary>
        private void OnDestroy()
        {
            _newGameButton.onClick.RemoveListener(OpenNewGameWindow);
            _continueGameButton.onClick.RemoveListener(ContinueGame);
            _settingsButton.onClick.RemoveListener(OpenSettingsWindow);
            _exitGameButton.onClick.RemoveListener(ShowExitConfirmationWindow);
        } 
        
        /// <summary> Открывает окно создания новой игры. </summary>
        private void OpenNewGameWindow() => _windowService.OpenWindow<NewGameWindow>();
        
        /// <summary> Продолжить ранее сохраненную игру. </summary>
        public void ContinueGame() => _savingWrapper.ContinueGameAsync().Forget();

        /// <summary> Открывает окно настроек. </summary>
        private void OpenSettingsWindow() => _windowService.OpenWindow<SettingsWindow>();

        /// <summary> Открывает окно подтверждения выхода из игры. </summary>
        private void ShowExitConfirmationWindow() => _windowService.OpenWindow<ExitConfirmationWindow>();
    }
}