#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.Saving.Editor
{
    /// <summary> Утилита очистки всех сохранений и PlayerPrefs. </summary>
    public static class SaveCleanupUtility
    {
        /// <summary> Путь к пункту меню в редакторе Unity. </summary>
        private const string MenuPath = "FlavorfulStory/Saving/Clear All Saves and PlayerPrefs";

        /// <summary> Очищает все сохранения и сбрасывает PlayerPrefs. </summary>
        [MenuItem(MenuPath)]
        public static void ClearAllSavesAndPrefs()
        {
            if (!EditorUtility.DisplayDialog("Удалить все сохранения?",
                    $"Это удалит все сохранения из папки [{Application.persistentDataPath}] и сбросит PlayerPrefs.\nПродолжить?",
                    "Да", "Отмена"))
                return;

            int deletedFilesCount = DeleteAllSaveFiles();
            ClearPlayerPrefs();

            EditorUtility.DisplayDialog("Очистка завершена",
                $"Удалено сохранений: {deletedFilesCount}\nPlayerPrefs сброшены.", "OK");

            Debug.Log($"[SaveCleanupUtility] Удалено файлов сохранений: {deletedFilesCount}, PlayerPrefs очищены.");
        }

        /// <summary> Удаляет все сохранения из папки сохранений. </summary>
        /// <returns> Количество успешно удалённых файлов. </returns>
        private static int DeleteAllSaveFiles()
        {
            int deletedCount = 0;
            string path = Application.persistentDataPath;

            if (!Directory.Exists(path))
            {
                Debug.LogWarning($"Папка сохранений не найдена: {path}");
                return deletedCount;
            }

            string[] saveFiles = Directory.GetFiles(path, $"*{SavingSystem.SaveFileExtension}",
                SearchOption.TopDirectoryOnly);

            foreach (string file in saveFiles)
                try
                {
                    File.Delete(file);
                    deletedCount++;
                }
                catch (IOException ex)
                {
                    Debug.LogWarning($"Не удалось удалить файл: {file}\n{ex.Message}");
                }

            return deletedCount;
        }

        /// <summary> Удаляет все данные из PlayerPrefs и сохраняет изменения. </summary>
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}

#endif