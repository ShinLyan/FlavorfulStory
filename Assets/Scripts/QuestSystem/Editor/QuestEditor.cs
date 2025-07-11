using System;
using System.Collections.Generic;
using FlavorfulStory.QuestSystem.Objectives;
using FlavorfulStory.QuestSystem.Objectives.Params;
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

        /// <summary> ReorderableList для этапов квеста. </summary>
        private ReorderableList _stagesList;

        /// <summary> Кешированные списки целей по каждому этапу. </summary>
        private readonly Dictionary<int, ReorderableList> _objectivesLists = new();

        /// <summary> Состояния свёрнутости целей по индексу этапа и цели. </summary>
        private readonly Dictionary<(int, int), bool> _objectiveFoldouts = new();

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

        /// <summary> Создаёт ReorderableList для этапов и настраивает отрисовку. </summary>
        private void SetupStagesList()
        {
            _stagesList = new ReorderableList(serializedObject, _stagesProperty, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Quest Stages"),

                drawElementCallback = (rect, index, active, focused) =>
                {
                    if (index >= _stagesProperty.arraySize) return;

                    var stageProp = _stagesProperty.GetArrayElementAtIndex(index);
                    var objectivesProp = stageProp.FindPropertyRelative("_objectives");

                    if (!_objectivesLists.ContainsKey(index))
                        _objectivesLists[index] = SetupObjectivesList(objectivesProp, index);

                    var stageLabelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.HelpBox(stageLabelRect, $"Stage {index + 1}", MessageType.None);

                    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    if (!_objectivesLists.TryGetValue(index, out var list)) return;

                    float height = list.GetHeight();
                    var listRect = new Rect(rect.x, rect.y, rect.width, height);
                    list.DoList(listRect);
                },

                elementHeightCallback = index =>
                {
                    if (_objectivesLists.TryGetValue(index, out var list))
                        return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                               list.GetHeight();

                    var stageProp = _stagesProperty.GetArrayElementAtIndex(index);
                    var objectivesProp = stageProp.FindPropertyRelative("_objectives");
                    _objectivesLists[index] = SetupObjectivesList(objectivesProp, index);

                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                           _objectivesLists[index].GetHeight();
                },

                onAddCallback = list =>
                {
                    _stagesProperty.InsertArrayElementAtIndex(_stagesProperty.arraySize);
                    serializedObject.ApplyModifiedProperties();
                }
            };
        }

        /// <summary> Создаёт ReorderableList для целей одного этапа. </summary>
        /// <param name="objectivesProp"> Сериализованное свойство списка целей. </param>
        /// <param name="stageIndex"> Индекс этапа. </param>
        /// <returns> Настроенный ReorderableList. </returns>
        private ReorderableList SetupObjectivesList(SerializedProperty objectivesProp, int stageIndex)
        {
            var list = new ReorderableList(objectivesProp.serializedObject, objectivesProp, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Objectives"),

                drawElementCallback = (rect, index, active, focused) =>
                {
                    var objectiveProp = objectivesProp.GetArrayElementAtIndex(index);
                    var (desc, type, @params) = GetObjectiveProperties(objectiveProp);

                    float spacing = EditorGUIUtility.standardVerticalSpacing;
                    float line = EditorGUIUtility.singleLineHeight;
                    var foldoutKey = (stageIndex, index);

                    _objectiveFoldouts.TryAdd(foldoutKey, true);
                    _objectiveFoldouts[foldoutKey] = EditorGUI.Foldout(
                        new Rect(rect.x, rect.y, rect.width, line),
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
                },

                elementHeightCallback = index =>
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
                },

                onAddCallback = list =>
                {
                    objectivesProp.InsertArrayElementAtIndex(objectivesProp.arraySize);
                    var newObj = objectivesProp.GetArrayElementAtIndex(objectivesProp.arraySize - 1);
                    InitializeNewObjective(newObj);
                }
            };

            return list;
        }

        /// <summary> Отрисовывает сериализованное поле по имени. </summary>
        /// <param name="propertyName"> Имя сериализованного поля. </param>
        private void DrawSerializedField(string propertyName)
        {
            var prop = serializedObject.FindProperty(propertyName);
            if (prop != null) EditorGUILayout.PropertyField(prop);
        }

        /// <summary> Получает свойства описания, типа и параметров из цели. </summary>
        /// <param name="objectiveProp"> Сериализованное свойство цели. </param>
        /// <returns> Кортеж с Description, Type и Params. </returns>
        private static (SerializedProperty desc, SerializedProperty type, SerializedProperty @params)
            GetObjectiveProperties(SerializedProperty objectiveProp)
        {
            var desc = objectiveProp.FindPropertyRelative("<Description>k__BackingField");
            var type = objectiveProp.FindPropertyRelative("<Type>k__BackingField");
            var @params = objectiveProp.FindPropertyRelative("<Params>k__BackingField");
            return (desc, type, @params);
        }

        /// <summary> Инициализирует новую цель при добавлении в список. </summary>
        /// <param name="objectiveProp"> Сериализованное свойство цели. </param>
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
        /// <param name="typeProp"> Сериализованное свойство типа. </param>
        /// <param name="paramsProp"> Сериализованное свойство параметров. </param>
        private void UpdateObjectiveParams(SerializedProperty typeProp, SerializedProperty paramsProp)
        {
            var selectedType = (ObjectiveType)typeProp.enumValueIndex;
            paramsProp.managedReferenceValue =
                ObjectiveParamsRegistry.Mapping.TryGetValue(selectedType, out var paramType)
                    ? Activator.CreateInstance(paramType)
                    : null;

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Отрисовывает все дочерние поля параметров цели. </summary>
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

        /// <summary> Вычисляет общую высоту отрисовки параметров цели. </summary>
        /// <param name="paramsProp"> Сериализованное свойство параметров. </param>
        /// <returns> Общая высота в пикселях. </returns>
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