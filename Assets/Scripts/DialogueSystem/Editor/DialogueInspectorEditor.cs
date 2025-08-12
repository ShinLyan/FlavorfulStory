using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.DialogueSystem.Conditions;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Editor
{
    /// <summary> Кастомный инспектор для диалогов. </summary>
    [CustomEditor(typeof(Dialogue))]
    public class DialogueInspectorEditor : UnityEditor.Editor
    {
        private SerializedProperty _conditionsProp;
        private Dictionary<string, Type> _conditionTypes;

        /// <summary> Инициализация при включении редактора. </summary>
        private void OnEnable()
        {
            _conditionsProp = serializedObject.FindProperty("Conditions");
            _conditionTypes = GetConditionTypes();
        }

        /// <summary> Получает все типы условий диалога. </summary>
        private Dictionary<string, Type> GetConditionTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(DialogueCondition).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .ToDictionary(t => t.Name, t => t);
        }

        /// <summary> Отрисовка GUI инспектора. </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultProperties();
            DrawConditionsSection();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Отрисовывает стандартные свойства. </summary>
        private void DrawDefaultProperties()
        {
            DrawPropertiesExcluding(serializedObject, "Conditions");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Conditions", EditorStyles.boldLabel);
        }

        /// <summary> Отрисовывает секцию условий. </summary>
        private void DrawConditionsSection()
        {
            DrawAddConditionButton();
            DrawConditionsList();
        }

        /// <summary> Отрисовывает кнопку добавления условия. </summary>
        private void DrawAddConditionButton()
        {
            if (GUILayout.Button("Add Condition")) ShowConditionMenu();
        }

        /// <summary> Показывает меню выбора условий. </summary>
        private void ShowConditionMenu()
        {
            var menu = new GenericMenu();
            foreach (var kv in _conditionTypes.OrderBy(k => k.Key))
                menu.AddItem(new GUIContent(kv.Key), false, () => AddCondition(kv.Value));
            menu.ShowAsContext();
        }

        /// <summary> Отрисовывает список условий. </summary>
        private void DrawConditionsList()
        {
            if (_conditionsProp == null) return;

            for (int i = 0; i < _conditionsProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical("box");
                DrawCondition(i);
                EditorGUILayout.EndVertical();
            }
        }

        /// <summary> Отрисовывает отдельное условие. </summary>
        /// <param name="index"> Индекс. </param>
        private void DrawCondition(int index)
        {
            var el = _conditionsProp.GetArrayElementAtIndex(index);
            EditorGUILayout.PropertyField(el, new GUIContent($"Condition {index}"), true);
            DrawRemoveButton(index);
        }

        /// <summary> Отрисовывает кнопку удаления условия. </summary>
        /// <param name="index"> Индекс. </param>
        private void DrawRemoveButton(int index)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove")) RemoveCondition(index);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary> Добавляет новое условие. </summary>
        /// <param name="type"> Тип. </param>
        private void AddCondition(Type type)
        {
            if (_conditionsProp == null) return;

            _conditionsProp.arraySize++;
            var element = _conditionsProp.GetArrayElementAtIndex(_conditionsProp.arraySize - 1);
            element.managedReferenceValue = Activator.CreateInstance(type);
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Удаляет условие по индексу. </summary>
        /// <param name="index"> Индекс условия. </param>
        private void RemoveCondition(int index)
        {
            var el = _conditionsProp.GetArrayElementAtIndex(index);
            el.managedReferenceValue = null;
            _conditionsProp.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }
    }
}