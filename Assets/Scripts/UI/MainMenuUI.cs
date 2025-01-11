using FlavorfulStory.Control;
using FlavorfulStory.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine;

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

        /// <summary> Название сохраненного файла для новой игры, формируемое на основе ввода игрока. </summary>
        /// <remarks> Название формируется путем соединения строк имени игрока и названия магазина. </remarks>
        private string NewGameSaveFileName => string.Concat(_newGameInputFields.Select(field => field.text));

        /// <summary> Вызывается при включении объекта. Отключает управление игроком. </summary>
        private void OnEnable()
        {
            PlayerController.SwitchController(false);
        }

        /// <summary> Вызывается при выключении объекта. Включает управление игроком. </summary>
        private void OnDisable()
        {
            PlayerController.SwitchController(true);
        }

        /// <summary> Обрабатывает запуск новой игры. </summary>
        /// <remarks> Проверяет корректность заполнения полей ввода и, при успехе, 
        /// инициирует запуск новой игры через систему сохранений. </remarks>
        public void OnClickNewGame()
        {
            if (!AreInputFieldsValid()) return;
            PersistentObject.Instance.GetSavingWrapper().StartNewGame(NewGameSaveFileName);
        }

        /// <summary> Проверяет поля ввода на корректность. </summary>
        /// <returns> Возвращает True, если все поля ввода заполнены корректно, иначе False. </returns>
        private bool AreInputFieldsValid()
        {
            for (int i = 0; i < _newGameInputFields.Length; i++)
            {
                if (!InputFieldValidator.IsValid(_newGameInputFields[i].text, out string warningMessage))
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
        private void ClearInputFields()
        {
            foreach (var inputField in _newGameInputFields)
            {
                inputField.text = string.Empty;
            }
        }

        /// <summary> Продолжить ранее сохраненную игру. </summary>
        public void OnClickContinue() =>
            PersistentObject.Instance.GetSavingWrapper().ContinueGame();

        /// <summary> Вернуться в главное меню. </summary>
        /// <remarks> Загружает сцену главного меню через систему сохранений. </remarks>
        public void OnClickReturnToMainMenu()
        {
            SavingWrapper.LoadSceneByName(SceneType.MainMenu.ToString());
        }

        /// <summary> Завершить работу приложения. </summary>
        public void OnClickQuit() => Application.Quit();
    }
}