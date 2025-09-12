using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        /// <summary> Сериализованное свойство для контекстных диалогов. </summary>
        private SerializedProperty _contextDialoguesProperty;

        /// <summary> Список контекстных диалогов с поддержкой drag-and-drop и сортировки. </summary>
        private ReorderableList _dialogueList;

        /// <summary> Словарь списков условий для каждого диалога. </summary>
        private readonly Dictionary<int, ReorderableList> _conditionLists = new();

        /// <summary> Словарь состояния foldout'ов для каждого диалога. </summary>
        private readonly Dictionary<int, bool> _dialogueFoldouts = new();

        /// <summary> Список доступных типов условий, найденных в сборках. </summary>
        private List<Type> _conditionTypes;

        /// <summary> Фиксированный порядок типов условий. </summary>
        private static readonly List<Type> ConditionTypeOrder = new()
        {
            typeof(WeatherDialogueCondition),
            typeof(DayOfWeekDialogueCondition),
            typeof(TimeOfDayDialogueCondition)
        };

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

        /// <summary> Рисует заголовок списка контекстных диалогов. </summary>
        /// <param name="rect"> Область для отрисовки. </param>
        private static void DrawDialogueListHeader(Rect rect) => EditorGUI.LabelField(rect, "Context Dialogues");

        /// <summary> Рисует элемент списка контекстных диалогов. </summary>
        /// <param name="rect"> Область для отрисовки. </param>
        /// <param name="index"> Индекс элемента. </param>
        /// <param name="isActive"> Активен ли элемент. </param>
        /// <param name="isFocused"> В фокусе ли элемент. </param>
        private void DrawDialogueElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _contextDialoguesProperty.GetArrayElementAtIndex(index);
            var dialoguesField = element.FindPropertyRelative("<Dialogues>k__BackingField");
            var conditionsField = element.FindPropertyRelative("<Conditions>k__BackingField");

            float y = rect.y;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float line = EditorGUIUtility.singleLineHeight;

            _dialogueFoldouts.TryAdd(index, true);

            EditorGUI.indentLevel++;

            _dialogueFoldouts[index] = EditorGUI.Foldout(
                new Rect(rect.x, y, rect.width, line),
                _dialogueFoldouts[index],
                GetDialogueTitle(conditionsField),
                true
            );

            y += line + spacing;
            if (!_dialogueFoldouts[index])
            {
                EditorGUI.indentLevel--;
                return;
            }

            if (!_conditionLists.ContainsKey(index))
                _conditionLists[index] = CreateConditionsReorderableList(conditionsField);

            var conditionList = _conditionLists[index];
            conditionList.DoList(new Rect(rect.x, y, rect.width, conditionList.GetHeight()));
            y += conditionList.GetHeight();

            EditorGUI.PropertyField(
                new Rect(rect.x, y, rect.width, EditorGUI.GetPropertyHeight(dialoguesField, true)),
                dialoguesField,
                new GUIContent("Dialogues"),
                true
            );

            EditorGUI.indentLevel--;
        }

        /// <summary> Генерирует заголовок для foldout-а, отображая список сокращённых названий условий. </summary>
        /// <param name="conditionsProperty"> Список условий. </param>
        /// <returns> Строка с названиями типов условий. </returns>
        private static string GetDialogueTitle(SerializedProperty conditionsProperty)
        {
            if (conditionsProperty == null || conditionsProperty.arraySize == 0) return "No Conditions";

            var typeNameList = new List<(int order, string displayName)>();

            for (int i = 0; i < conditionsProperty.arraySize; i++)
            {
                var prop = conditionsProperty.GetArrayElementAtIndex(i);
                object value = prop.managedReferenceValue;

                if (value == null)
                {
                    typeNameList.Add((int.MaxValue, "Missing"));
                    continue;
                }

                var type = value.GetType();
                int index = ConditionTypeOrder.IndexOf(type);
                if (index < 0) index = int.MaxValue;

                string readable = GetReadableConditionName(type);
                typeNameList.Add((index, readable));
            }

            var sortedNames = typeNameList.OrderBy(x => x.order).Select(x => x.displayName);
            return string.Join(", ", sortedNames);
        }

        /// <summary> Преобразует имя типа условия в читаемый вид.</summary>
        /// <param name="type"> Тип условия. </param>
        /// <returns> Первая буква — заглавная, остальные — строчные, все слова раздельно. </returns>
        private static string GetReadableConditionName(Type type)
        {
            string name = type.Name.Replace("DialogueCondition", "");
            string withSpaces = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
            return char.ToUpper(withSpaces[0]) + withSpaces[1..].ToLower();
        }

        /// <summary> Вычисляет высоту элемента списка диалога. </summary>
        /// <param name="index"> Индекс элемента. </param>
        /// <returns> Высота элемента в пикселях. </returns>
        private float CalculateDialogueElementHeight(int index)
        {
            float height = 2f;
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (_dialogueFoldouts.TryGetValue(index, out bool expanded) && expanded)
            {
                var element = _contextDialoguesProperty.GetArrayElementAtIndex(index);
                var dialoguesField = element.FindPropertyRelative("<Dialogues>k__BackingField");

                if (_conditionLists.TryGetValue(index, out var list)) height += list.GetHeight();

                height += EditorGUI.GetPropertyHeight(dialoguesField, true);
            }

            height += 2f;
            return height;
        }

        /// <summary> Добавляет новую запись контекстного диалога в список. </summary>
        /// <param name="list"> Список контекстных диалогов. </param>
        private void AddNewDialogueEntry(ReorderableList list)
        {
            serializedObject.Update();

            _contextDialoguesProperty.arraySize++;
            var newElement = _contextDialoguesProperty.GetArrayElementAtIndex(_contextDialoguesProperty.arraySize - 1);

            var dialogues = newElement.FindPropertyRelative("<Dialogues>k__BackingField");
            dialogues.ClearArray();
            dialogues.arraySize = 2;
            dialogues.GetArrayElementAtIndex(0).objectReferenceValue = null;
            dialogues.GetArrayElementAtIndex(1).objectReferenceValue = null;

            newElement.FindPropertyRelative("<Conditions>k__BackingField").ClearArray();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Создает ReorderableList для отображения условий диалога. </summary>
        /// <param name="conditionsProperty"> Сериализованное свойство списка условий. </param>
        /// <returns> Список для редактирования условий. </returns>
        private ReorderableList CreateConditionsReorderableList(SerializedProperty conditionsProperty) => new(
            conditionsProperty.serializedObject, conditionsProperty, false, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Conditions"),
            drawElementCallback = (rect, index, _, _) =>
            {
                var sorted = GetSortedConditions(conditionsProperty);
                if (index < sorted.Count) DrawConditionFields(rect, sorted[index]);
            },
            elementHeightCallback = index =>
            {
                var sorted = GetSortedConditions(conditionsProperty);
                return index < sorted.Count
                    ? CalculateConditionHeight(sorted[index])
                    : EditorGUIUtility.singleLineHeight;
            },
            onAddDropdownCallback = (_, _) => ShowConditionTypeMenu(conditionsProperty)
        };

        /// <summary> Возвращает список условий, отсортированных по фиксированному порядку. </summary>
        /// <param name="conditionsProperty"> Сериализованное свойство списка условий. </param>
        /// <returns> Список условий, отсортированных по фиксированному порядку. </returns>
        private static List<SerializedProperty> GetSortedConditions(SerializedProperty conditionsProperty)
        {
            var list = new List<(int order, SerializedProperty prop)>();

            for (int i = 0; i < conditionsProperty.arraySize; i++)
            {
                var prop = conditionsProperty.GetArrayElementAtIndex(i);
                object obj = prop.managedReferenceValue;
                if (obj == null)
                {
                    list.Add((int.MaxValue, prop));
                    continue;
                }

                var type = obj.GetType();
                int index = ConditionTypeOrder.IndexOf(type);
                if (index < 0) index = int.MaxValue;

                list.Add((index, prop));
            }

            return list.OrderBy(x => x.order).Select(x => x.prop).ToList();
        }

        /// <summary> Рисует поля одного условия. </summary>
        /// <param name="rect"> Область для отрисовки. </param>
        /// <param name="conditionProperty"> Сериализованное свойство условия. </param>
        private static void DrawConditionFields(Rect rect, SerializedProperty conditionProperty)
        {
            float y = rect.y;
            var property = conditionProperty.Copy();
            var end = property.GetEndProperty();
            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, end))
            {
                float height = EditorGUI.GetPropertyHeight(property, true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), property, true);
                y += height;
                property.NextVisible(false);
            }
        }

        /// <summary> Вычисляет высоту одного условия. </summary>
        /// <param name="conditionProperty"> Сериализованное свойство условия. </param>
        /// <returns> Высота условия в пикселях. </returns>
        private static float CalculateConditionHeight(SerializedProperty conditionProperty)
        {
            if (conditionProperty.managedReferenceValue == null) return EditorGUIUtility.singleLineHeight;

            float height = 0f;
            var property = conditionProperty.Copy();
            var end = property.GetEndProperty();
            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, end))
            {
                height += EditorGUI.GetPropertyHeight(property, true);
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