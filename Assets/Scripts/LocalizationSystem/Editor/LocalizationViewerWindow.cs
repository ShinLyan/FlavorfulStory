using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem.Editor
{
#if UNITY_EDITOR
    public class LocalizationViewerWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private string _search = "";

        [MenuItem("Tools/Localization Viewer")]
        public static void ShowWindow()
        {
            var window = GetWindow<LocalizationViewerWindow>("Localization Viewer");
            window.minSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            if (!LocalizationService.IsInitialized)
                EditorGUILayout.HelpBox("LocalizationService is not initialized.", MessageType.Warning);

            var availableLanguages = LocalizationService.AvailableLanguages;
            var dictionary = LocalizationService.AllTranslations;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Search:", EditorStyles.boldLabel);
            _search = EditorGUILayout.TextField(_search);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Key", EditorStyles.boldLabel, GUILayout.Width(250));
            foreach (string lang in availableLanguages)
                EditorGUILayout.LabelField(lang.ToUpper(), EditorStyles.boldLabel, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var kvp in dictionary)
            {
                string key = kvp.Key;
                if (!string.IsNullOrEmpty(_search) && !key.ToLower().Contains(_search.ToLower())) continue;

                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(key, GUILayout.Width(250));

                foreach (string lang in availableLanguages)
                {
                    string value = LocalizationService.GetLocalizedValueOrNull(key, lang);

                    var style = new GUIStyle(EditorStyles.label);
                    if (string.IsNullOrWhiteSpace(value)) style.normal.textColor = Color.red;

                    EditorGUILayout.LabelField(string.IsNullOrWhiteSpace(value) ? "[MISSING]" : value, style,
                        GUILayout.Width(150));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
#endif
}