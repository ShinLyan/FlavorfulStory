﻿using System.Collections;
using DG.Tweening;
using FlavorfulStory.InputSystem;
using FlavorfulStory.Saving;
using FlavorfulStory.UI.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace FlavorfulStory.SceneManagement
{
    /// <summary> Управляет сохранением, загрузкой и переключением сцен. </summary>
    public class SavingWrapper : MonoBehaviour
    {
        /// <summary> Название первой сцены, которая загружается при старте новой игры. </summary>
        private const SceneName FirstUploadedScene = SceneName.Game;

        /// <summary> Ключ в PlayerPrefs, по которому хранится текущее имя файла сохранения. </summary>
        private const string CurrentSaveKey = "currentSaveName";

        /// <summary> Возвращает true, если существует активное имя сохранения и связанный с ним файл. </summary>
        public static bool SaveFileExists => PlayerPrefs.HasKey(CurrentSaveKey) &&
                                             SavingSystem.SaveFileExists(GetCurrentSaveFileName());

        /// <summary> Компонент затемнения экрана при переходах между сценами. </summary>
        private CanvasGroupFader _canvasGroupFader;

        /// <summary> Менеджер локаций. </summary>
        private LocationManager _locationManager;

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="canvasGroupFader"> Компонент затемнения экрана при переходах между сценами. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        [Inject]
        private void Construct(CanvasGroupFader canvasGroupFader, [InjectOptional] LocationManager locationManager)
        {
            _canvasGroupFader = canvasGroupFader;
            _locationManager = locationManager;
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
        private IEnumerator LoadFirstScene() // TODO ПЕРЕПИСАТЬ НА UniTask
        {
            var fadeTween = _canvasGroupFader.Show();
            var sceneLoad = SceneManager.LoadSceneAsync(FirstUploadedScene.ToString());

            yield return sceneLoad;
            Save();

            yield return fadeTween.WaitForCompletion();

            _canvasGroupFader.Hide();
            _locationManager?.UpdateActiveLocation();
            InputWrapper.UnblockAllInput();
        }

        /// <summary> Загружает последнюю сохранённую сцену. </summary>
        /// <returns> Корутина, выполняющая загрузку последней сохранённой сцены. </returns>
        private IEnumerator LoadLastScene()
        {
            var fadeTween = _canvasGroupFader.Show(); // затемнение
            var loadScene = SavingSystem.LoadLastScene(GetCurrentSaveFileName());

            yield return loadScene;
            yield return fadeTween.WaitForCompletion(); // дождаться окончания фейда

            _canvasGroupFader.Hide(); // убрать затемнение

            _locationManager?.UpdateActiveLocation();
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
        public static void LoadSceneByName(string sceneName) =>
            SceneManager.LoadScene(sceneName); // TODO: ПЕРЕПИСАТЬ НА АСИНХРОННУЮ ВЕРСИЮ

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