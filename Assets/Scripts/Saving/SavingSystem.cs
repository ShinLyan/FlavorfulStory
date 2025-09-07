using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace FlavorfulStory.Saving
{
    /// <summary> Система сохранений. </summary>
    /// <remarks> Сохраняет и сценовые <see cref="SaveableEntity"/>,
    /// и зарегистрированные Zenject-сервисы <see cref="ISaveableService"/>. </remarks>
    public static class SavingSystem
    {
        /// <summary> Расширение файлов сохранений. </summary>
        public const string SaveFileExtension = ".sav";

        /// <summary> Ключ для сохранения индекса последней сцены. </summary>
        private const string LastSceneKey = "lastSceneBuildIndex";

        /// <summary> Событие, вызываемое после завершения загрузки сцены и восстановления состояния. </summary>
        public static event Action OnLoadCompleted;

        #region Public Methods

        /// <summary> Асинхронная загрузка последней сцены и восстановление состояния. </summary>
        /// <param name="saveFile"> Название файла с сохранением. </param>
        public static async UniTask LoadLastSceneAsync(string saveFile)
        {
            var state = LoadFile(saveFile);
            if (state == null)
            {
                Debug.LogError("No save file loaded");
                return;
            }

            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.TryGetValue(LastSceneKey, out object value)) buildIndex = (int)value;

            await SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        /// <summary> Сохранение текущего состояния сцены и сервисов в заданном файле сохранения. </summary>
        /// <param name="saveFile"> Название файла, куда необходимо сохранить данные. </param>
        public static void Save(string saveFile)
        {
            var state = LoadFile(saveFile);
            CaptureState(state);
            SaveToFile(saveFile, state);
        }

        /// <summary> Загрузка состояния из файла и восстановление объектов. </summary>
        /// <param name="saveFile"> Название файла, откуда необходимо загружать данные. </param>
        public static void Load(string saveFile) => RestoreState(LoadFile(saveFile));

        /// <summary> Удаление файла сохранения. </summary>
        /// <param name="saveFile"> Название файла, который необходимо удалить. </param>
        public static void Delete(string saveFile) => File.Delete(GetPath(saveFile));

        /// <summary> Получение пути до сохраненного файла. </summary>
        /// <param name="saveFile"> Название файла сохранения. </param>
        /// <returns> Путь до сохраненного файла. </returns>
        public static string GetPath(string saveFile) =>
            Path.Combine(Application.persistentDataPath, saveFile + SaveFileExtension);

        /// <summary> Существует ли сохраненный файл?</summary>
        /// <param name="saveFile"> Название файла сохранения. </param>
        /// <returns> Возвращает True - если файл сохранения существует, False - в противном случае. </returns>
        public static bool SaveFileExists(string saveFile) => File.Exists(GetPath(saveFile));

        #endregion

        #region Private Methods

        /// <summary> Загрузка данных из файла. </summary>
        /// <param name="saveFile"> Название файла сохранения. </param>
        /// <returns> Возвращает словарь названия и объекта. </returns>
        private static Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPath(saveFile);
            if (!File.Exists(path)) return new Dictionary<string, object>();

            using var stream = File.Open(path, FileMode.Open);
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(stream) as Dictionary<string, object>;
        }

        /// <summary> Сохранение данных в файл. </summary>
        /// <param name="saveFile"> Название файла сохранения. </param>
        /// <param name="state"> Состояние, которое необходимо записать в файл. </param>
        private static void SaveToFile(string saveFile, object state)
        {
            string path = GetPath(saveFile);
            using var stream = File.Open(path, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, state);

            Debug.Log($"Saving to {path}");
        }

        /// <summary> Сохранение зарегистрированных объектов сцены и сервисов. </summary>
        /// <param name="state"> Словарь, содержащий состояния всех объектов, которые необходимо сохранить. </param>
        private static void CaptureState(Dictionary<string, object> state)
        {
            foreach ((string key, var saveable) in GetAllSaveables()) state[key] = saveable.CaptureState();

            state[LastSceneKey] = SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary> Возвращает все объекты, поддерживающие сохранение: сценовые и сервисные. </summary>
        /// <returns> Перечисление пар (ключ, объект ISaveable). </returns>
        private static IEnumerable<(string Key, ISaveable Saveable)> GetAllSaveables()
        {
            foreach (var saveableEntity in Object.FindObjectsByType<SaveableEntity>
                         (FindObjectsInactive.Include, FindObjectsSortMode.None))
                yield return (saveableEntity.UniqueIdentifier, saveableEntity);

            foreach (var saveableService in SaveServiceRegistry.All)
                yield return (saveableService.UniqueIdentifier, saveableService);
        }

        /// <summary> Восстановление состояний зарегистрированных объектов сцены и сервисов. </summary>
        /// <param name="state"> Словарь, содержащий состояния объектов, которые необходимо загрузить. </param>
        private static void RestoreState(Dictionary<string, object> state)
        {
            RestoreSceneObjects(state);
            RestoreServiceStatesAsync(state).Forget();
            OnLoadCompleted?.Invoke();
        }

        /// <summary> Восстанавливает состояния объектов сцены (SaveableEntity). </summary>
        /// <param name="state"> Словарь, содержащий состояния объектов, которые необходимо загрузить. </param>
        private static void RestoreSceneObjects(Dictionary<string, object> state)
        {
            foreach (var entity in Object.FindObjectsByType<SaveableEntity>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (state.TryGetValue(entity.UniqueIdentifier, out object value))
                    entity.RestoreState(value);
        }

        /// <summary> Восстанавливает состояния зарегистрированных сервисов (ISaveableService). </summary>
        /// <param name="state"> Словарь, содержащий состояния объектов, которые необходимо загрузить. </param>
        private static async UniTaskVoid RestoreServiceStatesAsync(Dictionary<string, object> state)
        {
            await UniTask.NextFrame();

            foreach (var service in SaveServiceRegistry.All)
                if (state.TryGetValue(service.UniqueIdentifier, out object value))
                    service.RestoreState(value);
        }

        #endregion
    }
}