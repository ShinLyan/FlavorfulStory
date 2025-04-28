using System;
using System.Collections.Generic;
using UnityEngine;
using DayOfWeek = FlavorfulStory.TimeManagement.DayOfWeek;
#if UNITY_EDITOR
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEditor;
#endif

namespace FlavorfulStory.AI.Scheduling
{
    public class NpcScheduleViewer : MonoBehaviour
    {
        [Header("Npc Schedule")] public NpcSchedule schedule;

        [Header("Visualisation Settings")] public float LineThickness = 5f;
        public Color LineColor = Color.yellow;
        public float SphereSize = 0.5f;

        public int selectedParamIndex;
        public int selectedPointIndex;

        [Header("Ground Settings")] public float groundHeight = 0.5f;  
        public LayerMask groundMask = -1; 
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NpcScheduleViewer))]
    public class NpcScheduleViewerEditor : Editor
    {
        private static bool isInitialized;
        private static readonly List<Location> locations = new();

        private void OnEnable()
        {
            if (!isInitialized)
            {
                FindTaggedObjects();
                isInitialized = true;
            }
        }

        private static void FindTaggedObjects()
        {
            locations.Clear();
            var allObjects = FindObjectsByType<Location>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            locations.AddRange(allObjects);
        }

        private void OnSceneGUI()
        {
            var viewer = (NpcScheduleViewer)target;
            var schedule = viewer.schedule;
            if (schedule == null || schedule.Params == null) return;

            if (viewer.selectedParamIndex >= schedule.Params.Length) return;
            var param = schedule.Params[viewer.selectedParamIndex];
            if (param.Path == null || param.Path.Length == 0) return;

            // Рисуем точки и их соединения
            for (int i = 0; i < param.Path.Length; i++)
            {
                var pathPoint = param.Path[i];
                var newPosition = GetSurfaceAdjustedPosition(pathPoint.Position, viewer);
                
                if (pathPoint.Position != newPosition)
                {
                    pathPoint.Position = newPosition;
                    EditorUtility.SetDirty(schedule);
                }

                if (i < param.Path.Length - 1)
                {
                    var next = param.Path[i + 1];
                    Handles.color = viewer.LineColor;
                    Handles.DrawLine(pathPoint.Position, next.Position, viewer.LineThickness);
                }

                // Рисуем метку с фоном
                var labelPosition = pathPoint.Position + Vector3.forward * viewer.SphereSize;
                string labelContent =
                    $"{pathPoint.Hour:00}:{pathPoint.Minutes:00}\n{pathPoint.NpcAnimation}\n{pathPoint.LocationName}";
                var realLocationName = LocationName.RockyIsland;
                foreach (var location in locations)
                    if (location.IsPositionInLocation(pathPoint.Position))
                    {
                        realLocationName = location.LocationName;
                        break;
                    }
                
                var labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState
                    {
                        textColor = Color.black,
                        background = realLocationName == pathPoint.LocationName
                            ? Texture2D.whiteTexture
                            : Texture2D.grayTexture
                    },
                    padding = new RectOffset(6, 6, 4, 4),
                    alignment = TextAnchor.MiddleCenter
                };

                Handles.BeginGUI();
                var guiPosition = HandleUtility.WorldToGUIPoint(labelPosition);
                var content = new GUIContent(labelContent);
                var size = labelStyle.CalcSize(content);
                var rect = new Rect(guiPosition.x - size.x / 2, guiPosition.y - size.y / 2, size.x, size.y);

                GUI.Label(rect, content, labelStyle);
                Handles.EndGUI();

                Handles.color = i == viewer.selectedPointIndex ? Color.green : Color.red;
                Handles.SphereHandleCap(0, pathPoint.Position, Quaternion.identity, viewer.SphereSize,
                    EventType.Repaint);
            }

            // Отрисовываем инструменты для перемещения точки.
            if (viewer.selectedPointIndex >= 0 && viewer.selectedPointIndex < param.Path.Length)
            {
                var point = param.Path[viewer.selectedPointIndex];
                var pos = point.Position;
                var rot = Quaternion.Euler(point.Rotation);

                EditorGUI.BeginChangeCheck();
                var newPos = Handles.PositionHandle(pos, rot);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(schedule, "Move Schedule Point");
                    point.Position = newPos;
                    EditorUtility.SetDirty(schedule);
                }

                EditorGUI.BeginChangeCheck();
                var newRot = Handles.RotationHandle(rot, newPos);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(schedule, "Rotate Schedule Point");
                    point.Rotation = newRot.eulerAngles;
                    EditorUtility.SetDirty(schedule);
                }
            }
        }

        private Vector3 GetSurfaceAdjustedPosition(Vector3 position, NpcScheduleViewer viewer)
        {
            var ray = new Ray(position + Vector3.up * 50f, Vector3.down);
            if (Physics.Raycast(ray, out var hit, 100f, viewer.groundMask))
                return new Vector3(
                    position.x,
                    hit.point.y + viewer.groundHeight,
                    position.z
                );
            return position;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var viewer = (NpcScheduleViewer)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("schedule"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LineThickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LineColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SphereSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("groundHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("groundMask"));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (viewer.schedule == null)
            {
                EditorGUILayout.HelpBox("Assign a Schedule first!", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (viewer.selectedParamIndex > viewer.schedule.Params.Length - 1)
                viewer.selectedParamIndex = viewer.schedule.Params.Length - 1;

            if (viewer.selectedPointIndex > viewer.schedule.Params[viewer.selectedParamIndex].Path.Length - 1)
                viewer.selectedPointIndex = viewer.schedule.Params[viewer.selectedParamIndex].Path.Length - 1;

            HandleParameterCreation(viewer);
            if (viewer.schedule.Params.Length == 0)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            HandleParameterSelection(viewer);
            var currentParam = viewer.schedule.Params[viewer.selectedParamIndex];

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Selected Schedule Parameters: {viewer.selectedParamIndex}",
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
                Undo.RecordObject(viewer.schedule, "Change Schedule Parameter");
                EditorUtility.SetDirty(viewer.schedule);
            }

            ShowPathManagement(viewer);
            ShowSelectedPointEdit(viewer);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) SceneView.RepaintAll();
        }

        private void HandleParameterCreation(NpcScheduleViewer viewer)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add New Parameter"))
                {
                    Undo.RecordObject(viewer.schedule, "Add Parameter");

                    var newParams = viewer.schedule.Params != null
                        ? new ScheduleParams[viewer.schedule.Params.Length + 1]
                        : new ScheduleParams[1];

                    if (viewer.schedule.Params != null)
                        Array.Copy(viewer.schedule.Params, newParams, viewer.schedule.Params.Length);

                    newParams[^1] = new ScheduleParams();
                    viewer.schedule.Params = newParams;
                    viewer.selectedParamIndex = newParams.Length - 1;

                    EditorUtility.SetDirty(viewer.schedule);
                    SceneView.RepaintAll();
                }

                using (new EditorGUI.DisabledScope(viewer.schedule.Params == null ||
                                                   viewer.schedule.Params.Length == 0))
                {
                    if (GUILayout.Button("Remove Last Parameter"))
                    {
                        Undo.RecordObject(viewer.schedule, "Remove Parameter");

                        if (viewer.schedule.Params.Length > 0)
                        {
                            var newParams = new ScheduleParams[viewer.schedule.Params.Length - 1];
                            Array.Copy(viewer.schedule.Params, newParams, newParams.Length);
                            viewer.schedule.Params = newParams;
                            viewer.selectedParamIndex = Mathf.Clamp(viewer.selectedParamIndex, 0, newParams.Length - 1);
                        }
                        else
                        {
                            viewer.schedule.Params = Array.Empty<ScheduleParams>();
                        }

                        EditorUtility.SetDirty(viewer.schedule);
                        SceneView.RepaintAll();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void HandleParameterSelection(NpcScheduleViewer viewer)
        {
            EditorGUI.BeginChangeCheck();
            viewer.selectedParamIndex = EditorGUILayout.IntSlider(
                "Schedule Param",
                viewer.selectedParamIndex,
                0,
                Mathf.Max(0, viewer.schedule.Params.Length - 1)
            );
            if (EditorGUI.EndChangeCheck())
            {
                viewer.selectedPointIndex = 0;
                GUI.FocusControl(null);
            }
        }

        private static void ShowSeasonsEdit(ScheduleParams param)
        {
            param.Seasons = (Season)EditorGUILayout.EnumFlagsField("Seasons", param.Seasons);
        }

        private static void ShowDayOfWeekEdit(ScheduleParams param)
        {
            param.DayOfWeek = (DayOfWeek)EditorGUILayout.EnumFlagsField("Day of Week", param.DayOfWeek);
        }

        private static void ShowDateEdit(NpcScheduleViewer viewer)
        {
            var currentParam = viewer.schedule.Params[viewer.selectedParamIndex];
            EditorGUILayout.LabelField("Dates", EditorStyles.boldLabel);

            var dates = currentParam.Dates ?? Array.Empty<Vector2Int>();

            EditorGUILayout.BeginHorizontal();
            bool canRemove = dates.Length > 0;

            if (GUILayout.Button("Add Date"))
            {
                Undo.RecordObject(viewer.schedule, "Add Date");
                Array.Resize(ref dates, dates.Length + 1);
                dates[^1] = new Vector2Int(1, 28);
                currentParam.Dates = dates;
                EditorUtility.SetDirty(viewer.schedule);
            }

            using (new EditorGUI.DisabledScope(!canRemove))
            {
                if (GUILayout.Button("Remove Last Date") && canRemove)
                {
                    Undo.RecordObject(viewer.schedule, "Remove Date");
                    Array.Resize(ref dates, dates.Length - 1);
                    currentParam.Dates = dates;
                    EditorUtility.SetDirty(viewer.schedule);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (dates.Length > 0)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < dates.Length; i++)
                    dates[i] = EditorGUILayout.Vector2IntField(
                        $"Date Range {i + 1}",
                        dates[i]
                    );
                currentParam.Dates = dates;
                EditorGUI.indentLevel--;
            }
        }

        private static void ShowHeartsEdit(ScheduleParams param)
        {
            param.Hearts = EditorGUILayout.IntSlider("Hearts", param.Hearts, 0, 12);
        }

        private static void ShowIsRainingEdit(ScheduleParams param)
        {
            param.IsRaining = EditorGUILayout.Toggle("Raining", param.IsRaining);
        }

        private void ShowPathManagement(NpcScheduleViewer viewer)
        {
            var currentParam = viewer.schedule.Params[viewer.selectedParamIndex];
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField($"Path Points: {currentParam.Path.Length}", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            viewer.selectedPointIndex = EditorGUILayout.IntSlider(
                "Path Point",
                viewer.selectedPointIndex,
                0,
                Mathf.Max(0, currentParam.Path.Length - 1)
            );
            EditorGUI.EndChangeCheck();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Previous Point") && viewer.selectedPointIndex > 0) viewer.selectedPointIndex--;

                if (GUILayout.Button("Next Point") && viewer.selectedPointIndex < currentParam.Path.Length - 1)
                    viewer.selectedPointIndex++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Schedule Point")) AddSchedulePoint(viewer);

                if (GUILayout.Button("Delete Last Point") && currentParam.Path.Length > 0)
                    DeleteLastSchedulePoint(viewer);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddSchedulePoint(NpcScheduleViewer viewer)
        {
            var currentParam = viewer.schedule.Params[viewer.selectedParamIndex];
            var newPath = new SchedulePoint[currentParam.Path.Length + 1];
            currentParam.Path.CopyTo(newPath, 0);

            newPath[^1] = new SchedulePoint
            {
                Hour = 12,
                Minutes = 0,
                LocationName = LocationName.RockyIsland,
                NpcAnimation = AnimationType.Idle,
                Position = currentParam.Path.Length > 0
                    ? currentParam.Path[^1].Position + Vector3.forward
                    : Vector3.zero
            };

            Undo.RecordObject(viewer.schedule, "Add Schedule Point");
            currentParam.Path = newPath;
            viewer.selectedPointIndex = newPath.Length - 1;
            EditorUtility.SetDirty(viewer.schedule);
        }

        private void DeleteLastSchedulePoint(NpcScheduleViewer viewer)
        {
            var currentParam = viewer.schedule.Params[viewer.selectedParamIndex];
            var newPath = new SchedulePoint[currentParam.Path.Length - 1];
            bool canRemove = currentParam.Path.Length > 0;

            using (new EditorGUI.DisabledScope(!canRemove))
            {
                Array.Copy(currentParam.Path, newPath, currentParam.Path.Length - 1);

                Undo.RecordObject(viewer.schedule, "Delete Schedule Point");
                currentParam.Path = newPath;
                viewer.selectedPointIndex = Mathf.Clamp(viewer.selectedPointIndex, 0, newPath.Length - 1);
                EditorUtility.SetDirty(viewer.schedule);
            }
        }

        private static void ShowSelectedPointEdit(NpcScheduleViewer viewer)
        {
            var currentParam = viewer.schedule.Params[viewer.selectedParamIndex];
            if (currentParam.Path.Length == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected Point Settings", EditorStyles.boldLabel);

            var point = currentParam.Path[viewer.selectedPointIndex];
            EditorGUI.BeginChangeCheck();
            {
                point.Hour = EditorGUILayout.IntSlider("Hour", point.Hour, 0, 23);
                point.Minutes = EditorGUILayout.IntSlider("Minutes", point.Minutes, 0, 59);
                point.LocationName = (LocationName)EditorGUILayout.EnumPopup("Location", point.LocationName);
                point.NpcAnimation = (AnimationType)EditorGUILayout.EnumPopup("Animation", point.NpcAnimation);
            }
            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(viewer.schedule);
        }
    }
#endif
}