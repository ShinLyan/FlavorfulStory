using FlavorfulStory.Control;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> Интерфейс главного меню. </summary>
    public class MainMenuUI : MonoBehaviour
    {
        /// <summary> Поля ввода, заполняемые игроком при старте новой игры. </summary>
        [SerializeField] private TMP_InputField[] _newGameInputFields;

        /// <summary> Объект отображения ошибки. </summary>
        [SerializeField] private GameObject _messageError;

        /// <summary> Поле текста для вывода сообщений об ошибках. </summary>
        [SerializeField] private TMP_Text _errorText;

        /// <summary> Название сохраненного файла для новой игры. </summary>
        /// <remarks> Формируется путем объединения значений полей ввода. </remarks>
        private string NewGameSaveFileName => string.Concat(_newGameInputFields.Select(field => field.text));

        /// <summary> Подписка на событие включения. Отключает управление игроком. </summary>
        private void OnEnable()
        {
            PlayerController.SwitchController(false);
        }

        /// <summary> Подписка на событие выключения. Включает управление игроком. </summary>
        private void OnDisable()
        {
            PlayerController.SwitchController(true);
        }

        /// <summary> Запуск новой игры. </summary>
        /// <remarks> Проверяет корректность введенных данных, затем запускает новую игру. </remarks>
        public void OnClickNewGame()
        {
            if (!AreInputFieldsValid()) return;
            PersistentObject.Instance.GetSavingWrapper().StartNewGame(NewGameSaveFileName);
        }

        /// <summary> Проверка корректности введенных данных. </summary>
        /// <returns> Возвращает True, если данные корректны, иначе False. </returns>
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

        /// <summary> Отмена ввода данных. </summary>
        /// <remarks> Очищает поля ввода и скрывает сообщения об ошибках. </remarks>
        public void OnClickCancel()
        {
            ClearInputFields();
            _messageError.SetActive(false);
        }

        /// <summary> Очистка полей ввода. </summary>
        private void ClearInputFields()
        {
            foreach (var inputField in _newGameInputFields)
            {
                inputField.text = string.Empty;
            }
        }

        /// <summary> Продолжение сохраненной игры. </summary>
        public void OnClickContinue() =>
            PersistentObject.Instance.GetSavingWrapper().ContinueGame();

        /// <summary> Выход из приложения. </summary>
        public void OnClickQuit() => Application.Quit();
    }
}