using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
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
        [Header("Npc Schedule")] public NpcSchedule Schedule;

        [Header("Visualisation Settings")] public float LineThickness = 5f;
        public Color LineColor = Color.yellow;
        public float SphereSize = 0.5f;

        public int SelectedParamIndex;
        public int SelectedPointIndex;

        [Header("Ground Settings")] public float GroundHeight = 0.5f;

        public LayerMask GroundMask = -1;

        [Header("NavMesh Settings")] public Color NavMeshColor = new(0, 0.5f, 1f, 0.2f);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NpcScheduleViewer))]
    public class NpcScheduleViewerEditor : Editor
    {
        private static readonly List<Location> locations = new();

        [InitializeOnLoadMethod]
        private static void InitOnLoad()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorSceneManager.sceneOpened += (_, _) => RefreshLocations();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.EnteredEditMode or PlayModeStateChange.EnteredPlayMode) RefreshLocations();
        }

        private void OnEnable() => RefreshLocations();

        private static void RefreshLocations()
        {
            locations.Clear();

            if (!Application.isPlaying || SceneManager.GetActiveScene().isLoaded)
            {
                var locs = FindObjectsByType<Location>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                locations.AddRange(locs);
            }
        }


        private void OnSceneGUI()
        {
            if (target == null) return;

            var viewer = (NpcScheduleViewer)target;
            var schedule = viewer.Schedule;
            if (schedule == null || schedule.Params == null) return;

            if (viewer.SelectedParamIndex >= schedule.Params.Length) return;
            var param = schedule.Params[viewer.SelectedParamIndex];
            if (param.Path == null || param.Path.Length == 0) return;

            DrawNavMesh(viewer);

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

                Handles.color = i == viewer.SelectedPointIndex ? Color.green : Color.red;
                Handles.SphereHandleCap(0, pathPoint.Position, Quaternion.identity, viewer.SphereSize,
                    EventType.Repaint);
            }

            // Отрисовываем инструменты для перемещения точки.
            if (viewer.SelectedPointIndex >= 0 && viewer.SelectedPointIndex < param.Path.Length)
            {
                var point = param.Path[viewer.SelectedPointIndex];
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
            if (Physics.Raycast(ray, out var hit, 100f, viewer.GroundMask))
                return new Vector3(
                    position.x,
                    hit.point.y + viewer.GroundHeight,
                    position.z
                );
            return position;
        }


        public override void OnInspectorGUI()
        {
            if (target == null) return;
            serializedObject.Update();
            var viewer = (NpcScheduleViewer)target;

            var schedule = viewer.Schedule;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Schedule"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LineThickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LineColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SphereSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GroundHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GroundMask"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NavMeshColor"));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (schedule == null)
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
                viewer.SelectedParamIndex = schedule.Params.Length - 1;

            if (viewer.SelectedPointIndex > schedule.Params[viewer.SelectedParamIndex].Path.Length - 1)
                viewer.SelectedPointIndex = schedule.Params[viewer.SelectedParamIndex].Path.Length - 1;

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

        private void HandleParameterCreation(NpcScheduleViewer viewer)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add New Parameter"))
                {
                    Undo.RecordObject(viewer.Schedule, "Add Parameter");

                    var newParams = viewer.Schedule.Params != null
                        ? new ScheduleParams[viewer.Schedule.Params.Length + 1]
                        : new ScheduleParams[1];

                    if (viewer.Schedule.Params != null)
                        Array.Copy(viewer.Schedule.Params, newParams, viewer.Schedule.Params.Length);

                    newParams[^1] = new ScheduleParams();
                    viewer.Schedule.Params = newParams;
                    viewer.SelectedParamIndex = newParams.Length - 1;

                    EditorUtility.SetDirty(viewer.Schedule);
                    SceneView.RepaintAll();
                }

                using (new EditorGUI.DisabledScope(viewer.Schedule.Params == null ||
                                                   viewer.Schedule.Params.Length == 0))
                {
                    if (GUILayout.Button("Remove Last Parameter"))
                    {
                        Undo.RecordObject(viewer.Schedule, "Remove Parameter");

                        if (viewer.Schedule.Params.Length > 0)
                        {
                            var newParams = new ScheduleParams[viewer.Schedule.Params.Length - 1];
                            Array.Copy(viewer.Schedule.Params, newParams, newParams.Length);
                            viewer.Schedule.Params = newParams;
                            viewer.SelectedParamIndex = Mathf.Clamp(viewer.SelectedParamIndex, 0, newParams.Length - 1);
                        }
                        else
                        {
                            viewer.Schedule.Params = Array.Empty<ScheduleParams>();
                        }

                        EditorUtility.SetDirty(viewer.Schedule);
                        SceneView.RepaintAll();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void HandleParameterSelection(NpcScheduleViewer viewer)
        {
            EditorGUI.BeginChangeCheck();
            viewer.SelectedParamIndex = EditorGUILayout.IntSlider(
                "Schedule Param",
                viewer.SelectedParamIndex,
                0,
                Mathf.Max(0, viewer.Schedule.Params.Length - 1)
            );
            if (EditorGUI.EndChangeCheck())
            {
                viewer.SelectedPointIndex = 0;
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
            var currentParam = viewer.Schedule.Params[viewer.SelectedParamIndex];
            EditorGUILayout.LabelField("Dates", EditorStyles.boldLabel);

            var dates = currentParam.Dates ?? Array.Empty<Vector2Int>();

            EditorGUILayout.BeginHorizontal();
            bool canRemove = dates.Length > 0;

            if (GUILayout.Button("Add Date"))
            {
                Undo.RecordObject(viewer.Schedule, "Add Date");
                Array.Resize(ref dates, dates.Length + 1);
                dates[^1] = new Vector2Int(1, 28);
                currentParam.Dates = dates;
                EditorUtility.SetDirty(viewer.Schedule);
            }

            using (new EditorGUI.DisabledScope(!canRemove))
            {
                if (GUILayout.Button("Remove Last Date") && canRemove)
                {
                    Undo.RecordObject(viewer.Schedule, "Remove Date");
                    Array.Resize(ref dates, dates.Length - 1);
                    currentParam.Dates = dates;
                    EditorUtility.SetDirty(viewer.Schedule);
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
            var currentParam = viewer.Schedule.Params[viewer.SelectedParamIndex];
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField($"Path Points: {currentParam.Path.Length}", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            viewer.SelectedPointIndex = EditorGUILayout.IntSlider(
                "Path Point",
                viewer.SelectedPointIndex,
                0,
                Mathf.Max(0, currentParam.Path.Length - 1)
            );
            EditorGUI.EndChangeCheck();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Previous Point") && viewer.SelectedPointIndex > 0) viewer.SelectedPointIndex--;

                if (GUILayout.Button("Next Point") && viewer.SelectedPointIndex < currentParam.Path.Length - 1)
                    viewer.SelectedPointIndex++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Schedule Point")) AddSchedulePoint(viewer);

                bool canDelete = currentParam.Path.Length > 0;
                using (new EditorGUI.DisabledScope(!canDelete))
                {
                    if (GUILayout.Button("Delete Last Point")) DeleteLastSchedulePoint(viewer);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddSchedulePoint(NpcScheduleViewer viewer)
        {
            var currentParam = viewer.Schedule.Params[viewer.SelectedParamIndex];
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

            Undo.RecordObject(viewer.Schedule, "Add Schedule Point");
            currentParam.Path = newPath;
            viewer.SelectedPointIndex = newPath.Length - 1;
            EditorUtility.SetDirty(viewer.Schedule);
        }

        private void DeleteLastSchedulePoint(NpcScheduleViewer viewer)
        {
            var currentParam = viewer.Schedule.Params[viewer.SelectedParamIndex];
            var newPath = new SchedulePoint[currentParam.Path.Length - 1];

            Array.Copy(currentParam.Path, newPath, currentParam.Path.Length - 1);

            Undo.RecordObject(viewer.Schedule, "Delete Schedule Point");
            currentParam.Path = newPath;
            viewer.SelectedPointIndex = Mathf.Clamp(viewer.SelectedPointIndex, 0, newPath.Length - 1);
            EditorUtility.SetDirty(viewer.Schedule);
        }

        private static void ShowSelectedPointEdit(NpcScheduleViewer viewer)
        {
            var currentParam = viewer.Schedule.Params[viewer.SelectedParamIndex];
            if (currentParam.Path.Length == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected Point Settings", EditorStyles.boldLabel);

            var point = currentParam.Path[viewer.SelectedPointIndex];
            EditorGUI.BeginChangeCheck();
            {
                point.Hour = EditorGUILayout.IntSlider("Hour", point.Hour, 0, 23);
                point.Minutes = EditorGUILayout.IntSlider("Minutes", point.Minutes, 0, 59);
                point.LocationName = (LocationName)EditorGUILayout.EnumPopup("Location", point.LocationName);
                point.NpcAnimation = (AnimationType)EditorGUILayout.EnumPopup("Animation", point.NpcAnimation);
            }
            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(viewer.Schedule);
        }

        private void DrawNavMesh(NpcScheduleViewer viewer)
        {
            var triangulation = NavMesh.CalculateTriangulation();

            Handles.color = viewer.NavMeshColor;
            for (int i = 0; i < triangulation.indices.Length; i += 3)
            {
                var a = triangulation.vertices[triangulation.indices[i]];
                var b = triangulation.vertices[triangulation.indices[i + 1]];
                var c = triangulation.vertices[triangulation.indices[i + 2]];

                Handles.DrawAAConvexPolygon(a, b, c);
            }
        }
    }
#endif
}