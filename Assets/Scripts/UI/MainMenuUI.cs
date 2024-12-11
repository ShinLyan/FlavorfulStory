using FlavorfulStory.Control;
using FlavorfulStory.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI
{
    /// <summary> UI главного меню.</summary>
    public class MainMenuUI : MonoBehaviour
    {
        /// <summary> Поля ввода, которые игрок заполняет при запуске новой игры.</summary>
        [SerializeField] private TMP_InputField[] _newGameInputFields;

        /// <summary> Тексты предупреждений.</summary>
        [SerializeField] private TMP_Text[] _warningTexts;

        /// <summary> Название сохраненного файла для новой игры.</summary>
        /// <remarks> Название формируется путем соединения строк имени игрока и названия магазина.</remarks>
        private string NewGameSaveFileName => string.Concat(_newGameInputFields.Select(field => field.text));

        private void OnEnable()
        {
            PlayerController.SwitchController(false);
        }

        private void OnDisable()
        {
            PlayerController.SwitchController(true);
        }

        /// <summary> Начать новую игру.</summary>
        public void OnClickNewGame()
        {
            if (!ValidateInputFields()) return;
            PersistentObject.Instance.GetSavingWrapper().StartNewGame(NewGameSaveFileName);
        }

        /// <summary> Проверка полей ввода.</summary>
        /// <returns> Возвращает True, если поля ввода прошли все проверки.</returns>
        private bool ValidateInputFields()
        {
            for (int i = 0; i < _newGameInputFields.Length; i++)
            {
                if (!InputFieldValidator.IsValid(_newGameInputFields[i].text, out string warningMessage))
                {
                    // Показываем текст с предупреждением.
                    _warningTexts[i].text = warningMessage;
                    _warningTexts[i].gameObject.SetActive(true);
                    return false;
                }
                _warningTexts[i].gameObject.SetActive(false);
            }
            return true;
        }

        /// <summary> Отменить.</summary>
        public void OnClickCancel()
        {
            ClearInputFields();
            HideWarnings();
        }

        /// <summary> Очистить поля ввода.</summary>
        private void ClearInputFields()
        {
            foreach (var inputField in _newGameInputFields)
            {
                inputField.text = string.Empty;
            }
        }

        /// <summary> Скрыть предупреждения.</summary>
        private void HideWarnings()
        {
            foreach (var warningText in _warningTexts)
            {
                warningText.gameObject.SetActive(false);
            }
        }

        /// <summary> Продолжить игру.</summary>
        public void OnClickContinue() =>
            PersistentObject.Instance.GetSavingWrapper().ContinueGame();

        /// <summary> Вернуться в главное меню.</summary>
        public void OnClickReturnToMainMenu()
        {
            SavingWrapper.LoadSceneByName(SceneType.MainMenu.ToString());
        }

        /// <summary> Выйти из игры.</summary>
        public void OnClickQuit() => Application.Quit();
    }
}