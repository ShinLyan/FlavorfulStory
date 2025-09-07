using System;
using System.Collections.Generic;
using System.Linq;
using FlavorfulStory.DialogueSystem.Conditions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Editor
{
    /// <summary> Кастомный редактор для DialogueConfig, отображает приветственный и условные диалоги. </summary>
    [CustomEditor(typeof(DialogueConfig))]
    public class DialogueConfigEditor : UnityEditor.Editor
    {
        /// <summary> Сериализованное свойство для приветственного диалога. </summary>
        private SerializedProperty _greetingDialogueProperty;

        /// <summary> Сериализованное свойство для условных диалогов. </summary>
        private SerializedProperty _contextDialoguesProperty;

        /// <summary> Список условных диалогов с поддержкой drag-and-drop и сортировки. </summary>
        private ReorderableList _dialogueList;

        /// <summary> Словарь списков условий для каждого диалога. </summary>
        private readonly Dictionary<int, ReorderableList> _conditionLists = new();

        /// <summary> Словарь состояния foldout'ов для каждого диалога. </summary>
        private readonly Dictionary<int, bool> _dialogueFoldouts = new();

        /// <summary> Список доступных типов условий, найденных в сборках. </summary>
        private List<Type> _conditionTypes;

        /// <summary> Вызывается при включении инспектора, инициализирует свойства и списки. </summary>
        private void OnEnable()
        {
            _greetingDialogueProperty = serializedObject.FindProperty("<GreetingDialogue>k__BackingField");
            _contextDialoguesProperty = serializedObject.FindProperty("<ContextDialogues>k__BackingField");

            _conditionTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && typeof(DialogueCondition).IsAssignableFrom(type)).ToList();

            _dialogueList = new ReorderableList(serializedObject, _contextDialoguesProperty, true, true, true, true)
            {
                drawHeaderCallback = DrawDialogueListHeader,
                drawElementCallback = DrawDialogueElement,
                elementHeightCallback = CalculateDialogueElementHeight,
                onAddCallback = AddNewDialogueEntry
            };
        }

        /// <summary> Рисует пользовательский интерфейс инспектора. </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_greetingDialogueProperty);
            EditorGUILayout.Space(10);
            _dialogueList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Рисует заголовок списка условных диалогов. </summary>
        /// <param name="rect"> Область для отрисовки. </param>
        private static void DrawDialogueListHeader(Rect rect) => EditorGUI.LabelField(rect, "Context Dialogues");

        /// <summary> Рисует элемент списка условных диалогов. </summary>
        /// <param name="rect"> Область для отрисовки. </param>
        /// <param name="index"> Индекс элемента. </param>
        /// <param name="isActive"> Активен ли элемент. </param>
        /// <param name="isFocused"> В фокусе ли элемент. </param>
        private void DrawDialogueElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var dialogueElement = _contextDialoguesProperty.GetArrayElementAtIndex(index);
            var dialogueField = dialogueElement.FindPropertyRelative("<Dialogue>k__BackingField");
            var conditionsField = dialogueElement.FindPropertyRelative("<Conditions>k__BackingField");

            float y = rect.y;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float line = EditorGUIUtility.singleLineHeight;

            _dialogueFoldouts.TryAdd(index, true);
            _dialogueFoldouts[index] = EditorGUI.Foldout(new Rect(rect.x, y, rect.width, line),
                _dialogueFoldouts[index], $"Context Dialogue {index + 1}", true);

            y += line + spacing;
            if (!_dialogueFoldouts[index]) return;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, line), dialogueField);
            y += line + spacing;

            if (!_conditionLists.ContainsKey(index))
                _conditionLists[index] = CreateConditionsReorderableList(conditionsField);

            _conditionLists[index].DoList(new Rect(rect.x, y, rect.width, _conditionLists[index].GetHeight()));
        }

        /// <summary> Вычисляет высоту элемента списка диалога. </summary>
        /// <param name="index"> Индекс элемента. </param>
        /// <returns> Высота элемента в пикселях. </returns>
        private float CalculateDialogueElementHeight(int index)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (_dialogueFoldouts.TryGetValue(index, out bool expanded) && expanded)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (_conditionLists.TryGetValue(index, out var list)) height += list.GetHeight();
            }

            return height;
        }

        /// <summary> Добавляет новую запись условного диалога в список. </summary>
        /// <param name="list"> Список условных диалогов. </param>
        private void AddNewDialogueEntry(ReorderableList list)
        {
            _contextDialoguesProperty.arraySize++;
            var newElement =
                _contextDialoguesProperty.GetArrayElementAtIndex(_contextDialoguesProperty.arraySize - 1);
            newElement.FindPropertyRelative("<Dialogue>k__BackingField").objectReferenceValue = null;
            newElement.FindPropertyRelative("<Conditions>k__BackingField").ClearArray();
        }

        /// <summary> Создает ReorderableList для отображения условий диалога. </summary>
        /// <param name="conditionsProperty"> Сериализованное свойство списка условий. </param>
        /// <returns> Список для редактирования условий. </returns>
        private ReorderableList CreateConditionsReorderableList(SerializedProperty conditionsProperty) => new(
            conditionsProperty.serializedObject, conditionsProperty, true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Conditions"),

            drawElementCallback = (rect, index, _, _) =>
            {
                var condition = conditionsProperty.GetArrayElementAtIndex(index);
                DrawConditionFields(rect, condition);
            },

            elementHeightCallback = index =>
            {
                var condition = conditionsProperty.GetArrayElementAtIndex(index);
                return CalculateConditionHeight(condition);
            },

            onAddDropdownCallback = (_, _) => ShowConditionTypeMenu(conditionsProperty)
        };

        /// <summary> Рисует поля одного условия. </summary>
        /// <param name="rect"> Область для отрисовки. </param>
        /// <param name="conditionProperty"> Сериализованное свойство условия. </param>
        private static void DrawConditionFields(Rect rect, SerializedProperty conditionProperty)
        {
            float y = rect.y;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            string typeName = conditionProperty.managedReferenceValue?.GetType().Name ?? "Missing";
            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), typeName);
            y += EditorGUIUtility.singleLineHeight + spacing;

            if (conditionProperty.managedReferenceValue == null) return;

            var property = conditionProperty.Copy();
            var end = property.GetEndProperty();
            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, end))
            {
                float height = EditorGUI.GetPropertyHeight(property, true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), property, true);
                y += height + spacing;
                property.NextVisible(false);
            }
        }

        /// <summary> Вычисляет высоту одного условия. </summary>
        /// <param name="conditionProperty"> Сериализованное свойство условия. </param>
        /// <returns> Высота условия в пикселях. </returns>
        private static float CalculateConditionHeight(SerializedProperty conditionProperty)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (conditionProperty.managedReferenceValue == null) return height;

            var property = conditionProperty.Copy();
            var end = property.GetEndProperty();
            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, end))
            {
                height += EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.standardVerticalSpacing;
                property.NextVisible(false);
            }

            return height;
        }

        /// <summary> Показывает контекстное меню для выбора типа условия и добавления его в список. </summary>
        /// <param name="conditionsProperty"> Сериализованное свойство списка условий. </param>
        private void ShowConditionTypeMenu(SerializedProperty conditionsProperty)
        {
            var existingTypes = new HashSet<Type>();
            for (int i = 0; i < conditionsProperty.arraySize; i++)
            {
                object condition = conditionsProperty.GetArrayElementAtIndex(i).managedReferenceValue;
                if (condition != null) existingTypes.Add(condition.GetType());
            }

            var menu = new GenericMenu();
            foreach (var type in _conditionTypes)
                if (existingTypes.Contains(type))
                    menu.AddDisabledItem(new GUIContent(type.Name));
                else
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        object instance = Activator.CreateInstance(type);
                        conditionsProperty.arraySize++;
                        var newCondition = conditionsProperty.GetArrayElementAtIndex(conditionsProperty.arraySize - 1);
                        newCondition.managedReferenceValue = instance;
                        serializedObject.ApplyModifiedProperties();
                    });

            menu.ShowAsContext();
        }
    }
}