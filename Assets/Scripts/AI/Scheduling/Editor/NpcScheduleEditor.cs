#if UNITY_EDITOR

using System;
using FlavorfulStory.TimeManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using DayOfWeek = FlavorfulStory.TimeManagement.DayOfWeek;

namespace FlavorfulStory.AI.Scheduling.Editor
{
    /// <summary> Редактор для NpcScheduleDemonstrator. Обеспечивает визуальное редактирование расписаний NPC 
    /// в Unity Editor, включая управление точками маршрута, параметрами условий и визуализацией NavMesh. </summary>
    [CustomEditor(typeof(NpcScheduleDemonstrator))]
    public class NpcScheduleEditor : UnityEditor.Editor
    {
        /// <summary> Кэшированные данные триангуляции NavMesh. </summary>
        private NavMeshTriangulation _cachedNavMeshTriangulation;

        /// <summary> Флаг, указывающий, что NavMesh был закэширован. </summary>
        private bool _isNavMeshCached;

        /// <summary> Отрисовывает элементы в сцене: точки маршрута, линии между ними, метки с информацией, 
        /// инструменты для перемещения и вращения выбранной точки. </summary>
        private void OnSceneGUI()
        {
            if (!target) return;

            var viewer = (NpcScheduleDemonstrator)target;

            var schedule = viewer.Schedule;
            if (!schedule || schedule.Params == null) return;

            if (viewer.SelectedParamIndex >= schedule.Params.Length) return;

            var param = schedule.Params[viewer.SelectedParamIndex];
            if (param.Path == null || param.Path.Length == 0) return;

            DrawNavMesh(viewer);

            // Рисуем точки и их соединения
            for (int i = 0; i < param.Path.Length; i++)
            {
                var pathPoint = param.Path[i];
                // var newPosition = GetSurfaceAdjustedPosition(pathPoint.NpcDestinationPoint.Position, viewer);
                //
                // if (pathPoint.NpcDestinationPoint.Position != newPosition)
                // {
                //     pathPoint.SetTransform(newPosition, pathPoint.NpcDestinationPoint.Rotation);
                //     EditorUtility.SetDirty(schedule);
                // }

                if (i < param.Path.Length - 1)
                {
                    var next = param.Path[i + 1];
                    Handles.color = viewer.LineColor;
                    Handles.DrawLine(pathPoint.NpcDestinationPoint.Position, next.NpcDestinationPoint.Position,
                        viewer.LineThickness);
                }

                // Рисуем метку с фоном
                var labelPosition = pathPoint.NpcDestinationPoint.Position + Vector3.forward * viewer.SphereSize;
                string labelContent =
                    $"{pathPoint.Hour:00}:{pathPoint.Minutes:00}\n{pathPoint.NpcAnimation}";

                var labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState
                    {
                        textColor = Color.black,
                        background = Texture2D.whiteTexture
                    },
                    padding = new RectOffset(6, 6, 4, 4),
                    alignment = TextAnchor.MiddleCenter
                };

                if (viewer.ShowInfoLabel)
                {
                    Handles.BeginGUI();
                    var guiPosition = HandleUtility.WorldToGUIPoint(labelPosition);
                    var content = new GUIContent(labelContent);
                    var size = labelStyle.CalcSize(content);
                    var rect = new Rect(guiPosition.x - size.x / 2, guiPosition.y - size.y / 2, size.x, size.y);

                    GUI.Label(rect, content, labelStyle);
                    Handles.EndGUI();
                }

                Handles.color = i == viewer.SelectedPointIndex ? Color.green : Color.red;
                Handles.SphereHandleCap(0, pathPoint.NpcDestinationPoint.Position, Quaternion.identity,
                    viewer.SphereSize,
                    EventType.Repaint);
            }

            // Отрисовываем инструменты для перемещения точки.
            if (viewer.SelectedPointIndex < 0 || viewer.SelectedPointIndex >= param.Path.Length) return;

            var point = param.Path[viewer.SelectedPointIndex];
            var pos = point.NpcDestinationPoint.Position;
            var rot = point.NpcDestinationPoint.Rotation;

            if (rot == default || rot.Equals(new Quaternion(0, 0, 0, 0))) rot = Quaternion.identity;

            EditorGUI.BeginChangeCheck();
            var newPos = Handles.PositionHandle(pos, rot);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(schedule, "Move Schedule Point");
                point.SetTransform(newPos, rot);
                EditorUtility.SetDirty(schedule);
            }

