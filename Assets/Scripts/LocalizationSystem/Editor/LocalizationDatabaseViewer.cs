#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public class LocalizationDatabaseViewer : EditorWindow
    {
        private LocalizationDatabase _database;
        private Dictionary<string, Dictionary<string, string>> _data;

        private Vector2 _scroll;
        private GUIStyle _missedTranslationStyle;
        private GUIStyle _usedStyle;
        private GUIStyle _unusedStyle;
        private GUIStyle _translationStyle;


        private List<string> _languages;
        private List<string> _groups;
        private int _selectedGroupIndex;

        private readonly HashSet<string> _sceneUsedKeys = new();

        private readonly Dictionary<string, LocalizedLabel> _keyToLabel = new();


        [MenuItem("Tools/Localization/Localization Table Viewer")]
        public static void ShowWindow()
        {
            var window = GetWindow<LocalizationDatabaseViewer>();
            window.titleContent = new GUIContent("Localization Viewer");
            window.Show();
        }

        private void OnEnable()
        {
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
        }

        private void RefreshData()
        {
            if (_database == null) return;

            _data = _database.ToDictionary();

            _languages = _data.Values
                .SelectMany(dict => dict.Keys)
                .Distinct()
                .ToList();

            _groups = _data.Keys
                .Select(k => k.Split('_')[0])
                .Distinct()
                .ToList();

            RefreshSceneUsage();
        }

        private void RefreshSceneUsage()
        {
            _sceneUsedKeys.Clear();
            _keyToLabel.Clear();

            var labels = FindObjectsByType<LocalizedLabel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var label in labels)
            {
                var so = new SerializedObject(label);
                var property = so.FindProperty("_localizationKey");
                if (property != null && !string.IsNullOrWhiteSpace(property.stringValue))
                {
                    string key = property.stringValue.Trim();
                    _sceneUsedKeys.Add(key);
                    _keyToLabel.TryAdd(key, label);
                }
            }
        }


        private void OnGUI()
        {
            EditorGUILayout.Space();
            _database = (LocalizationDatabase)EditorGUILayout.ObjectField("Localization Database:", _database,
                typeof(LocalizationDatabase), false);

            if (_database == null)
            {
                EditorGUILayout.HelpBox("Please assign a LocalizationDatabase asset.", MessageType.Info);
                return;
            }

            if (_data == null || GUILayout.Button("Refresh Data")) RefreshData();

            if (_groups == null || _groups.Count == 0)
            {
                EditorGUILayout.HelpBox("No data found in the selected database.", MessageType.Warning);
                return;
            }

            DrawGroupSelector();
            EditorGUILayout.Space(10);
            DrawTable();
        }

        private void DrawGroupSelector()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select Group:", GUILayout.Width(100));
            _selectedGroupIndex = EditorGUILayout.Popup(_selectedGroupIndex, _groups.ToArray());
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTable()
        {
            if (_groups.Count == 0 || _selectedGroupIndex >= _groups.Count) return;

            string selectedGroup = _groups[_selectedGroupIndex];

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", GUILayout.Width(250));
            EditorGUILayout.LabelField("Used", GUILayout.Width(50));
            foreach (string lang in _languages) EditorGUILayout.LabelField(lang, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            foreach ((string key, var translations) in _data)
            {
                if (!key.StartsWith(selectedGroup + "_")) continue;

                EditorGUILayout.BeginHorizontal();

                // Key
                EditorGUILayout.BeginVertical(GUILayout.Width(250));
                EditorGUILayout.SelectableLabel(key, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                // Кнопка Select, если объект с этим ключом найден
                if (_keyToLabel.TryGetValue(key, out var label))
                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        Selection.activeObject = label.gameObject;
                        EditorGUIUtility.PingObject(label.gameObject);
                    }

                EditorGUILayout.EndVertical();


                // Used in scene
                if (_sceneUsedKeys.Contains(key))
                    EditorGUILayout.LabelField("✓", _usedStyle, GUILayout.Width(50));
                else
                    EditorGUILayout.LabelField("—", _unusedStyle, GUILayout.Width(50));

                // Translations
                foreach (string lang in _languages)
                {
                    bool missed = !translations.ContainsKey(lang) || string.IsNullOrWhiteSpace(translations[lang]);
                    string translation = missed ? "[MISSED]" : translations[lang];
                    var style = missed ? _missedTranslationStyle : _translationStyle;

                    float height = style.CalcHeight(new GUIContent(translation), 200);
                    EditorGUILayout.TextArea(translation, style, GUILayout.Width(200),
                        GUILayout.Height(height));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif