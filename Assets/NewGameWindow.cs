using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FlavorfulStory.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory
{
    public class NewGameWindow : BaseWindow
    {
        /// <summary> Поля ввода, которые игрок заполняет при запуске новой игры. </summary>
        [SerializeField] private TMP_InputField _playerName;
        
        /// <summary> Поля ввода, которые игрок заполняет при запуске новой игры. </summary>
        [SerializeField] private TMP_InputField _shopName;
        
        [SerializeField] private Button _newGameButton;
        
        [SerializeField] private Button _closeButton;

        /// <summary> Объект для отображения сообщения об ошибке. </summary>
        [SerializeField] private GameObject _errorMessage;
        
        /// <summary> Поле текста для вывода сообщений об ошибках. </summary>
        [SerializeField] private TMP_Text _errorText;
        
        private SavingWrapper _savingWrapper;
        
        private Tween _errorFadeTween;
        
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private static readonly char[] ForbiddenCharacters = {
            '"', '\'', '@', '#', '$', '%', '^', '&', '*', '(', ')', '=', '+',
            '[', ']', '{', '}', '\\', '|', '/', '<', '>', '?', '`', '~'
        };
        
        private IStringValidator _nameValidator;
        
        [Inject]
        public void Construct(SavingWrapper savingWrapper) => _savingWrapper = savingWrapper;
        
        protected override void OnOpened()
        {
            base.OnOpened();
            _closeButton.onClick.AddListener(Close);
            _newGameButton.onClick.AddListener(StartNewGame);
            _errorMessage.gameObject.SetActive(false);
            ClearInputFields();
            
            _nameValidator = new InputFieldValidatorBuilder()
                .NotEmpty()
                .MinLength(3)
                .MaxLength(20)
                .NoForbiddenCharacters(ForbiddenCharacters)
                .Build();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _errorMessage.SetActive(false);
            _errorFadeTween?.Kill();
        }

        //TODO: внутри пахнет говной
        /// <summary> Проверяет поля ввода на корректность. </summary>
        /// <returns> Возвращает True, если все поля ввода заполнены корректно, иначе False. </returns>
        private bool AreInputFieldsValid()
        {
            var inputFields = new List<TMP_InputField> { _playerName, _shopName};

            foreach (var input in inputFields)
            {
                if (!_nameValidator.IsValid(input.text, out string warningMessage))
                {
                    ShowError(warningMessage);
                    return false;
                }
            }

            _errorMessage.SetActive(false);
            return true;
        }

        private void StartNewGame()
        {
            if (!AreInputFieldsValid()) return;

            //Нарушение SRP.
            //TODO:: не должен NewGameWindow решать че-то для системы сохранения.
            // да и вообще системе сохранения нужно сделать тотальную перестройку:
            // нахер статику, перевсти на сервис, сделать более вразумительную поддержку рантайм сохранялок
            string newGameSaveFileName = string.Concat(_playerName.text, _shopName.text);
            _savingWrapper.StartNewGameAsync(newGameSaveFileName).Forget();
        }
        
        private void ClearInputFields()
        {
            _playerName.text = string.Empty;
            _shopName.text = string.Empty;
        }
        
        private void ShowError(string message)
        {
            _errorFadeTween?.Kill();

            _errorText.text = message;
            _errorMessage.SetActive(true);
            _canvasGroup.alpha = 1f;

            _errorFadeTween = _canvasGroup
                .DOFade(0f, 2.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => _errorMessage.SetActive(false));
        }
    }
}