using UnityEngine;
#if UNITY_EDITOR
using FlavorfulStory.SceneManagement;
using FlavorfulStory.TimeManagement;
using UnityEditor;
#endif

namespace FlavorfulStory.AI.Scheduling
{
    public class NpcScheduleViewer : MonoBehaviour
    {
        public NpcSchedule schedule;
        [HideInInspector] public int selectedParamIndex;
        [HideInInspector] public int selectedPointIndex;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NpcScheduleViewer))]
    public class NpcScheduleViewerEditor : Editor
    {
        private void OnSceneGUI()
        {
            var viewer = (NpcScheduleViewer)target;
            var schedule = viewer.schedule;
            if (schedule == null || schedule.Params == null) return;

            if (viewer.selectedParamIndex >= schedule.Params.Length) return;
            var param = schedule.Params[viewer.selectedParamIndex];
            if (param.Path == null || param.Path.Length == 0) return;

            // Создаем стиль для меток
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

            // Draw all points and lines
            for (int i = 0; i < param.Path.Length; i++)
            {
                Handles.color = i == viewer.selectedPointIndex ? Color.green : Color.red;
                var p = param.Path[i];

                // Рисуем метку с фоном
                var labelPosition = p.Position + Vector3.up * 1f;
                string labelContent = $"{p.Hour:00}:{p.Minutes:00}\n{p.NpcAnimation}\n{p.LocationName}";

                Handles.BeginGUI();
                var guiPosition = HandleUtility.WorldToGUIPoint(labelPosition);
                var content = new GUIContent(labelContent);
                var size = labelStyle.CalcSize(content);
                var rect = new Rect(guiPosition.x - size.x / 2, guiPosition.y - size.y / 2, size.x, size.y);

                GUI.Label(rect, content, labelStyle);
                Handles.EndGUI();
                Handles.SphereHandleCap(0, p.Position, Quaternion.identity, 0.5f, EventType.Repaint);

                // Draw line to next point
                if (i < param.Path.Length - 1)
                {
                    var next = param.Path[i + 1];
                    Handles.color = Color.yellow;
                    Handles.DrawLine(p.Position, next.Position, 1f);
                }
            }

            // Draw handles for selected point only
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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var viewer = (NpcScheduleViewer)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("schedule"));

            if (viewer.schedule == null)
            {
                EditorGUILayout.HelpBox("Assign a Schedule first!", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUI.BeginChangeCheck();
            viewer.selectedParamIndex = EditorGUILayout.IntSlider(
                "Schedule Param",
                viewer.selectedParamIndex,
                0,
                Mathf.Max(0, viewer.schedule.Params.Length - 1)
            );
            bool paramChanged = EditorGUI.EndChangeCheck();

            if (paramChanged)
            {
                viewer.selectedPointIndex = 0;
                GUI.FocusControl(null); // Сброс фокуса
            }


            if (viewer.schedule.Params.Length > 0)
            {
                var currentParam = viewer.schedule.Params[viewer.selectedParamIndex];

                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Selected Schedule Parameters: {viewer.selectedParamIndex}",
                    EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                currentParam.Seasons = (Season)EditorGUILayout.EnumFlagsField("Seasons", currentParam.Seasons);
                currentParam.DayOfWeek =
                    (DayOfWeek)EditorGUILayout.EnumFlagsField("Day of Week", currentParam.DayOfWeek);
                for (int i = 0; i < currentParam.Dates.Length; i++)
                    currentParam.Dates[i] = EditorGUILayout.Vector2IntField($"Date {i + 1}", currentParam.Dates[i]);
                currentParam.Hearts = EditorGUILayout.IntSlider("Hearts", currentParam.Hearts, 0, 12);
                currentParam.IsRaining = EditorGUILayout.Toggle("Raining", currentParam.IsRaining);
                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(viewer.schedule);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Path Points: {currentParam.Path.Length}", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                viewer.selectedPointIndex = EditorGUILayout.IntSlider(
                    "Path Point",
                    viewer.selectedPointIndex,
                    0,
                    Mathf.Max(0, currentParam.Path.Length - 1)
                );
                EditorGUI.EndChangeCheck();


                // Редакт точек.
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous Point"))
                    if (viewer.selectedPointIndex > 0)
                        viewer.selectedPointIndex--;
                if (GUILayout.Button("Next Point"))
                    if (viewer.selectedPointIndex < currentParam.Path.Length - 1)
                        viewer.selectedPointIndex++;
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add Schedule Point"))
                {
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

                    currentParam.Path = newPath;
                    viewer.selectedPointIndex = newPath.Length - 1;
                }

                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(viewer.schedule);

                // Редактирование выбранной точки
                if (currentParam.Path.Length > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Selected Point Settings", EditorStyles.boldLabel);

                    var point = currentParam.Path[viewer.selectedPointIndex];

                    EditorGUI.BeginChangeCheck();
                    point.Hour = EditorGUILayout.IntSlider("Hour", point.Hour, 0, 23);
                    point.Minutes = EditorGUILayout.IntSlider("Minutes", point.Minutes, 0, 59);
                    point.LocationName = (LocationName)EditorGUILayout.EnumPopup("Location", point.LocationName);
                    point.NpcAnimation = (AnimationType)EditorGUILayout.EnumPopup("Animation", point.NpcAnimation);
                    if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(viewer.schedule);
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (paramChanged || GUI.changed) SceneView.RepaintAll();
        }
    }
#endif
}