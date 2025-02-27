using System.Collections;
using FlavorfulStory.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using FlavorfulStory.Saving;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Управляет сохранением, загрузкой и переключением сцен. </summary>
    public class SavingWrapper : MonoBehaviour
    {
        /// <summary> Первая сцена, загружаемая после главного меню. </summary>
        [SerializeField] private SceneType _firstUploadedScene;

        /// <summary> Ключ для хранения имени текущего сохранения в PlayerPrefs. </summary>
        private const string CurrentSaveKey = "currentSaveName";

        /// <summary> Флаг существующего файла сохранения. </summary>
        public static bool SaveFileExists => PlayerPrefs.HasKey(CurrentSaveKey) &&
                                             SavingSystem.SaveFileExists(GetCurrentSaveFileName());

        /// <summary> Продолжает последнюю сохранённую игру. </summary>
        /// <remarks> Вызывается из главного меню. </remarks>
        public void ContinueGame()
        {
            if (!SaveFileExists) return;

            StartCoroutine(LoadLastScene());
            InputWrapper.UnblockAllInput();
        }

        /// <summary> Начинает новую игру с указанным файлом сохранения. </summary>
        /// <param name="saveFileName"> Название файла сохранения. </param>
        public void StartNewGame(string saveFileName)
        {
            if (string.IsNullOrEmpty(saveFileName)) return;

            SetCurrentSaveFileName(saveFileName);
            StartCoroutine(LoadFirstScene());
        }

        /// <summary> Устанавливает текущее сохранение. </summary>
        /// <param name="saveFileName"> Название файла сохранения. </param>
        private static void SetCurrentSaveFileName(string saveFileName) => PlayerPrefs.SetString(CurrentSaveKey, saveFileName);

        /// <summary> Получает название текущего сохранения. </summary>
        /// <returns> Название текущего сохранения. </returns>
        private static string GetCurrentSaveFileName() => PlayerPrefs.GetString(CurrentSaveKey);

        /// <summary> Загружает первую сцену игры. </summary>
        /// <returns> Корутина, выполняющая загрузку первой сцены. </returns>
        private IEnumerator LoadFirstScene()
        {
            yield return PersistentObject.Instance.Fader.FadeOut(Fader.FadeOutTime);
            yield return SceneManager.LoadSceneAsync(_firstUploadedScene.ToString());
            yield return PersistentObject.Instance.Fader.FadeIn(Fader.FadeInTime);
        }

        /// <summary> Загружает последнюю сохранённую сцену. </summary>
        /// <returns> Корутина, выполняющая загрузку последней сохранённой сцены. </returns>
        private static IEnumerator LoadLastScene()
        {
            yield return PersistentObject.Instance.Fader.FadeOut(Fader.FadeOutTime);
            yield return SavingSystem.LoadLastScene(GetCurrentSaveFileName());
            yield return PersistentObject.Instance.Fader.FadeIn(Fader.FadeInTime);
        }

        /// <summary> Асинхронно загружает сцену по её названию. </summary>
        /// <param name="sceneName"> Название сцены. </param>
        /// <returns> Корутина, выполняющая загрузку сцены. </returns>
        public static IEnumerator LoadSceneAsyncByName(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        /// <summary> Загружает сцену по её названию. </summary>
        /// <param name="sceneName"> Название сцены. </param>
        public static void LoadSceneByName(string sceneName) => SceneManager.LoadScene(sceneName);

        /// <summary> Загружает данные игры из текущего сохранения. </summary>
        public static void Load() => SavingSystem.Load(GetCurrentSaveFileName());

        /// <summary> Сохраняет данные игры в текущий файл сохранения. </summary>
        public static void Save() => SavingSystem.Save(GetCurrentSaveFileName());

        /// <summary> Удаляет текущее сохранение. </summary>
        public static void Delete() => SavingSystem.Delete(GetCurrentSaveFileName());

        #region Debug

#if UNITY_EDITOR
        /// <summary> Клавиша для сохранения в режиме отладки. </summary>
        [SerializeField] private KeyCode _saveKey;

        /// <summary> Клавиша для загрузки в режиме отладки. </summary>
        [SerializeField] private KeyCode _loadKey;

        /// <summary> Клавиша для удаления сохранения в режиме отладки. </summary>
        [SerializeField] private KeyCode _deleteKey;

        /// <summary> Обрабатывает ввод для сохранения, загрузки и удаления в режиме отладки. </summary>
        private void Update()
        {
            if (Input.GetKeyDown(_saveKey)) Save();
            if (Input.GetKeyDown(_loadKey)) Load();
            if (Input.GetKeyDown(_deleteKey)) Delete();
        }
#endif

        #endregion
    }
}