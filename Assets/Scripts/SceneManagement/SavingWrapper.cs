using System.Collections;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Управляет сохранением, загрузкой и переключением сцен. </summary>
    public class SavingWrapper : MonoBehaviour
    {
        /// <summary> Компонент затемнения экрана при переходах между сценами. </summary>
        private Fader _fader;

        /// <summary> Название первой сцены, которая загружается при старте новой игры. </summary>
        private const SceneName FirstUploadedScene = SceneName.Game;

        /// <summary> Ключ в PlayerPrefs, по которому хранится текущее имя файла сохранения. </summary>
        private const string CurrentSaveKey = "currentSaveName";

        /// <summary> Возвращает true, если существует активное имя сохранения и связанный с ним файл. </summary>
        public static bool SaveFileExists => PlayerPrefs.HasKey(CurrentSaveKey) &&
                                             SavingSystem.SaveFileExists(GetCurrentSaveFileName());

        /// <summary> Внедряет зависимость компонента затемнения экрана. </summary>
        /// <param name="fader"> Компонент затемнения экрана при переходах между сценами. </param>
        [Inject]
        public void Construct(Fader fader)
        {
            _fader = fader;
        }

        /// <summary> Продолжает последнюю сохранённую игру. </summary>
        /// <remarks> Вызывается из главного меню. </remarks>
        public void ContinueGame()
        {
            if (!SaveFileExists) return;

            StartCoroutine(LoadLastScene());
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
        private static void SetCurrentSaveFileName(string saveFileName) =>
            PlayerPrefs.SetString(CurrentSaveKey, saveFileName);

        /// <summary> Получает название текущего сохранения. </summary>
        /// <returns> Название текущего сохранения. </returns>
        private static string GetCurrentSaveFileName() => PlayerPrefs.GetString(CurrentSaveKey);

        /// <summary> Загружает первую сцену игры. </summary>
        /// <returns> Корутина, выполняющая загрузку первой сцены. </returns>
        private IEnumerator LoadFirstScene()
        {
            yield return _fader.FadeOut(Fader.FadeOutTime);
            yield return SceneManager.LoadSceneAsync(FirstUploadedScene.ToString());
            Save();
            yield return _fader.FadeIn(Fader.FadeInTime);

            LocationChanger.ActivatePlayerCurrentLocation();
            InputWrapper.UnblockAllInput();
        }

        /// <summary> Загружает последнюю сохранённую сцену. </summary>
        /// <returns> Корутина, выполняющая загрузку последней сохранённой сцены. </returns>
        private IEnumerator LoadLastScene()
        {
            yield return _fader.FadeOut(Fader.FadeOutTime);
            yield return SavingSystem.LoadLastScene(GetCurrentSaveFileName());
            yield return _fader.FadeIn(Fader.FadeInTime);

            LocationChanger.ActivatePlayerCurrentLocation();
            InputWrapper.UnblockAllInput();
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
        private const KeyCode SaveKey = KeyCode.K;

        /// <summary> Клавиша для загрузки в режиме отладки. </summary>
        private const KeyCode LoadKey = KeyCode.L;

        /// <summary> Клавиша для удаления сохранения в режиме отладки. </summary>
        private const KeyCode DeleteKey = KeyCode.Delete;

        /// <summary> Обрабатывает ввод для сохранения, загрузки и удаления в режиме отладки. </summary>
        private void Update()
        {
            if (Input.GetKeyDown(SaveKey)) Save();
            if (Input.GetKeyDown(LoadKey)) Load();
            if (Input.GetKeyDown(DeleteKey)) Delete();
        }
#endif

        #endregion
    }
}