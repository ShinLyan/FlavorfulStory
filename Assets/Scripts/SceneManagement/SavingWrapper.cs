using Cysharp.Threading.Tasks;
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
        /// <summary> Компонент затемнения экрана при переходах между сценами. </summary>
        private CanvasGroupFader _canvasGroupFader;

        /// <summary> Менеджер локаций. </summary>
        private LocationManager _locationManager;

        /// <summary> Название первой сцены, которая загружается при старте новой игры. </summary>
        private const SceneName FirstUploadedScene = SceneName.Game;

        /// <summary> Ключ в PlayerPrefs, по которому хранится текущее имя файла сохранения. </summary>
        private const string CurrentSaveKey = "currentSaveName";

        /// <summary> Возвращает true, если существует активное имя сохранения и связанный с ним файл. </summary>
        public static bool SaveFileExists => PlayerPrefs.HasKey(CurrentSaveKey) &&
                                             SavingSystem.SaveFileExists(CurrentSaveFileName);

        /// <summary> Название текущего сохранения. </summary>
        private static string CurrentSaveFileName => PlayerPrefs.GetString(CurrentSaveKey);

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="canvasGroupFader"> Компонент затемнения экрана при переходах между сценами. </param>
        /// <param name="locationManager"> Менеджер локаций. </param>
        [Inject]
        private void Construct(CanvasGroupFader canvasGroupFader, [InjectOptional] LocationManager locationManager)
        {
            _canvasGroupFader = canvasGroupFader;
            _locationManager = locationManager;
        }

        /// <summary> Асинхронно начинает новую игру. </summary>
        /// <param name="saveFileName"> Название файла сохранения. </param>
        public async UniTask StartNewGameAsync(string saveFileName)
        {
            if (string.IsNullOrEmpty(saveFileName)) return;

            SetCurrentSaveFileName(saveFileName);

            await _canvasGroupFader.Show().AsyncWaitForCompletion();

            await SceneManager.LoadSceneAsync(FirstUploadedScene.ToString());
            Save();

            _canvasGroupFader.Hide();
            _locationManager?.UpdateActiveLocation();
            InputWrapper.UnblockAllInput();
        }

        /// <summary> Асинхронно продолжает игру из последнего сохранения. </summary>
        public async UniTask ContinueGameAsync()
        {
            if (!SaveFileExists) return;

            await _canvasGroupFader.Show().AsyncWaitForCompletion();

            await SavingSystem.LoadLastScene(CurrentSaveFileName);

            _canvasGroupFader.Hide();
            _locationManager?.UpdateActiveLocation();
            InputWrapper.UnblockAllInput();
        }

        /// <summary> Устанавливает текущее сохранение. </summary>
        private static void SetCurrentSaveFileName(string saveFileName) =>
            PlayerPrefs.SetString(CurrentSaveKey, saveFileName);

        /// <summary> Асинхронно загружает сцену по её названию. </summary>
        /// <param name="sceneName"> Название сцены. </param>
        public static async UniTask LoadSceneAsyncByName(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }

        /// <summary> Загружает данные игры из текущего сохранения. </summary>
        public static void Load() => SavingSystem.Load(CurrentSaveFileName);

        /// <summary> Сохраняет данные игры в текущий файл сохранения. </summary>
        public static void Save() => SavingSystem.Save(CurrentSaveFileName);

        /// <summary> Удаляет текущее сохранение. </summary>
        public static void Delete() => SavingSystem.Delete(CurrentSaveFileName);

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