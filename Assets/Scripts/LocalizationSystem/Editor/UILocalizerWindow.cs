using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    /// <summary> Окно редактора для работы с локализацией и управления переводами. </summary>
    public class UILocalizerWindow : EditorWindow
    {
        /// <summary> База данных локализации. </summary>
        private LocalizationDatabase _database;

        /// <summary> Позиция скролла в окне. </summary>
        private Vector2 _scrollPos;

        /// <summary> Словарь всех переводов с группировкой по ключам. </summary>
        private Dictionary<string, Dictionary<string, string>> _entries = new();

        /// <summary> Список доступных языков. </summary>
        private List<string> _languages = new();

        /// <summary> Текущая выбранная группа ключей. </summary>
        private string _currentGroup = "";

        /// <summary> Список всех групп ключей. </summary>
        private List<string> _groups = new();

        /// <summary> Стиль для пропущенных переводов. </summary>
        private GUIStyle _missedTranslationStyle;

        /// <summary> Стиль для обычных переводов. </summary>
        private GUIStyle _translationStyle;

        /// <summary> Стиль для используемых ключей. </summary>
        private GUIStyle _usedStyle;

        /// <summary> Стиль для неиспользуемых ключей. </summary>
        private GUIStyle _unusedStyle;

        /// <summary> Менеджер поиска использования ключей. </summary>
        private LocalizationUsageFinderManager _usageFinderManager;

        /// <summary> Открывает окно UILocalizer. </summary>
        [MenuItem("Tools/Localization/UILocalizer")]
        public static void ShowWindow()
        {
            var window = GetWindow<UILocalizerWindow>("UILocalizer");
            window.minSize = new Vector2(600, 400);
        }

        /// <summary> Инициализирует окно редактора. </summary>
        private void OnEnable()
        {
            _usageFinderManager = new LocalizationUsageFinderManager();

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

        /// <summary> Отрисовывает интерфейс окна. </summary>
        private void OnGUI()
        {
            _database = (LocalizationDatabase)EditorGUILayout.ObjectField("Localization Database", _database,
                typeof(LocalizationDatabase), false);

            if (_database == null) return;

            if (GUILayout.Button("Reload")) LoadEntries();

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

        /// <summary> Загружает данные из базы локализации. </summary>
        private void LoadEntries()
        {
            _entries = _database.ToDictionary();
            _languages = _database.GetLanguages();
            _groups = _entries.Keys.Select(GetGroup).Distinct().OrderBy(g => g).ToList();
            if (_groups.Count > 0) _currentGroup = _groups[0];
        }

        /// <summary> Получает группу из ключа локализации. </summary>
        /// <param name="key"> Ключ локализации. </param>
        /// <returns> Название группы или "Unknown", если группа не определена. </returns>
        private string GetGroup(string key)
        {
            int index = key.IndexOf('_');
            return index > 0 ? key[..index] : "Unknown";
        }
    }
}