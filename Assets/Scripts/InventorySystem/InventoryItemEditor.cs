#if UNITY_EDITOR
//[CustomEditor(typeof(InventoryItem))]
//public class InventoryItemEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        EditorGUILayout.PropertyField(serializedObject.FindProperty("_itemID"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("_displayItemName"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("_icon"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isStackable"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("_description"));
//        if (serializedObject.FindProperty("_isStackable").boolValue)
//        {
//            EditorGUILayout.PropertyField(serializedObject.FindProperty("_count"));
//        }
//        serializedObject.ApplyModifiedProperties();
//    }
//}
#endif