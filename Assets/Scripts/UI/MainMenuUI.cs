using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FlavorfulStory.SceneManagement;

namespace FlavorfulStory.UI
{
    /// <summary> UI главного меню. </summary>
    public class MainMenuUI : MonoBehaviour
    {
        /// <summary> Поля ввода, которые игрок заполняет при запуске новой игры. </summary>
        [SerializeField] private TMP_InputField[] _newGameInputFields;

        /// <summary> Объект для отображения сообщения об ошибке. </summary>
        [SerializeField] private GameObject _messageError;

        /// <summary> Поле текста для вывода сообщений об ошибках. </summary>
        [SerializeField] private TMP_Text _errorText;

        /// <summary> Кнопка продолжения игры. </summary>
        /// <remarks> Активна при условии существующего файла сохранения. </remarks>
        [SerializeField] private Button _continueGameButton;

        /// <summary> Название сохраненного файла для новой игры, формируемое на основе ввода игрока. </summary>
        /// <remarks> Название формируется путем соединения строк имени игрока и названия магазина. </remarks>
        private string NewGameSaveFileName => string.Concat(_newGameInputFields.Select(field => field.text));

        /// <summary> Коллбэк из UnityAPI. </summary>
        /// <remarks> Устанавливает кнопке продолжения игры неактивное состояние,
        /// если отсутствует файл сохранения. </remarks>
        private void Start() => _continueGameButton.interactable = SavingWrapper.SaveFileExists;

        /// <summary> Обрабатывает запуск новой игры. </summary>
        /// <remarks> Проверяет корректность заполнения полей ввода и, при успехе, 
        /// инициирует запуск новой игры через систему сохранений. </remarks>
        public void OnClickNewGame()
        {
            if (!AreInputFieldsValid()) return;
            PersistentObject.Instance.SavingWrapper.StartNewGame(NewGameSaveFileName);
        }

        /// <summary> Проверяет поля ввода на корректность. </summary>
        /// <returns> Возвращает True, если все поля ввода заполнены корректно, иначе False. </returns>
        private bool AreInputFieldsValid()
        {
            foreach (var inputField in _newGameInputFields)
            {
                if (!InputFieldValidator.IsValid(inputField.text, out string warningMessage))
                {
                    _messageError.SetActive(true);
                    _errorText.text = warningMessage;
                    return false;
                }
            }

            _messageError.SetActive(false);
            return true;
        }

        /// <summary> Обрабатывает закрытие окна. </summary>
        /// <remarks> Очищает поля ввода и скрывает сообщение об ошибке. </remarks>
        public void OnClickCancel()
        {
            ClearInputFields();
            _messageError.SetActive(false);
        }

        /// <summary> Очистить текст во всех полях ввода. </summary>
        private void ClearInputFields() =>
            System.Array.ForEach(_newGameInputFields, inputField => inputField.text = string.Empty);

        /// <summary> Продолжить ранее сохраненную игру. </summary>
        public void OnClickContinue() => PersistentObject.Instance.SavingWrapper.ContinueGame();

        /// <summary> Завершить работу приложения. </summary>
        public void OnClickQuit() => Application.Quit();
    }
}