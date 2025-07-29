using System.Collections.Generic;
using System.IO;
using FlavorfulStory.LocalizationSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationDatabase))]
public class LocalizationDatabaseLoader : Editor
{
    private const char BaseDelimiter = ';';

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Импорт локализации", EditorStyles.boldLabel);

        if (GUILayout.Button("Импортировать из CSV (Resources/Localization/*.csv)")) ImportCsvFiles();
    }

    private void ImportCsvFiles()
    {
        var database = (LocalizationDatabase)target;

        string folderPath = Path.Combine(Application.dataPath, "Resources/Localization");
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("Папка Resources/Localization не найдена.");
            return;
        }

        string[] csvPaths = Directory.GetFiles(folderPath, "*.csv");
        var entryDict = new Dictionary<string, Dictionary<string, string>>();

        foreach (string path in csvPaths)
        {
            string[] lines = File.ReadAllLines(path);
            if (lines.Length == 0) continue;

            string[] header = lines[0].Split(BaseDelimiter);

            int langStartIndex = -1;
            for (int i = 1; i < header.Length; i++)
            {
                string val = header[i].Trim().ToUpperInvariant();
                if (val.Length == 2)
                {
                    langStartIndex = i;
                    break;
                }
            }

            if (langStartIndex == -1)
            {
                Debug.LogWarning($"Не удалось найти языковые колонки в {path}");
                continue;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split(BaseDelimiter);
                if (columns.Length <= langStartIndex) continue;

                string key = columns[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;

                if (!entryDict.ContainsKey(key)) entryDict[key] = new Dictionary<string, string>();

                for (int j = langStartIndex; j < header.Length && j < columns.Length; j++)
                {
                    string lang = header[j].Trim();
                    string value = columns[j].Trim();

                    if (!string.IsNullOrEmpty(lang) && !string.IsNullOrEmpty(value)) entryDict[key][lang] = value;
                }
            }
        }

        var newEntries = new List<LocalizationEntry>();

        foreach (var kvp in entryDict)
        {
            var entry = new LocalizationEntry { Key = kvp.Key };
            foreach (var pair in kvp.Value)
                entry.Translations.Add(new LocalizedValue
                {
                    LanguageCode = pair.Key,
                    Text = pair.Value
                });

            newEntries.Add(entry);
        }

        Undo.RecordObject(database, "Импорт локализации");
        database.SetEntries(newEntries);

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"Импорт завершён. Импортировано ключей: {newEntries.Count}");
    }
}