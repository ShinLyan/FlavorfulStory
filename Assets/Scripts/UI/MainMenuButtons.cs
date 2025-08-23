using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using FlavorfulStory.SceneManagement;
using Zenject;

namespace FlavorfulStory.UI
{
    /// <summary> UI главного меню. </summary>
    public class MainMenuButtons : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        
        /// <summary> Кнопка продолжения игры. </summary>
        /// <remarks> Отображается при условии существующего файла сохранения. </remarks>
        [SerializeField] private Button _continueGameButton;

        /// <summary> Кнопка загрузки игры. </summary>
        /// <remarks> Отображается при условии существующего файла сохранения. </remarks>
        [SerializeField] private Button _loadGameButton;

        [SerializeField] private Button _settingsButton;
        
        [SerializeField] private Button _exitGameButton;
        
        /// <summary> Ссылка на компонент, управляющий системой сохранений. </summary>
        private SavingWrapper _savingWrapper;

        [Inject] private readonly IWindowService _windowService;
        
        /// <summary> Внедрение зависимости компонента сохранений. </summary>
        /// <param name="savingWrapper"> Экземпляр SavingWrapper. </param>
        [Inject]
        public void Construct(SavingWrapper savingWrapper) => _savingWrapper = savingWrapper;

        /// <summary> При старте включает отображение кнопок при условии существующего файла сохранения. </summary>
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

        private void OnDestroy()
        {
            _newGameButton.onClick.RemoveListener(OpenNewGameWindow);
            _continueGameButton.onClick.RemoveListener(ContinueGame);
            _settingsButton.onClick.RemoveListener(OpenSettingsWindow);
            _exitGameButton.onClick.RemoveListener(ShowExitConfirmationWindow);
        } 
        
        private void OpenNewGameWindow() => _windowService.OpenWindow<NewGameWindow>();
        
        /// <summary> Продолжить ранее сохраненную игру. </summary>
        public void ContinueGame() => _savingWrapper.ContinueGameAsync().Forget();

        private void OpenSettingsWindow() => _windowService.OpenWindow<SettingsWindow>();

        /// <summary> Завершить работу приложения. </summary>
        private void ShowExitConfirmationWindow() => _windowService.OpenWindow<ExitConfirmationWindow>();
    }
}