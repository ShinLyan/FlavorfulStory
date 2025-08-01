#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlavorfulStory.LocalizationSystem
{
    public class UIPrefabLocalizationUsageFinder : ILocalizationUsageFinder, IDisposable
    {
        private const int BatchSizePerFrame = 5;

        private readonly Dictionary<string, List<string>> _keyToPrefabPaths = new(StringComparer.OrdinalIgnoreCase);
        private string[] _allPrefabGuids = Array.Empty<string>();
        private int _nextGuidIndex;
        private Action _onBuilt;

        public bool IsBuilding { get; private set; }

        public float Progress => _allPrefabGuids.Length == 0
            ? 0f
            : Mathf.Clamp01((float)_nextGuidIndex / _allPrefabGuids.Length);

        public void RebuildIndex(Action onBuilt = null)
        {
            CancelBuild();
            _keyToPrefabPaths.Clear();
            _allPrefabGuids = AssetDatabase.FindAssets("t:Prefab");
            _nextGuidIndex = 0;
            IsBuilding = true;
            _onBuilt = onBuilt;
            EditorApplication.update += UpdateBuild;
        }

        private void UpdateBuild()
        {
            if (!IsBuilding)
            {
                EditorApplication.update -= UpdateBuild;
                return;
            }

            int processed = 0;
            while (processed < BatchSizePerFrame && _nextGuidIndex < _allPrefabGuids.Length)
            {
                string guid = _allPrefabGuids[_nextGuidIndex++];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                {
                    processed++;
                    continue;
                }

                GameObject root = null;
                try
                {
                    root = PrefabUtility.LoadPrefabContents(path);
                    var labels = root.GetComponentsInChildren<LocalizedLabel>(true);
                    foreach (var label in labels)
                    {
                        var so = new SerializedObject(label);
                        var prop = so.FindProperty("_localizationKey");
                        if (prop == null) continue;
                        string key = prop.stringValue;
                        if (string.IsNullOrWhiteSpace(key)) continue;

                        if (!_keyToPrefabPaths.TryGetValue(key, out var list))
                        {
                            list = new List<string>();
                            _keyToPrefabPaths[key] = list;
                        }

                        if (!list.Contains(path)) list.Add(path);
                    }
                }
                finally
                {
                    if (root != null) PrefabUtility.UnloadPrefabContents(root);
                }

                processed++;
            }

            if (_nextGuidIndex >= _allPrefabGuids.Length)
            {
                IsBuilding = false;
                EditorApplication.update -= UpdateBuild;
                _onBuilt?.Invoke();
                _onBuilt = null;
            }
        }

        private void CancelBuild()
        {
            if (IsBuilding)
            {
                IsBuilding = false;
                EditorApplication.update -= UpdateBuild;
                _onBuilt = null;
            }
        }

        private static Object GetPrefabToSelect(GameObject obj)
        {
            if (obj == null) return null;

            // Если объект — префаб ассет
            if (PrefabUtility.IsPartOfPrefabAsset(obj)) return obj;

            // Попробуем получить префаб-ассет для объекта
            var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(obj);
            if (prefabAsset != null) return prefabAsset;

            // Иначе — вернуть сам объект
            return obj;
        }

        public List<Object> FindUsages(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return new List<Object>();
            if (_keyToPrefabPaths.TryGetValue(key, out var paths))
            {
                var usages = new List<Object>();
                foreach (string path in paths)
                {
                    var prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefabRoot == null) continue;

                    // Загружаем временно префаб, чтобы найти вложенные объекты с LocalizedLabel и ключом
                    var prefabContents = PrefabUtility.LoadPrefabContents(path);
                    if (prefabContents == null) continue;

                    try
                    {
                        var labels = prefabContents.GetComponentsInChildren<LocalizedLabel>(true);
                        foreach (var label in labels)
                        {
                            var so = new SerializedObject(label);
                            var prop = so.FindProperty("_localizationKey");
                            if (prop == null) continue;
                            if (!string.Equals(prop.stringValue, key, StringComparison.OrdinalIgnoreCase)) continue;

                            var prefabToSelect = GetPrefabToSelect(label.gameObject);
                            if (prefabToSelect != null && !usages.Contains(prefabToSelect)) usages.Add(prefabToSelect);
                        }
                    }
                    finally
                    {
                        PrefabUtility.UnloadPrefabContents(prefabContents);
                    }
                }

                return usages;
            }

            return new List<Object>();
        }

        public void Invalidate() => _keyToPrefabPaths.Clear();

        public void Dispose() => CancelBuild();
    }
}
#endif