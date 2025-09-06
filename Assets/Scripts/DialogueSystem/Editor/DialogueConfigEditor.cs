// Assets/Editor/DialogueConfigEditor.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FlavorfulStory.DialogueSystem.Conditions;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Editor
{
    /// <summary> Кастомный редактор для конфигурации диалогов. </summary>
    [CustomEditor(typeof(DialogueConfig))]
    public class DialogueConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _greetingProp;
        private SerializedProperty _conditionalProp;

        private static Type[] _conditionTypes;
        private static string[] _conditionTypeNames;

        // UI state:
        private readonly List<bool> _dialogueFoldouts = new();
        private readonly List<List<bool>> _conditionFoldouts = new();
        private readonly List<int> _addConditionSelectedType = new();

        /// <summary> Инициализация редактора. </summary>
        private void OnEnable()
        {
            _greetingProp = serializedObject.FindProperty("<GreetingDialogues>k__BackingField");
            _conditionalProp = serializedObject.FindProperty("<ConditionalDialogues>k__BackingField");

            if (_conditionTypes == null)
            {
                _conditionTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => typeof(DialogueCondition).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToArray();

                _conditionTypeNames = _conditionTypes.Select(t => t.Name).ToArray();
            }

            EnsureUIStateSize();
        }

        /// <summary> Отрисовка GUI инспектора. </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EnsureNoSharedConditionInstances();

            EditorGUILayout.LabelField("Greeting Dialogues", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_greetingProp, true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Conditional Dialogues", EditorStyles.boldLabel);

            EnsureUIStateSize();

            for (int i = 0; i < _conditionalProp.arraySize; i++) DrawConditionalDialogue(i);

            DrawAddConditionalDialogueButton();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Отрисовывает условный диалог. </summary>
        /// <param name="index"> Индекс диалога </param>
        private void DrawConditionalDialogue(int index)
        {
            var dialogueItem = _conditionalProp.GetArrayElementAtIndex(index);
            var dialogueField = dialogueItem.FindPropertyRelative("Dialogue");
            var conditionsList = dialogueItem.FindPropertyRelative("Conditions");

            _dialogueFoldouts[index] = EditorGUILayout.Foldout(_dialogueFoldouts[index],
                $"Conditional Dialogue {index} : {(dialogueField.objectReferenceValue ? dialogueField.objectReferenceValue.name : "None")}",
                true);

            if (_dialogueFoldouts[index])
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(dialogueField);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Conditions", EditorStyles.boldLabel);

                EnsureConditionFoldoutsSize(index, conditionsList);
                DrawConditions(index, conditionsList);
                DrawAddConditionControls(index, conditionsList);

                EditorGUI.indentLevel--;
            }

            DrawRemoveDialogueButton(index);
            EditorGUILayout.Space();
        }

        /// <summary> Отрисовывает кнопку добавления диалога. </summary>
        private void DrawAddConditionalDialogueButton()
        {
            if (GUILayout.Button("Add Conditional Dialogue"))
            {
                _conditionalProp.arraySize++;
                var newDialogueProp = _conditionalProp.GetArrayElementAtIndex(_conditionalProp.arraySize - 1);
                newDialogueProp.FindPropertyRelative("Dialogue").objectReferenceValue = null;
                newDialogueProp.FindPropertyRelative("Conditions").ClearArray();

                serializedObject.ApplyModifiedProperties();
                _dialogueFoldouts.Add(true);
                _conditionFoldouts.Add(new List<bool>());
                _addConditionSelectedType.Add(0);
            }
        }

        /// <summary> Обеспечивает корректный размер списков состояний UI. </summary>
        private void EnsureUIStateSize()
        {
            while (_dialogueFoldouts.Count < _conditionalProp.arraySize) _dialogueFoldouts.Add(false);
            while (_conditionFoldouts.Count < _conditionalProp.arraySize) _conditionFoldouts.Add(new List<bool>());
            while (_addConditionSelectedType.Count < _conditionalProp.arraySize) _addConditionSelectedType.Add(0);
        }

        /// <summary> Обеспечивает корректный размер списка условий. </summary>
        private void EnsureConditionFoldoutsSize(int index, SerializedProperty conditionsList)
        {
            while (_conditionFoldouts[index].Count < conditionsList.arraySize) _conditionFoldouts[index].Add(false);
            while (_addConditionSelectedType.Count < _conditionalProp.arraySize) _addConditionSelectedType.Add(0);
        }

        /// <summary> Отрисовывает условия диалога. </summary>
        private void DrawConditions(int index, SerializedProperty conditionsList)
        {
            for (int j = 0; j < conditionsList.arraySize; j++) DrawCondition(index, conditionsList, j);
        }

        /// <summary> Отрисовывает отдельное условие. </summary>
        private void DrawCondition(int index, SerializedProperty conditionsList, int conditionIndex)
        {
            var condProp = conditionsList.GetArrayElementAtIndex(conditionIndex);
            object condObj = condProp.managedReferenceValue;
            string condName = condObj != null ? condObj.GetType().Name : "Null";

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();

            _conditionFoldouts[index][conditionIndex] = EditorGUILayout.Foldout(
                _conditionFoldouts[index][conditionIndex],
                $"Condition {conditionIndex}: {condName}", true);

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                RemoveConditionAt(conditionsList, conditionIndex);
                _conditionFoldouts[index].RemoveAt(conditionIndex);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.EndHorizontal();

            if (_conditionFoldouts[index][conditionIndex])
            {
                EditorGUI.indentLevel++;
                DrawConditionTypeSelector(condProp, condObj);
                DrawConditionFields(condProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary> Отрисовывает селектор типа условия. </summary>
        private void DrawConditionTypeSelector(SerializedProperty condProp, object condObj)
        {
            int currentIndex = 0;
            if (condObj != null)
            {
                int idx = Array.FindIndex(_conditionTypes, t => t == condObj.GetType());
                currentIndex = idx >= 0 ? idx : 0;
            }

            int newIndex = EditorGUILayout.Popup("Type", currentIndex, _conditionTypeNames);
            if (newIndex != currentIndex)
            {
                var newInstance = Activator.CreateInstance(_conditionTypes[newIndex]);
                condProp.managedReferenceValue = newInstance;
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary> Отрисовывает поля условия. </summary>
        private void DrawConditionFields(SerializedProperty condProp)
        {
            if (condProp.managedReferenceValue != null)
            {
                var iter = condProp.Copy();
                var end = iter.GetEndProperty();
                bool enter = iter.NextVisible(true);

                while (enter && !SerializedProperty.EqualContents(iter, end))
                {
                    EditorGUILayout.PropertyField(iter, true);
                    enter = iter.NextVisible(false);
                }
            }
        }

        /// <summary> Отрисовывает контролы добавления условия. </summary>
        private void DrawAddConditionControls(int index, SerializedProperty conditionsList)
        {
            EditorGUILayout.BeginHorizontal();
            _addConditionSelectedType[index] =
                EditorGUILayout.Popup(_addConditionSelectedType[index], _conditionTypeNames);

            if (GUILayout.Button("Add Condition", GUILayout.Width(120)))
            {
                AddNewCondition(conditionsList, _addConditionSelectedType[index]);
                _conditionFoldouts[index].Add(true);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary> Отрисовывает кнопку удаления диалога. </summary>
        private void DrawRemoveDialogueButton(int index)
        {
            if (GUILayout.Button($"Remove Conditional Dialogue {index}"))
            {
                _conditionalProp.DeleteArrayElementAtIndex(index);
                _dialogueFoldouts.RemoveAt(index);
                _conditionFoldouts.RemoveAt(index);
                if (_addConditionSelectedType.Count > index) _addConditionSelectedType.RemoveAt(index);
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary> Добавляет новое условие. </summary>
        private static void AddNewCondition(SerializedProperty conditionsList, int typeIndex)
        {
            if (typeIndex < 0 || typeIndex >= _conditionTypes.Length) typeIndex = 0;

            int insertIndex = conditionsList.arraySize;
            conditionsList.InsertArrayElementAtIndex(insertIndex);
            var element = conditionsList.GetArrayElementAtIndex(insertIndex);
            element.managedReferenceValue = Activator.CreateInstance(_conditionTypes[typeIndex]);
            conditionsList.serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Удаляет условие. </summary>
        private static void RemoveConditionAt(SerializedProperty conditionsList, int index)
        {
            if (index < 0 || index >= conditionsList.arraySize) return;
            conditionsList.DeleteArrayElementAtIndex(index);
            conditionsList.serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Проверяет отсутствие shared instances условий. </summary>
        private void EnsureNoSharedConditionInstances()
        {
            var seen = new Dictionary<object, SerializedProperty>(ReferenceEqualityComparer.Instance);
            bool changed = false;

            for (int i = 0; i < _conditionalProp.arraySize; i++)
            {
                var dialogueItem = _conditionalProp.GetArrayElementAtIndex(i);
                var conditionsList = dialogueItem.FindPropertyRelative("Conditions");

                for (int j = 0; j < conditionsList.arraySize; j++)
                {
                    var elem = conditionsList.GetArrayElementAtIndex(j);
                    var obj = elem.managedReferenceValue;
                    if (obj == null) continue;

                    if (seen.TryGetValue(obj, out var firstProp))
                    {
                        var clone = CloneManagedObject(obj);
                        elem.managedReferenceValue = clone;
                        changed = true;
                    }
                    else
                    {
                        seen[obj] = elem;
                    }
                }
            }

            if (changed) serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Клонирует managed объект. </summary>
        private static object CloneManagedObject(object original)
        {
            if (original == null) return null;
            var type = original.GetType();
            object clone = null;

            try
            {
                clone = Activator.CreateInstance(type);
                var json = JsonUtility.ToJson(original);
                JsonUtility.FromJsonOverwrite(json, clone);
                return clone;
            }
            catch
            {
                try
                {
                    clone = Activator.CreateInstance(type);
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var f in fields)
                    {
                        var val = f.GetValue(original);
                        f.SetValue(clone, val);
                    }

                    return clone;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to clone DialogueCondition of type {type.Name}: {ex}");
                    return null;
                }
            }
        }

        /// <summary> Сравнитель ссылочной эквивалентности. </summary>
        private class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new();
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}