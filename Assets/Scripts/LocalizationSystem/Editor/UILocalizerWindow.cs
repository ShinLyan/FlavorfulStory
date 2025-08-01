using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public class UILocalizerWindow : EditorWindow
    {
        private LocalizationDatabase _database;
        private Vector2 _scrollPos;

        private Dictionary<string, Dictionary<string, string>> _entries = new();
        private List<string> _languages = new();
        private string _currentGroup = "";
        private List<string> _groups = new();

        private GUIStyle _missedTranslationStyle;
        private GUIStyle _translationStyle;
        private GUIStyle _usedStyle;
        private GUIStyle _unusedStyle;

        private LocalizationUsageFinderManager _usageFinderManager;
        private UIPrefabLocalizationUsageFinder _uiFinder;

        [MenuItem("Tools/Localization/UILocalizer")]
        public static void ShowWindow()
        {
            var window = GetWindow<UILocalizerWindow>("UILocalizer");
            window.minSize = new Vector2(600, 400);
        }

        private void OnEnable()
        {
            _uiFinder = new UIPrefabLocalizationUsageFinder();
            _usageFinderManager = new LocalizationUsageFinderManager(_uiFinder);
            _uiFinder.RebuildIndex();

            _missedTranslationStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                normal = { textColor = Color.red }
            };

            _translationStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                normal = { textColor = Color.white }
            };

            _usedStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.green },
                alignment = TextAnchor.MiddleCenter
            };

            _unusedStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.gray },
                alignment = TextAnchor.MiddleCenter
            };
        }

        private void OnGUI()
        {
            _database = (LocalizationDatabase)EditorGUILayout.ObjectField("Localization Database", _database,
                typeof(LocalizationDatabase), false);

            if (_database == null) return;

            if (GUILayout.Button("Reload")) LoadEntries();


            EditorGUILayout.BeginHorizontal();
            if (_uiFinder.IsBuilding)
            {
                EditorGUILayout.LabelField("Rebuilding UI index...", GUILayout.Width(150));
                var barRect = GUILayoutUtility.GetRect(200, 16);
                EditorGUI.ProgressBar(barRect, _uiFinder.Progress,
                    $"{Mathf.RoundToInt(_uiFinder.Progress * 100)}%");
            }
            else
            {
                if (GUILayout.Button("Rebuild UI Index", GUILayout.Width(150))) _uiFinder.RebuildIndex();
            }

            EditorGUILayout.EndHorizontal();

            if (_groups.Count > 0)
            {
                int selected = Mathf.Max(0, _groups.IndexOf(_currentGroup));
                selected = EditorGUILayout.Popup("Group", selected, _groups.ToArray());
                _currentGroup = _groups[selected];
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Key", GUILayout.Width(250));
            GUILayout.Label("Used", GUILayout.Width(40));
            foreach (string lang in _languages) GUILayout.Label(lang, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            foreach (var entry in _entries)
            {
                if (!_currentGroup.Equals(GetGroup(entry.Key))) continue;

                var translations = entry.Value;
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.SelectableLabel(entry.Key, GUILayout.Width(250));


                var usages = _usageFinderManager.FindUsages(_currentGroup, entry.Key);
                bool isUsed = usages.Count > 0;

                GUILayout.Label(isUsed ? "✔" : "✖", isUsed ? _usedStyle : _unusedStyle, GUILayout.Width(25));

                if (isUsed)
                {
                    if (GUILayout.Button("🔍", GUILayout.Width(30)))
                    {
                        Selection.objects = usages.ToArray();
                        EditorGUIUtility.PingObject(usages[0]);
                    }
                }
                else
                {
                    GUILayout.Space(34);
                }

                foreach (string lang in _languages)
                {
                    bool missed = !translations.ContainsKey(lang) || string.IsNullOrWhiteSpace(translations[lang]);
                    string translation = missed ? "[MISSED]" : translations[lang];
                    var style = missed ? _missedTranslationStyle : _translationStyle;

                    float height = style.CalcHeight(new GUIContent(translation), 200);
                    EditorGUILayout.TextArea(translation, style, GUILayout.Width(200), GUILayout.Height(height));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void LoadEntries()
        {
            _entries = _database.ToDictionary();
            _languages = _database.GetLanguages();
            _groups = _entries.Keys.Select(GetGroup).Distinct().OrderBy(g => g).ToList();
            if (_groups.Count > 0) _currentGroup = _groups[0];
        }

        private string GetGroup(string key)
        {
            int index = key.IndexOf('_');
            return index > 0 ? key.Substring(0, index) : "Unknown";
        }
    }
}