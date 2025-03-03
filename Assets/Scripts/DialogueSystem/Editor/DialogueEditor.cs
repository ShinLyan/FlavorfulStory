using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private void OnGUI()
        {
            Debug.Log("ddd");
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is Dialogue dialogue)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
    }
}