using System;
using System.Collections.Generic;
using FlavorfulStory.QuestSystem.Objectives;
using FlavorfulStory.QuestSystem.TriggerActions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.Editor
{
    /// <summary> Кастомный инспектор для редактирования квестов с этапами и целями. </summary>
    [CustomEditor(typeof(Quest))]
    public class QuestEditor : UnityEditor.Editor
    {
        /// <summary> Сериализованное свойство списка этапов. </summary>
        private SerializedProperty _stagesProperty;

        /// <summary> Сериализованное свойство списка наград. </summary>
        private SerializedProperty _rewardsProperty;

        /// <summary> Кешированные списки целей по каждому этапу. </summary>
        private readonly Dictionary<int, ReorderableList> _objectivesLists = new();

        /// <summary> Кешированные списки действий по завершению этапа. </summary>
        private readonly Dictionary<int, ReorderableList> _actionsLists = new();

        /// <summary> Состояния свёрнутости целей по индексу этапа и цели. </summary>
        private readonly Dictionary<(int, int), bool> _objectiveFoldouts = new();

        /// <summary> Состояния свёрнутости действий по индексу этапа и действия. </summary>
        private readonly Dictionary<(int stageIndex, int actionIndex), bool> _actionFoldouts = new();

        /// <summary> Текущие выбранные типы действий для каждого этапа. </summary>
        private readonly Dictionary<int, List<TriggerActionType>> _selectedTriggerTypes = new();

        /// <summary> ReorderableList для этапов квеста. </summary>
        private ReorderableList _stagesList;

        /// <summary> Инициализация ссылок и списков при открытии инспектора. </summary>
        private void OnEnable()
        {
            _stagesProperty = serializedObject.FindProperty("_stages");
            _rewardsProperty = serializedObject.FindProperty("_rewards");
            SetupStagesList();
        }

        /// <summary> Основной метод отрисовки кастомного инспектора. </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawSerializedField("<QuestGiver>k__BackingField");
            DrawSerializedField("<QuestName>k__BackingField");
            DrawSerializedField("<QuestDescription>k__BackingField");
            DrawSerializedField("<QuestType>k__BackingField");

            EditorGUILayout.Space(10);
            _stagesList.DoLayoutList();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.PropertyField(_rewardsProperty, true);

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Отрисовывает сериализованное поле по имени. </summary>
        /// <param name="propertyName"> Имя сериализованного свойства. </param>
        private void DrawSerializedField(string propertyName)
        {
            var prop = serializedObject.FindProperty(propertyName);
            if (prop != null) EditorGUILayout.PropertyField(prop);
        }

        /// <summary> Создаёт ReorderableList для этапов квеста. </summary>
        private void SetupStagesList()
        {
            _stagesList = new ReorderableList(serializedObject, _stagesProperty, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Quest Stages"),

                drawElementCallback = (rect, index, active, focused) =>
                {
                    var stageProp = _stagesProperty.GetArrayElementAtIndex(index);
                    var objectivesProp = stageProp.FindPropertyRelative("_objectives");
                    var actionsProp = stageProp.FindPropertyRelative("_onStageCompleteActions");

                    if (!_objectivesLists.ContainsKey(index))
                        _objectivesLists[index] = SetupObjectivesList(objectivesProp, index);

                    if (!_actionsLists.ContainsKey(index)) _actionsLists[index] = SetupActionsList(actionsProp, index);

                    float y = rect.y;
                    EditorGUI.HelpBox(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                        $"Stage {index + 1}", MessageType.None);
                    y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    _objectivesLists[index]
                        .DoList(new Rect(rect.x, y, rect.width, _objectivesLists[index].GetHeight()));
                    y += _objectivesLists[index].GetHeight() + EditorGUIUtility.standardVerticalSpacing;

                    _actionsLists[index].DoList(new Rect(rect.x, y, rect.width, _actionsLists[index].GetHeight()));
                },

                elementHeightCallback = index =>
                {
                    if (!_objectivesLists.ContainsKey(index))
                    {
                        var stageProp = _stagesProperty.GetArrayElementAtIndex(index);
                        var objectivesProp = stageProp.FindPropertyRelative("_objectives");
                        _objectivesLists[index] = SetupObjectivesList(objectivesProp, index);
                    }

                    if (!_actionsLists.ContainsKey(index))
                    {
                        var stageProp = _stagesProperty.GetArrayElementAtIndex(index);
                        var actionsProp = stageProp.FindPropertyRelative("_onStageCompleteActions");
                        _actionsLists[index] = SetupActionsList(actionsProp, index);
                    }

                    return EditorGUIUtility.singleLineHeight +
                           _objectivesLists[index].GetHeight() +
                           _actionsLists[index].GetHeight() +
                           EditorGUIUtility.standardVerticalSpacing * 3;
                }
            };
        }

        /// <summary> Создаёт ReorderableList для списка целей. </summary>
        /// <param name="objectivesProp"> Сериализованное свойство целей. </param>
        /// <param name="stageIndex"> Индекс текущего этапа. </param>
        /// <returns> Сконфигурированный ReorderableList. </returns>
        private ReorderableList SetupObjectivesList(SerializedProperty objectivesProp, int stageIndex) => new(
            objectivesProp.serializedObject, objectivesProp, true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Objectives"),
            drawElementCallback = (rect, index, active, focused) =>
                DrawObjectiveElement(rect, objectivesProp, stageIndex, index),
            elementHeightCallback = index =>
                GetObjectiveHeight(objectivesProp, stageIndex, index),
            onAddCallback = l =>
            {
                objectivesProp.InsertArrayElementAtIndex(objectivesProp.arraySize);
                var newObj = objectivesProp.GetArrayElementAtIndex(objectivesProp.arraySize - 1);
                InitializeNewObjective(newObj);
            }
        };

        /// <summary> Создаёт ReorderableList для списка действий. </summary>
        /// <param name="actionsProp"> Сериализованное свойство действий. </param>
        /// <param name="stageIndex"> Индекс этапа. </param>
        /// <returns> Сконфигурированный ReorderableList. </returns>
        private ReorderableList SetupActionsList(SerializedProperty actionsProp, int stageIndex)
        {
            var list = new ReorderableList(actionsProp.serializedObject, actionsProp, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "On Stage Complete Actions"),

                drawElementCallback = (rect, index, active, focused) =>
                {
                    if (index >= actionsProp.arraySize) return;
                    var element = actionsProp.GetArrayElementAtIndex(index);
                    var key = (stageIndex, index);

                    if (!_selectedTriggerTypes.ContainsKey(stageIndex))
                        _selectedTriggerTypes[stageIndex] = new List<TriggerActionType>();

                    while (_selectedTriggerTypes[stageIndex].Count <= index)
                        _selectedTriggerTypes[stageIndex].Add(TriggerActionType.GiveQuest);

                    if (!_actionFoldouts.ContainsKey(key)) _actionFoldouts[key] = true;

                    var foldoutRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    _actionFoldouts[key] =
                        EditorGUI.Foldout(foldoutRect, _actionFoldouts[key], $"Action {index + 1}", true);

                    if (!_actionFoldouts[key]) return;

                    float y = rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    // Draw Trigger Type dropdown
                    var selected = _selectedTriggerTypes[stageIndex][index];
                    var newSelected = (TriggerActionType)EditorGUI.EnumPopup(
                        new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                        "Trigger Type", selected);

                    if (newSelected != selected)
                    {
                        _selectedTriggerTypes[stageIndex][index] = newSelected;
                        if (QuestRegistry.TriggerActionMap.TryGetValue(newSelected, out var type))
                            element.managedReferenceValue = Activator.CreateInstance(type);
                    }

                    y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    if (element.managedReferenceValue != null)
                    {
                        var copy = element.Copy();
                        var end = copy.GetEndProperty();
                        copy.NextVisible(true);

                        while (!SerializedProperty.EqualContents(copy, end))
                        {
                            float h = EditorGUI.GetPropertyHeight(copy, true);
                            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, h), copy, true);
                            y += h + EditorGUIUtility.standardVerticalSpacing;
                            copy.NextVisible(false);
                        }
                    }
                },

                elementHeightCallback = index =>
                {
                    var element = actionsProp.GetArrayElementAtIndex(index);
                    var key = (stageIndex, index);

                    if (!_actionFoldouts.TryGetValue(key, out bool expanded) || !expanded)
                        return EditorGUIUtility.singleLineHeight;

                    float height = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2;

                    if (element.managedReferenceValue != null)
                    {
                        var copy = element.Copy();
                        var end = copy.GetEndProperty();
                        copy.NextVisible(true);

                        while (!SerializedProperty.EqualContents(copy, end))
                        {
                            height += EditorGUI.GetPropertyHeight(copy, true) +
                                      EditorGUIUtility.standardVerticalSpacing;
                            copy.NextVisible(false);
                        }
                    }

                    return height;
                },

                onAddCallback = _ =>
                {
                    int i = actionsProp.arraySize;
                    actionsProp.InsertArrayElementAtIndex(i);
                    actionsProp.GetArrayElementAtIndex(i).managedReferenceValue =
                        Activator.CreateInstance(typeof(GiveQuestAction));

                    if (!_selectedTriggerTypes.ContainsKey(stageIndex))
                        _selectedTriggerTypes[stageIndex] = new List<TriggerActionType>();
                    _selectedTriggerTypes[stageIndex].Add(TriggerActionType.GiveQuest);
                }
            };

            return list;
        }

        /// <summary> Отрисовывает одну цель в списке. </summary>
        /// <param name="rect"> Прямоугольник области отрисовки. </param>
        /// <param name="objectivesProp"> Сериализованное свойство списка целей. </param>
        /// <param name="stageIndex"> Индекс текущего этапа. </param>
        /// <param name="index"> Индекс цели в списке. </param>
        private void DrawObjectiveElement(Rect rect, SerializedProperty objectivesProp, int stageIndex, int index)
        {
            var objectiveProp = objectivesProp.GetArrayElementAtIndex(index);
            var (desc, type, @params) = GetObjectiveProperties(objectiveProp);

            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float line = EditorGUIUtility.singleLineHeight;
            var foldoutKey = (stageIndex, index);

            _objectiveFoldouts.TryAdd(foldoutKey, true);
            _objectiveFoldouts[foldoutKey] = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, line),
                _objectiveFoldouts[foldoutKey], $"Objective {index + 1}", true);
            if (!_objectiveFoldouts[foldoutKey]) return;

            rect.y += line + spacing;

            float descHeight = EditorGUI.GetPropertyHeight(desc, true);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, descHeight), desc, true);
            rect.y += descHeight + spacing;

            float typeHeight = EditorGUI.GetPropertyHeight(type, true);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, typeHeight), type, true);
            if (EditorGUI.EndChangeCheck()) UpdateObjectiveParams(type, @params);
            rect.y += typeHeight + spacing;

            if (@params?.managedReferenceValue != null)
                DrawParamsFields(new Rect(rect.x, rect.y, rect.width, 1000), @params);
        }

        /// <summary> Возвращает сериализованные свойства цели. </summary>
        /// <param name="objectiveProp"> Сериализованное свойство цели. </param>
        /// <returns> Кортеж с описанием, типом и параметрами. </returns>
        private static (SerializedProperty desc, SerializedProperty type, SerializedProperty @params)
            GetObjectiveProperties(SerializedProperty objectiveProp)
        {
            var desc = objectiveProp.FindPropertyRelative("<Description>k__BackingField");
            var type = objectiveProp.FindPropertyRelative("<Type>k__BackingField");
            var @params = objectiveProp.FindPropertyRelative("<Params>k__BackingField");
            return (desc, type, @params);
        }

        /// <summary> Инициализирует новую цель. </summary>
        /// <param name="objectiveProp"> Сериализованное свойство новой цели. </param>
        private void InitializeNewObjective(SerializedProperty objectiveProp)
        {
            var referenceProp = objectiveProp.FindPropertyRelative("<Reference>k__BackingField");
            var (desc, type, @params) = GetObjectiveProperties(objectiveProp);
            referenceProp.stringValue = Guid.NewGuid().ToString();
            desc.stringValue = string.Empty;
            type.enumValueIndex = 0;
            UpdateObjectiveParams(type, @params);
        }

        /// <summary> Обновляет параметры цели при смене её типа. </summary>
        /// <param name="typeProp"> Свойство типа цели. </param>
        /// <param name="paramsProp"> Свойство параметров цели. </param>
        private void UpdateObjectiveParams(SerializedProperty typeProp, SerializedProperty paramsProp)
        {
            var selectedType = (ObjectiveType)typeProp.enumValueIndex;
            paramsProp.managedReferenceValue =
                QuestRegistry.ObjectiveParamsMap.TryGetValue(selectedType, out var paramType)
                    ? Activator.CreateInstance(paramType)
                    : null;

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Отрисовывает все вложенные поля параметров. </summary>
        /// <param name="rect"> Область отрисовки. </param>
        /// <param name="paramsProp"> Сериализованное свойство параметров. </param>
        private static void DrawParamsFields(Rect rect, SerializedProperty paramsProp)
        {
            if (!paramsProp.hasVisibleChildren) return;

            var current = paramsProp.Copy();
            var end = current.GetEndProperty();
            float y = rect.y;
            bool enterChildren = true;

            while (current.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(current, end)) break;
                enterChildren = false;

                float height = EditorGUI.GetPropertyHeight(current, true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), current, true);
                y += height + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        /// <summary> Возвращает высоту области отрисовки цели. </summary>
        /// <param name="objectivesProp"> Список целей. </param>
        /// <param name="stageIndex"> Индекс этапа. </param>
        /// <param name="index"> Индекс цели. </param>
        /// <returns> Высота области. </returns>
        private float GetObjectiveHeight(SerializedProperty objectivesProp, int stageIndex, int index)
        {
            var key = (stageIndex, index);
            if (!_objectiveFoldouts.TryGetValue(key, out bool expanded) || !expanded)
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var objectiveProp = objectivesProp.GetArrayElementAtIndex(index);
            var (desc, type, @params) = GetObjectiveProperties(objectiveProp);

            float height = EditorGUIUtility.singleLineHeight +
                           EditorGUI.GetPropertyHeight(desc, true) +
                           EditorGUI.GetPropertyHeight(type, true) +
                           EditorGUIUtility.standardVerticalSpacing * 4;

            if (@params?.managedReferenceValue != null) height += CalculateParamsHeight(@params);

            return height;
        }

        /// <summary> Вычисляет высоту всех сериализованных параметров. </summary>
        /// <param name="paramsProp"> Сериализованное свойство параметров. </param>
        /// <returns> Общая высота отображения. </returns>
        private static float CalculateParamsHeight(SerializedProperty paramsProp)
        {
            var current = paramsProp.Copy();
            var end = current.GetEndProperty();
            float totalHeight = 0f;
            bool enterChildren = true;

            while (current.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(current, end)) break;
                enterChildren = false;
                totalHeight += EditorGUI.GetPropertyHeight(current, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }
    }
}