using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Узел диалога, содержащий текст, связи с дочерними узлами и параметры отображения. </summary>
    public class DialogueNode : ScriptableObject
    {
        /// <summary> Говорит ли игрок в данном узле диалога? </summary>
        [field: SerializeField] public bool IsPlayerSpeaking { get; private set; }

        /// <summary> Текст, отображаемый в узле диалога. </summary>
        [field: SerializeField, TextArea(3, 5)]
        public string Text { get; private set; } = "Dialogue Text";

        /// <summary> Список идентификаторов дочерних узлов. </summary>
        [field: SerializeField, HideInInspector]
        public List<string> ChildNodes { get; private set; } = new();

        /// <summary> Позиция и размер узла в редакторе. </summary>
        [field: SerializeField] public Rect Rect { get; private set; } = new(0, 0, 200, 100);

        /// <summary> Имя действия, вызываемого при входе в этот узел. </summary>
        [field: SerializeField] public string EnterActionName { get; private set; }

        /// <summary> Имя действия, вызываемого при выходе из этого узла. </summary>
        [field: SerializeField] public string ExitActionName { get; private set; }

#if UNITY_EDITOR
        /// <summary> Установить новую позицию узла в редакторе. </summary>
        /// <param name="position"> Новая позиция узла. </param>
        public void SetPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Move Dialogue Node");

            var rect = Rect;
            rect.position = position;
            Rect = rect;

            EditorUtility.SetDirty(this);
        }

        /// <summary> Установить текст узла. </summary>
        /// <param name="text"> Новый текст диалога. </param>
        public void SetText(string text)
        {
            if (text == Text) return;

            Undo.RecordObject(this, "Update Dialogue Text");
            Text = text;
            EditorUtility.SetDirty(this);
        }

        /// <summary> Добавить дочерний узел по его идентификатору. </summary>
        /// <param name="childId"> Идентификатор дочернего узла. </param>
        public void AddChild(string childId)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            ChildNodes.Add(childId);
            EditorUtility.SetDirty(this);
        }

        /// <summary> Удалить дочерний узел по его идентификатору. </summary>
        /// <param name="childId"> Идентификатор дочернего узла. </param>
        public void RemoveChild(string childId)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            ChildNodes.Remove(childId);
            EditorUtility.SetDirty(this);
        }

        /// <summary> Установить, говорит ли в этом узле игрок. </summary>
        /// <param name="isPlayerSpeaking"> `true`, если говорит игрок; иначе `false`. </param>
        public void SetPlayerSpeaking(bool isPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            IsPlayerSpeaking = isPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}