using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        /// <summary> Ключ в PlayerPrefs, по которому хранится текущее имя файла сохранения. </summary>
        private const string CurrentSaveKey = "currentSaveName";

        /// <summary> Возвращает true, если существует активное имя сохранения и связанный с ним файл. </summary>
        public static bool SaveFileExists =>
            PlayerPrefs.HasKey(CurrentSaveKey) && SavingSystem.SaveFileExists(CurrentSaveFileName);

        /// <summary> Название текущего сохранения. </summary>
        private static string CurrentSaveFileName => PlayerPrefs.GetString(CurrentSaveKey);

        /// <summary> Внедрение зависимостей Zenject. </summary>
        /// <param name="canvasGroupFader"> Компонент затемнения экрана при переходах между сценами. </param>
        [Inject]
        private void Construct(CanvasGroupFader canvasGroupFader) => _canvasGroupFader = canvasGroupFader;

        /// <summary> Асинхронно начинает новую игру. </summary>
        /// <param name="saveFileName"> Название файла сохранения. </param>
        public async UniTask StartNewGameAsync(string saveFileName)
        {
            if (string.IsNullOrEmpty(saveFileName)) return;

            SetCurrentSaveFileName(saveFileName);

            await _canvasGroupFader.Show().AsyncWaitForCompletion();

            await SceneManager.LoadSceneAsync(nameof(SceneName.Game));

            // Ждём один кадр — Unity вызовет Start() всем объектам
            await UniTask.NextFrame();
            Save();

            _canvasGroupFader.Hide();
        }

        /// <summary> Продолжить игру. </summary>
        public void ContinueGame() => ContinueGameAsync().Forget();

        /// <summary> Асинхронно продолжает игру из последнего сохранения. </summary>
        private async UniTask ContinueGameAsync()
        {
            await _canvasGroupFader.Show().AsyncWaitForCompletion();

            await SavingSystem.LoadLastSceneAsync(CurrentSaveFileName);

            _canvasGroupFader.Hide();
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
    }
}