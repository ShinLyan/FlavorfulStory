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
        /// <summary> Сериализованное представление списка условий диалога. </summary>
        private SerializedProperty _conditionsProp;

        /// <summary> Словарь всех доступных типов условий диалога, отображаемых в меню добавления. </summary>
        private Dictionary<string, Type> _conditionTypes;

        /// <summary> Инициализация при включении редактора. </summary>
        private void OnEnable()
        {
            _conditionsProp = serializedObject.FindProperty("Conditions");
            _conditionTypes = GetConditionTypes();
        }

        /// <summary> Получает все типы условий диалога. </summary>
        /// <returns> Все типы условий диалога. </returns>
        private static Dictionary<string, Type> GetConditionTypes() => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(DialogueCondition).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass)
            .ToDictionary(type => type.Name, type => type);

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
            foreach (var pair in _conditionTypes.OrderBy(pair => pair.Key))
                menu.AddItem(new GUIContent(pair.Key), false, () => AddCondition(pair.Value));
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
            var property = _conditionsProp.GetArrayElementAtIndex(index);
            EditorGUILayout.PropertyField(property, new GUIContent($"Condition {index}"), true);
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
            var property = _conditionsProp.GetArrayElementAtIndex(_conditionsProp.arraySize - 1);
            property.managedReferenceValue = Activator.CreateInstance(type);
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Удаляет условие по индексу. </summary>
        /// <param name="index"> Индекс условия. </param>
        private void RemoveCondition(int index)
        {
            var property = _conditionsProp.GetArrayElementAtIndex(index);
            property.managedReferenceValue = null;
            _conditionsProp.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }
    }
}