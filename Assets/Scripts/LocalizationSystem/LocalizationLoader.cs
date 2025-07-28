using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public static class LocalizationLoader
    {
        public static Dictionary<string, LocalizationData> LoadFromResources(string folderPath)
        {
            var result = new Dictionary<string, LocalizationData>();

            var csvFiles = Resources.LoadAll<TextAsset>(folderPath);
            if (csvFiles.Length == 0)
            {
                Debug.LogError($"No CSV files found in Resources/{folderPath}");
                return result;
            }

            foreach (var csv in csvFiles) MergeCSV(csv.text, result);

            return result;
        }

        private static void MergeCSV(string csvText, Dictionary<string, LocalizationData> data)
        {
            string[] lines = csvText.Split('\n');
            if (lines.Length < 2) return;

            string[] header = lines[0].Trim().Split(';');
            var langIndices = new Dictionary<string, int>();
            int keyIndex = -1;

            for (int i = 0; i < header.Length; i++)
            {
                string col = header[i].Trim();
                if (col.Equals("Key", StringComparison.OrdinalIgnoreCase))
                    keyIndex = i;
                else if (col.Length == 2 || col.Length == 3) // EN, RU, FR, etc.
                    langIndices[col] = i;
            }

            if (keyIndex == -1 || langIndices.Count == 0)
            {
                Debug.LogWarning("CSV missing 'Key' or language columns.");
                return;
            }

            foreach (string lang in langIndices.Keys)
                if (!data.ContainsKey(lang))
                    data[lang] = new LocalizationData();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(';');
                if (values.Length <= keyIndex) continue;

                string key = values[keyIndex];
                foreach ((string lang, int index) in langIndices)
                    if (index < values.Length)
                        data[lang].Translations[key] = values[index];
            }
        }
    }
}