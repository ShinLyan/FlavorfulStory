using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FlavorfulStory.SceneManagement;
using FlavorfulStory.Utils.StringValidator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FlavorfulStory.UI.Windows
{
    /// <summary> Окно создания новой игры: ввод имени игрока, названия магазина и валидация. </summary>
    public class NewGameWindow : BaseWindow
    {
        /// <summary> Поля ввода, которые игрок заполняет при запуске новой игры. </summary>
        [SerializeField] private TMP_InputField _playerName;

        /// <summary> Поля ввода, которые игрок заполняет при запуске новой игры. </summary>
        [SerializeField] private TMP_InputField _shopName;

        /// <summary> Кнопка начала новой игры. </summary>
        [SerializeField] private Button _newGameButton;

        /// <summary> Кнопка закрытия окна. </summary>
        [SerializeField] private Button _closeButton;

        /// <summary> Объект для отображения сообщения об ошибке. </summary>
        [SerializeField] private GameObject _errorMessage;

        /// <summary> Поле текста для вывода сообщений об ошибках. </summary>
        [SerializeField] private TMP_Text _errorText;

        /// <summary> CanvasGroup, управляющий прозрачностью плашки ошибки. </summary>
        [SerializeField] private CanvasGroup _canvasGroup;

        /// <summary> Ссылка на систему сохранений. </summary>
        private SavingWrapper _savingWrapper;

        /// <summary> Активный tween ошибки. </summary>
        private Tween _errorFadeTween;

        /// <summary> Валидатор строк (имя игрока и магазина). </summary>
        private IStringValidator _nameValidator;

        /// <summary> Символы, запрещённые во вводе. </summary>
        private static readonly char[] ForbiddenCharacters =
        {
            '"', '\'', '@', '#', '$', '%', '^', '&', '*', '(', ')', '=', '+',
            '[', ']', '{', '}', '\\', '|', '/', '<', '>', '?', '`', '~'
        };

        /// <summary> Внедрение зависимостей Zenject. </summary>
        [Inject]
        public void Construct(SavingWrapper savingWrapper) => _savingWrapper = savingWrapper;

        /// <summary> Обработчик открытия окна: очистка, подписки, валидация. </summary>
        protected override void OnOpened()
        {
            _closeButton.onClick.AddListener(Close);
            _newGameButton.onClick.AddListener(StartNewGame);
            _newGameButton.onClick.AddListener(Close);
            _errorMessage.gameObject.SetActive(false);
            ClearInputFields();

            _nameValidator = new StringValidatorBuilder().NotEmpty().MinLength(3).MaxLength(20)
                .NoForbiddenCharacters(ForbiddenCharacters).Build();
        }

        /// <summary> Обработчик закрытия окна: очистка tween и скрытие ошибки. </summary>
        protected override void OnClosed()
        {
            _closeButton.onClick.RemoveListener(Close);
            _newGameButton.onClick.RemoveListener(Close);
            _errorMessage.SetActive(false);
            _errorFadeTween?.Kill();
        }

        /// <summary> Проверяет поля ввода на корректность. </summary>
        /// <returns> Возвращает True, если все поля ввода заполнены корректно, иначе False. </returns>
        private bool AreInputFieldsValid()
        {
            var inputFields = new List<TMP_InputField> { _playerName, _shopName };

            foreach (var input in inputFields)
                if (!_nameValidator.IsValid(input.text, out string warningMessage))
                {
                    ShowError(warningMessage);
                    return false;
                }

            _errorMessage.SetActive(false);
            return true;
        }

        /// <summary> Обработчик нажатия кнопки "Start New Game". </summary>
        private void StartNewGame()
        {
            if (!AreInputFieldsValid()) return;

            // TODO: Нарушение SRP. Нужно вынести в отдельный сервис / use case.
            string newGameSaveFileName = string.Concat(_playerName.text, _shopName.text);
            _savingWrapper.StartNewGameAsync(newGameSaveFileName).Forget();
        }

        /// <summary> Очищает поля ввода. </summary>
        private void ClearInputFields()
        {
            _playerName.text = string.Empty;
            _shopName.text = string.Empty;
        }

        /// <summary> Показывает сообщение об ошибке с fade-эффектом. </summary>
        private void ShowError(string message)
        {
            _errorFadeTween?.Kill();

            _errorText.text = message;
            _errorMessage.SetActive(true);
            _canvasGroup.alpha = 1f;

            _errorFadeTween = _canvasGroup.DOFade(0f, 2.5f).SetEase(Ease.InOutSine)
                .OnComplete(() => _errorMessage.SetActive(false));
        }
    }
}