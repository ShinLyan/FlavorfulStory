#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public static class LocalizedLabelEditorUtility
    {
        [MenuItem("Tools/Localization/Add LocalizedLabel To All TMP_Text")]
        public static void AddLocalizedLabelToAllTMPTexts()
        {
            var texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            int addedCount = 0;

            foreach (var text in texts)
                if (text.GetComponent<LocalizedLabel>() == null)
                {
                    Undo.AddComponent<LocalizedLabel>(text.gameObject);
                    addedCount++;
                }

            Debug.Log($"LocalizedLabel added to {addedCount} TMP_Text objects.");
        }

        [MenuItem("Tools/Localization/Remove Empty LocalizedLabels")]
        public static void RemoveEmptyLocalizedLabels()
        {
            var labels =
                Object.FindObjectsByType<LocalizedLabel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            int removedCount = 0;

            foreach (var label in labels)
            {
                var so = new SerializedObject(label);
                var keyProp = so.FindProperty("_localizationKey");

                string key = keyProp != null ? keyProp.stringValue : null;

                if (string.IsNullOrWhiteSpace(key))
                {
                    Undo.DestroyObjectImmediate(label);
                    removedCount++;
                }
            }

            Debug.Log($"Removed {removedCount} empty LocalizedLabel components.");
        }
    }
}
#endif