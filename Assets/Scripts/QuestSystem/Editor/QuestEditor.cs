using System;
using System.Collections.Generic;
using FlavorfulStory.QuestSystem.Objectives;
using FlavorfulStory.QuestSystem.Objectives.Params;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FlavorfulStory.QuestSystem.Editor
{
    /// <summary> Кастомный инспектор для квестов. Позволяет удобно редактировать цели и награды квеста. </summary>
    [CustomEditor(typeof(Quest))]
    public class QuestEditor : UnityEditor.Editor
    {
        /// <summary> Сериализованное свойство списка целей. </summary>
        private SerializedProperty _objectivesProperty;

        /// <summary> Сериализованное свойство списка наград. </summary>
        private SerializedProperty _rewardsProperty;

        /// <summary> ReorderableList для целей квеста. </summary>
        private ReorderableList _objectivesList;

        /// <summary> Словарь состояния свёрнутости целей. </summary>
        private readonly Dictionary<int, bool> _objectiveFoldouts = new();

        /// <summary> Вызывается при включении инспектора. Инициализирует ссылки и список целей. </summary>
        private void OnEnable()
        {
            _objectivesProperty = serializedObject.FindProperty("_objectives");
            _rewardsProperty = serializedObject.FindProperty("_rewards");
            SetupObjectivesReorderableList();
        }

        /// <summary> Основной метод отрисовки инспектора. </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawMainQuestFields();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            _objectivesList.DoLayoutList();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            DrawRewardsSection();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Настраивает reorderable list для целей квеста. </summary>
        private void SetupObjectivesReorderableList()
        {
            _objectivesList = new ReorderableList(serializedObject, _objectivesProperty, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Objectives", EditorStyles.boldLabel),
                drawElementCallback = DrawObjectiveElement,
                elementHeightCallback = CalculateObjectiveHeight,
                onAddCallback = list => AddObjective()
            };
        }

        /// <summary> Отрисовывает поля основной информации о квесте. </summary>
        private void DrawMainQuestFields()
        {
            DrawSerializedField("<QuestGiver>k__BackingField");
            DrawSerializedField("<QuestName>k__BackingField");
            DrawSerializedField("<QuestDescription>k__BackingField");
            DrawSerializedField("<QuestType>k__BackingField");
        }

        /// <summary> Отрисовывает SerializedProperty по имени. </summary>
        /// <param name="propertyName"> Имя сериализованного свойства. </param>
        private void DrawSerializedField(string propertyName)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null) EditorGUILayout.PropertyField(property);
        }

        /// <summary> Отрисовывает элемент цели в списке. </summary>
        /// <param name="rect"> Прямоугольник для отрисовки. </param>
        /// <param name="index"> Индекс цели. </param>
        /// <param name="isActive"> Является ли элемент активным. </param>
        /// <param name="isFocused"> Имеет ли элемент фокус. </param>
        private void DrawObjectiveElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var objectiveProp = _objectivesProperty.GetArrayElementAtIndex(index);
            var (descriptionProp, typeProp, paramsProp) = GetObjectiveProperties(objectiveProp);

            rect.y += 2;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            _objectiveFoldouts.TryAdd(index, true);
            _objectiveFoldouts[index] = EditorGUI.Foldout(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                _objectiveFoldouts[index], $"Objective {index + 1}", true);

            rect.y += EditorGUIUtility.singleLineHeight + spacing;

            if (!_objectiveFoldouts[index]) return;

            float descHeight = EditorGUI.GetPropertyHeight(descriptionProp, true);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, descHeight), descriptionProp, true);

            rect.y += descHeight + spacing;

            float typeHeight = EditorGUI.GetPropertyHeight(typeProp, true);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, typeHeight), typeProp, true);
            if (EditorGUI.EndChangeCheck()) UpdateObjectiveParams(typeProp, paramsProp);

            rect.y += typeHeight + spacing;

            if (paramsProp?.managedReferenceValue != null) DrawParamsFields(rect, paramsProp);
        }

        /// <summary> Вычисляет высоту элемента цели для корректного отображения. </summary>
        /// <param name="index"> Индекс цели. </param>
        /// <returns> Высота элемента в пикселях. </returns>
        private float CalculateObjectiveHeight(int index)
        {
            if (_objectiveFoldouts.TryGetValue(index, out bool expanded) && !expanded)
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var objectiveProp = _objectivesProperty.GetArrayElementAtIndex(index);
            var (descriptionProp, typeProp, paramsProp) = GetObjectiveProperties(objectiveProp);

            float height = EditorGUIUtility.singleLineHeight + // foldout
                           EditorGUI.GetPropertyHeight(descriptionProp, true) +
                           EditorGUI.GetPropertyHeight(typeProp, true) +
                           EditorGUIUtility.standardVerticalSpacing * 4;

            if (paramsProp?.managedReferenceValue != null) height += CalculateParamsHeight(paramsProp);

            return height;
        }

        /// <summary> Добавляет новую цель в список целей. </summary>
        private void AddObjective()
        {
            _objectivesProperty.InsertArrayElementAtIndex(_objectivesProperty.arraySize);
            var newObjective = _objectivesProperty.GetArrayElementAtIndex(_objectivesProperty.arraySize - 1);
            InitializeNewObjective(newObjective);
        }

        /// <summary> Отрисовывает секцию наград в инспекторе. </summary>
        private void DrawRewardsSection() => EditorGUILayout.PropertyField(_rewardsProperty, true);

        /// <summary> Получает сериализованные свойства цели. </summary>
        /// <param name="objectiveProp"> Сериализованная цель. </param>
        /// <returns> Кортеж с Description, Type и Params свойствами. </returns>
        private static (SerializedProperty description, SerializedProperty type, SerializedProperty @params)
            GetObjectiveProperties(SerializedProperty objectiveProp)
        {
            var descriptionProp = objectiveProp.FindPropertyRelative("<Description>k__BackingField");
            var typeProp = objectiveProp.FindPropertyRelative("<Type>k__BackingField");
            var paramsProp = objectiveProp.FindPropertyRelative("<Params>k__BackingField");
            return (descriptionProp, typeProp, paramsProp);
        }

        /// <summary> Инициализирует новую цель при добавлении. </summary>
        /// <param name="objectiveProp"> Сериализованная цель. </param>
        private void InitializeNewObjective(SerializedProperty objectiveProp)
        {
            var referenceProp = objectiveProp.FindPropertyRelative("<Reference>k__BackingField");
            var (descriptionProp, typeProp, paramsProp) = GetObjectiveProperties(objectiveProp);

            referenceProp.stringValue = Guid.NewGuid().ToString();
            descriptionProp.stringValue = string.Empty;
            typeProp.enumValueIndex = 0;

            UpdateObjectiveParams(typeProp, paramsProp);
        }

        /// <summary> Обновляет параметры цели при изменении её типа. </summary>
        /// <param name="typeProp"> SerializedProperty типа цели. </param>
        /// <param name="paramsProp"> SerializedProperty параметров цели. </param>
        private void UpdateObjectiveParams(SerializedProperty typeProp, SerializedProperty paramsProp)
        {
            var selectedType = (ObjectiveType)typeProp.enumValueIndex;

            paramsProp.managedReferenceValue =
                ObjectiveParamsRegistry.Mapping.TryGetValue(selectedType, out var paramsType)
                    ? Activator.CreateInstance(paramsType)
                    : null;

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Отрисовывает поля параметров цели. </summary>
        /// <param name="rect"> Прямоугольник для отрисовки. </param>
        /// <param name="paramsProp"> SerializedProperty параметров. </param>
        private static void DrawParamsFields(Rect rect, SerializedProperty paramsProp)
        {
            if (paramsProp is not { hasVisibleChildren: true }) return;

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

        /// <summary> Вычисляет общую высоту для всех параметров цели. </summary>
        /// <param name="paramsProp"> SerializedProperty параметров. </param>
        /// <returns> Общая высота параметров. </returns>
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