            EditorGUI.BeginChangeCheck();
            var newRot = Handles.RotationHandle(rot, newPos);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(schedule, "Rotate Schedule Point");
                point.SetTransform(newPos, newRot);
                EditorUtility.SetDirty(schedule);
            }
        }

        /// <summary> Корректирует позицию точки с учетом высоты над поверхностью. </summary>
        /// <param name="position"> Исходная позиция точки. </param>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        /// <returns> Скорректированная позиция точки. </returns>
        private static Vector3 GetSurfaceAdjustedPosition(Vector3 position, NpcScheduleDemonstrator demonstrator)
        {
            var ray = new Ray(position + Vector3.up * 50f, Vector3.down);
            return Physics.Raycast(ray, out var hit, 100f, demonstrator.GroundMask)
                ? new Vector3(position.x, hit.point.y + demonstrator.GroundHeight, position.z)
                : position;
        }

        /// <summary> Отрисовывает кастомный интерфейс инспектора. Включает: 
        /// - Настройки визуализации (толщину линий, цвет, размер сфер)
        /// - Управление параметрами расписания (добавление/удаление, выбор сезонов, дней недели, дат)
        /// - Редактирование выбранной точки маршрута (время, анимация, локация)
        /// - Инструменты управления точками (предыдущая/следующая, добавление/удаление) </summary>
        public override void OnInspectorGUI()
        {
            if (!target) return;

            serializedObject.Update();
            var viewer = (NpcScheduleDemonstrator)target;

            var schedule = viewer.Schedule;

            EditorGUILayout.LabelField("Schedule", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<Schedule>k__BackingField"));

            EditorGUILayout.LabelField("Visualisation Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<LineThickness>k__BackingField"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<LineColor>k__BackingField"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<SphereSize>k__BackingField"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<ShowInfoLabel>k__BackingField"));

            EditorGUILayout.LabelField("Ground Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<GroundHeight>k__BackingField"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<GroundMask>k__BackingField"));

            EditorGUILayout.LabelField("Navmesh Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<NavMeshColor>k__BackingField"));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (!schedule)
            {
                EditorGUILayout.HelpBox("Assign a Schedule first!", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            HandleParameterCreation(viewer);
            if (schedule.Params.Length == 0)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (viewer.SelectedParamIndex > schedule.Params.Length - 1)
                viewer.SetNewValForParamIndex(schedule.Params.Length - 1);

            if (viewer.SelectedPointIndex > schedule.Params[viewer.SelectedParamIndex].Path.Length - 1)
                viewer.SetNewValForPointIndex(schedule.Params[viewer.SelectedParamIndex].Path.Length - 1);

            HandleParameterSelection(viewer);
            var currentParam = schedule.Params[viewer.SelectedParamIndex];

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Selected Schedule Parameters: {viewer.SelectedParamIndex}",
                EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            {
                ShowSeasonsEdit(currentParam);
                ShowDayOfWeekEdit(currentParam);
                ShowDateEdit(viewer);
                ShowHeartsEdit(currentParam);
                ShowIsRainingEdit(currentParam);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(schedule, "Change Schedule Parameter");
                EditorUtility.SetDirty(schedule);
            }

            ShowPathManagement(viewer);
            ShowSelectedPointEdit(viewer);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) SceneView.RepaintAll();
        }

        /// <summary> Обрабатывает создание и удаление параметров расписания. </summary>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        private static void HandleParameterCreation(NpcScheduleDemonstrator demonstrator)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add New Parameter"))
                {
                    Undo.RecordObject(demonstrator.Schedule, "Add Parameter");

                    var newParams = demonstrator.Schedule.Params != null
                        ? new NpcScheduleParams[demonstrator.Schedule.Params.Length + 1]
                        : new NpcScheduleParams[1];

                    if (demonstrator.Schedule.Params != null)
                        Array.Copy(demonstrator.Schedule.Params, newParams, demonstrator.Schedule.Params.Length);

                    newParams[^1] = new NpcScheduleParams();
                    demonstrator.Schedule.Params = newParams;
                    demonstrator.SetNewValForParamIndex(newParams.Length - 1);

                    EditorUtility.SetDirty(demonstrator.Schedule);
                    SceneView.RepaintAll();
                }

                using (new EditorGUI.DisabledScope(demonstrator.Schedule.Params == null ||
                                                   demonstrator.Schedule.Params.Length == 0))
                {
                    if (GUILayout.Button("Remove Last Parameter"))
                    {
                        Undo.RecordObject(demonstrator.Schedule, "Remove Parameter");

                        if (demonstrator.Schedule.Params is { Length: > 0 })
                        {
                            var newParams = new NpcScheduleParams[demonstrator.Schedule.Params.Length - 1];
                            Array.Copy(demonstrator.Schedule.Params, newParams, newParams.Length);
                            demonstrator.Schedule.Params = newParams;
                            demonstrator.SetNewValForParamIndex(
                                Mathf.Clamp(demonstrator.SelectedParamIndex, 0, newParams.Length - 1));
                        }
                        else
                        {
                            demonstrator.Schedule.Params = Array.Empty<NpcScheduleParams>();
                        }

                        EditorUtility.SetDirty(demonstrator.Schedule);
                        SceneView.RepaintAll();
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary> Управляет выбором текущего параметра расписания через слайдер. </summary>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        private static void HandleParameterSelection(NpcScheduleDemonstrator demonstrator)
        {
            EditorGUI.BeginChangeCheck();
            int newParamIndex = EditorGUILayout.IntSlider(
                "Schedule Param",
                demonstrator.SelectedParamIndex,
                0,
                Mathf.Max(0, demonstrator.Schedule.Params.Length - 1)
            );

            if (!EditorGUI.EndChangeCheck()) return;

            demonstrator.SetNewValForParamIndex(newParamIndex);
            demonstrator.SetNewValForPointIndex(0);
            GUI.FocusControl(null);
        }

        /// <summary> Отображает редактор для выбора сезонов. </summary>
        /// <param name="param"> Параметр расписания, в который записываются выбранные значения. </param>
        private static void ShowSeasonsEdit(NpcScheduleParams param)
        {
            param.Seasons = (Season)EditorGUILayout.EnumFlagsField("Seasons", param.Seasons);
        }

        /// <summary> Отображает редактор для выбора дней недели. </summary>
        /// <param name="param"> Параметр расписания, в который записываются выбранные значения. </param>
        private static void ShowDayOfWeekEdit(NpcScheduleParams param)
        {
            param.DayOfWeek = (DayOfWeek)EditorGUILayout.EnumFlagsField("Day of Week", param.DayOfWeek);
        }

        /// <summary> Управляет редактированием дат для параметра расписания. </summary>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        private static void ShowDateEdit(NpcScheduleDemonstrator demonstrator)
        {
            var currentParam = demonstrator.Schedule.Params[demonstrator.SelectedParamIndex];
            EditorGUILayout.LabelField("Dates", EditorStyles.boldLabel);

            var dates = currentParam.Dates ?? Array.Empty<Vector2Int>();

            EditorGUILayout.BeginHorizontal();
            bool canRemove = dates.Length > 0;

            if (GUILayout.Button("Add Date"))
            {
                Undo.RecordObject(demonstrator.Schedule, "Add Date");
                Array.Resize(ref dates, dates.Length + 1);
                dates[^1] = new Vector2Int(1, 28);
                currentParam.Dates = dates;
                EditorUtility.SetDirty(demonstrator.Schedule);
            }

            using (new EditorGUI.DisabledScope(!canRemove))
            {
                if (GUILayout.Button("Remove Last Date") && canRemove)
                {
                    Undo.RecordObject(demonstrator.Schedule, "Remove Date");
                    Array.Resize(ref dates, dates.Length - 1);
                    currentParam.Dates = dates;
                    EditorUtility.SetDirty(demonstrator.Schedule);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (dates.Length <= 0) return;

            EditorGUI.indentLevel++;
            for (int i = 0; i < dates.Length; i++)
                dates[i] = EditorGUILayout.Vector2IntField($"Date Range {i + 1}", dates[i]);

            currentParam.Dates = dates;
            EditorGUI.indentLevel--;
        }

        /// <summary> Отображает слайдер для настройки уровня отношений (Hearts). </summary>
        /// <param name="param"> Параметр расписания, в который записываются выбранные значения. </param>
        private static void ShowHeartsEdit(NpcScheduleParams param)
        {
            param.Hearts = EditorGUILayout.IntSlider("Hearts", param.Hearts, 0, 12);
        }

        /// <summary> Отображает переключатель для условия дождя. </summary>
        /// <param name="param"> Параметр расписания, в который записываются выбранные значения. </param>
        private static void ShowIsRainingEdit(NpcScheduleParams param)
        {
            param.IsRaining = EditorGUILayout.Toggle("Raining", param.IsRaining);
        }

        /// <summary> Управляет отображением и взаимодействием с точками маршрута. </summary>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        private static void ShowPathManagement(NpcScheduleDemonstrator demonstrator)
        {
            var currentParam = demonstrator.Schedule.Params[demonstrator.SelectedParamIndex];
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField($"Path Points: {currentParam.Path.Length}", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            int newPointIndex = EditorGUILayout.IntSlider(
                "Path Point",
                demonstrator.SelectedPointIndex,
                0,
                Mathf.Max(0, currentParam.Path.Length - 1)
            );
            if (EditorGUI.EndChangeCheck()) demonstrator.SetNewValForPointIndex(newPointIndex);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Previous Point") && demonstrator.SelectedPointIndex > 0)
                    demonstrator.DecrementSelectedPointIndex();

                if (GUILayout.Button("Next Point") && demonstrator.SelectedPointIndex < currentParam.Path.Length - 1)
                    demonstrator.IncrementSelectedPointIndex();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Schedule Point")) AddSchedulePoint(demonstrator);

                bool canDelete = currentParam.Path.Length > 0;
                using (new EditorGUI.DisabledScope(!canDelete))
                {
                    if (GUILayout.Button("Delete Last Point")) DeleteLastSchedulePoint(demonstrator);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary> Добавляет новую точку в маршрут. </summary>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        private static void AddSchedulePoint(NpcScheduleDemonstrator demonstrator)
        {
            var currentParam = demonstrator.Schedule.Params[demonstrator.SelectedParamIndex];
            var newPath = new NpcSchedulePoint[currentParam.Path.Length + 1];
            currentParam.Path.CopyTo(newPath, 0);

            newPath[^1] = new NpcSchedulePoint
            {
                Hour = 12,
                Minutes = 0,
                NpcAnimation = AnimationType.Idle
            };
            var pos = currentParam.Path.Length > 0
                ? currentParam.Path[^1].NpcDestinationPoint.Position + Vector3.forward
                : Vector3.zero;
            newPath[^1].SetTransform(pos, Quaternion.identity);

            Undo.RecordObject(demonstrator.Schedule, "Add Schedule Point");
            currentParam.Path = newPath;
            demonstrator.SetNewValForPointIndex(newPath.Length - 1);
            EditorUtility.SetDirty(demonstrator.Schedule);
        }

        /// <summary> Удаляет последнюю точку маршрута. </summary>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        private static void DeleteLastSchedulePoint(NpcScheduleDemonstrator demonstrator)
        {
            var currentParam = demonstrator.Schedule.Params[demonstrator.SelectedParamIndex];
            var newPath = new NpcSchedulePoint[currentParam.Path.Length - 1];

            Array.Copy(currentParam.Path, newPath, currentParam.Path.Length - 1);

            Undo.RecordObject(demonstrator.Schedule, "Delete Schedule Point");
            currentParam.Path = newPath;
            demonstrator.SetNewValForPointIndex(Mathf.Clamp(demonstrator.SelectedPointIndex, 0, newPath.Length - 1));
            EditorUtility.SetDirty(demonstrator.Schedule);
        }

        /// <summary> Отображает настройки выбранной точки маршрута. </summary>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        private static void ShowSelectedPointEdit(NpcScheduleDemonstrator demonstrator)
        {
            var currentParam = demonstrator.Schedule.Params[demonstrator.SelectedParamIndex];
            if (currentParam.Path.Length == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected Point Settings", EditorStyles.boldLabel);

            var point = currentParam.Path[demonstrator.SelectedPointIndex];
            EditorGUI.BeginChangeCheck();
            {
                point.Hour = EditorGUILayout.IntSlider("Hour", point.Hour, 0, 23);
                point.Minutes = EditorGUILayout.IntSlider("Minutes", point.Minutes, 0, 59);
                point.NpcAnimation = (AnimationType)EditorGUILayout.EnumPopup("Animation", point.NpcAnimation);
            }

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(demonstrator.Schedule);
        }

        /// <summary> Отрисовывает NavMesh в сцене. </summary>
        /// <param name="demonstrator"> Демонстратор расписания. </param>
        private void DrawNavMesh(NpcScheduleDemonstrator demonstrator)
        {
            if (!_isNavMeshCached)
            {
                _cachedNavMeshTriangulation = NavMesh.CalculateTriangulation();
                _isNavMeshCached = true;
            }

            Handles.color = demonstrator.NavMeshColor;

            for (int i = 0; i < _cachedNavMeshTriangulation.indices.Length; i += 3)
            {
                var a = _cachedNavMeshTriangulation.vertices[_cachedNavMeshTriangulation.indices[i]];
                var b = _cachedNavMeshTriangulation.vertices[_cachedNavMeshTriangulation.indices[i + 1]];
                var c = _cachedNavMeshTriangulation.vertices[_cachedNavMeshTriangulation.indices[i + 2]];
                Handles.DrawAAConvexPolygon(a, b, c);
            }
        }
    }
}

#endif