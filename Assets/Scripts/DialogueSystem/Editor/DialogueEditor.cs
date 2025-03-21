using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem.Editor
{
    /// <summary> Окно редактора диалогов. </summary>
    public class DialogueEditor : EditorWindow
    {
        /// <summary> Инициализация редактора при открытии. </summary>
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            InitializeStyles();
        }

        /// <summary> Отрисовать GUI редактора. </summary>
        private void OnGUI()
        {
            if (!_selectedDialogue)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
                return;
            }

            ProcessEvents();
            DrawCanvas();
            HandleNodeCreationDeletion();
        }

        /// <summary> Открывает редактор диалогов при двойном клике по файлу диалога. </summary>
        /// <param name="instanceId"> Идентификатор объекта. </param>
        /// <param name="line"> Номер строки (не используется). </param>
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceId) is not Dialogue) return false;

            ShowEditorWindow();
            return true;
        }

        /// <summary> Открывает окно редактора диалогов. </summary>
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        /// <summary> Инициализация стилей узлов. </summary>
        private void InitializeStyles()
        {
            _nodeStyle = CreateNodeStyle("node0");
            _playerNodeStyle = CreateNodeStyle("node1");
        }

        /// <summary> Создает стиль узла. </summary>
        /// <param name="textureName"> Название текстуры для узла. </param>
        /// <returns> Созданный стиль. </returns>
        private static GUIStyle CreateNodeStyle(string textureName) => new()
        {
            normal =
            {
                background = EditorGUIUtility.Load(textureName) as Texture2D,
                textColor = Color.white
            },
            padding = new RectOffset(20, 20, 20, 20),
            border = new RectOffset(12, 12, 12, 12)
        };

        /// <summary> Обработать пользовательские события. </summary>
        private void OnSelectionChanged()
        {
            if (Selection.activeObject is not Dialogue newDialogue) return;

            _selectedDialogue = newDialogue;
            Repaint();
        }

        /// <summary> Обрабатывает пользовательские события. </summary>
        private void ProcessEvents()
        {
            var currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && !_draggingNode)
            {
                _draggingNode = GetNodeAtPoint(currentEvent.mousePosition + _scrollPosition);
                if (_draggingNode)
                {
                    _draggingOffset = _draggingNode.Rect.position - currentEvent.mousePosition;
                    Selection.activeObject = _draggingNode;
                }
                else
                {
                    _isDraggingCanvas = true;
                    _draggingCanvasOffset = currentEvent.mousePosition + _scrollPosition;
                    Selection.activeObject = _selectedDialogue;
                }
            }
            else if (currentEvent.type == EventType.MouseDrag && currentEvent.button == 0)
            {
                if (_draggingNode)
                {
                    _draggingNode.SetPosition(currentEvent.mousePosition + _draggingOffset);
                    GUI.changed = true;
                }
                else if (_isDraggingCanvas)
                {
                    _scrollPosition = _draggingCanvasOffset - currentEvent.mousePosition;
                    GUI.changed = true;
                }
            }
            else if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                _draggingNode = null;
                _isDraggingCanvas = false;
            }
        }

        /// <summary> Получить узел по координатам. </summary>
        /// <param name="point"> Координаты точки. </param>
        private DialogueNode GetNodeAtPoint(Vector2 point) =>
            _selectedDialogue.Nodes.FirstOrDefault(node => node.Rect.Contains(point));

        /// <summary> Отрисовать холст. </summary>
        private void DrawCanvas()
        {
            const float CanvasSize = 4000f;
            const float BackgroundSize = 64f;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            var canvas = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
            var backgroundTexture = Resources.Load("background") as Texture2D;
            var textureCoords = new Rect(0, 0, CanvasSize / BackgroundSize, CanvasSize / BackgroundSize);
            GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);

            foreach (var node in _selectedDialogue.Nodes) DrawNodeConnections(node);
            foreach (var node in _selectedDialogue.Nodes) DrawNode(node);

            EditorGUILayout.EndScrollView();
        }

        /// <summary> Отрисовать узел. </summary>
        /// <param name="node"> Узел для отрисовки. </param>
        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Rect, node.IsPlayerSpeaking ? _playerNodeStyle : _nodeStyle);

            node.SetText(EditorGUILayout.TextField(node.Text));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create")) _creatingNode = node;
            if (GUILayout.Button("Delete")) _deletingNode = node;
            GUILayout.EndHorizontal();

            DrawLinkButtons(node);
            GUILayout.EndArea();
        }

        /// <summary> Отрисовать кнопки для связывания узлов. </summary>
        /// <param name="node"> Узел для отрисовки. </param>
        private void DrawLinkButtons(DialogueNode node)
        {
            if (!_linkingParentNode)
            {
                if (GUILayout.Button("link")) _linkingParentNode = node;
            }
            else if (_linkingParentNode == node)
            {
                if (GUILayout.Button("cancel")) _linkingParentNode = null;
            }
            else if (_linkingParentNode.ChildNodes.Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    _linkingParentNode.RemoveChild(node.name);
                    _linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    _linkingParentNode.AddChild(node.name);
                    _linkingParentNode = null;
                }
            }
        }

        /// <summary> Отрисовать связи родительского узла с дочерними узлами. </summary>
        /// <param name="node"> Узел для отрисовки. </param>
        private void DrawNodeConnections(DialogueNode node)
        {
            const float ConnectionWidth = 3f;
            var connectionColor = Color.white;
            const float ArrowSize = 15f;
            Vector3 startPosition = new Vector2(node.Rect.xMax, node.Rect.center.y);
            foreach (var childNode in _selectedDialogue.GetChildNodes(node))
            {
                Vector3 endPosition = new Vector2(childNode.Rect.xMin, childNode.Rect.center.y);
                var controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                DrawBezierWithArrow(startPosition, endPosition, startPosition + controlPointOffset,
                    endPosition - controlPointOffset, connectionColor, ConnectionWidth, ArrowSize);
            }
        }

        /// <summary> Нарисовать кривую Безье со стрелкой на конце. </summary>
        private static void DrawBezierWithArrow(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent,
            Color color, float width, float arrowSize)
        {
            Handles.DrawBezier(start, end, startTangent, endTangent, color, null, width);

            // Вычисляем направление в конце кривой
            var direction = (end - endTangent).normalized;

            // Делаем стрелку перпендикулярной линии соединения
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.Euler(0, 0, angle);

            // Вычисляем вершины стрелки
            var arrowLeft = end + rotation * new Vector3(-arrowSize * 0.5f, arrowSize * 0.3f, 0);
            var arrowRight = end + rotation * new Vector3(-arrowSize * 0.5f, -arrowSize * 0.3f, 0);

            // Рисуем треугольную стрелку
            Handles.DrawAAConvexPolygon(end, arrowLeft, arrowRight);
        }

        /// <summary> Обработать создание и удаление узлов. </summary>
        private void HandleNodeCreationDeletion()
        {
            if (_creatingNode)
            {
                _selectedDialogue.CreateNode(_creatingNode);
                _creatingNode = null;
            }

            if (_deletingNode)
            {
                _selectedDialogue.DeleteNode(_deletingNode);
                _deletingNode = null;
            }
        }

        #region Fields

        /// <summary> Текущий выбранный диалог. </summary>
        private Dialogue _selectedDialogue;

        /// <summary> Текущий выбранный диалог. </summary>
        private GUIStyle _nodeStyle;

        /// <summary> Стиль узла для NPC. </summary>
        private GUIStyle _playerNodeStyle;

        /// <summary> Узел, который в данный момент перетаскивается. </summary>
        private DialogueNode _draggingNode;

        /// <summary> Смещение перетаскиваемого узла относительно курсора. </summary>
        private Vector2 _draggingOffset;

        /// <summary> Узел, который создается. </summary>
        private DialogueNode _creatingNode;

        /// <summary> Узел, который создается. </summary>
        private DialogueNode _deletingNode;

        /// <summary> Родительский узел, к которому добавляется связь. </summary>
        private DialogueNode _linkingParentNode;

        /// <summary> Текущая позиция скроллинга редактора. </summary>
        private Vector2 _scrollPosition;

        /// <summary> Происходит ли перетаскивание канваса? </summary>
        private bool _isDraggingCanvas;

        /// <summary> Смещение при перетаскивании канваса. </summary>
        private Vector2 _draggingCanvasOffset;

        #endregion
    }
}