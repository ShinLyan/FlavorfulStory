using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> ScriptableObject, представляющий диалог. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary> Узлы диалога. </summary>
        [SerializeField] private List<DialogueNode> _nodes = new();

        /// <summary> Словарь для быстрого поиска узлов по идентификатору. </summary>
        private readonly Dictionary<string, DialogueNode> _nodeLookup = new();

        /// <summary> Все узлы диалога. </summary>
        public IEnumerable<DialogueNode> Nodes => _nodes;

        /// <summary>
        /// 
        /// </summary>
        public DialogueNode RootNode => _nodes[0];

        /// <summary> Получить дочерние узлы указанного узла. </summary>
        /// <param name="parentNode"> Родительский узел. </param>
        /// <returns> Перечисление дочерних узлов. </returns>
        public IEnumerable<DialogueNode> GetChildNodes(DialogueNode parentNode)
        {
            foreach (string childId in parentNode.ChildNodes)
                if (_nodeLookup.TryGetValue(childId, out var childNode))
                    yield return childNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        public IEnumerable<DialogueNode> GetPlayerChildNodes(DialogueNode currentNode) =>
            GetChildNodes(currentNode).Where(node => node.IsPlayerSpeaking);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        public IEnumerable<DialogueNode> GetAIChildNodes(DialogueNode currentNode) =>
            GetChildNodes(currentNode).Where(node => !node.IsPlayerSpeaking);

        /// <summary> Пересоздать словарь узлов при изменении ScriptableObject в редакторе. </summary>
        private void OnValidate() => RebuildNodeLookup();

        /// <summary> Пересоздать словарь узлов. </summary>
        private void RebuildNodeLookup()
        {
            _nodeLookup.Clear();
            foreach (var node in Nodes)
                if (node)
                    _nodeLookup[node.name] = node;
        }

        /// <summary> Действия перед сериализацией объекта. </summary>
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (_nodes.Count == 0) AddNode(CreateDialogueNode(null));

            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this))) return;

            foreach (var node in Nodes)
                if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(node)))
                    AssetDatabase.AddObjectToAsset(node, this);
#endif
        }

        /// <summary> Действия после десериализации объекта. </summary>
        public void OnAfterDeserialize()
        {
        }

#if UNITY_EDITOR
        /// <summary> Создать новый узел и добавить его к родителю. </summary>
        /// <param name="parentNode"> Родительский узел. </param>
        public void CreateNode(DialogueNode parentNode)
        {
            var newNode = CreateDialogueNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        /// <summary> Создать новый узел. </summary>
        /// <param name="parentNode"> Родительский узел. </param>
        /// <returns> Новый узел диалога. </returns>
        private static DialogueNode CreateDialogueNode(DialogueNode parentNode)
        {
            var newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (!parentNode) return newNode;

            parentNode.AddChild(newNode.name);
            newNode.SetPlayerSpeaking(!parentNode.IsPlayerSpeaking);

            var newNodeOffset = new Vector2(250f, (parentNode.ChildNodes.Count - 1) * 100f);
            newNode.SetPosition(parentNode.Rect.position + newNodeOffset);

            return newNode;
        }

        /// <summary> Добавить узел в список и обновить словарь. </summary>
        /// <param name="newNode"> Новый узел. </param>
        private void AddNode(DialogueNode newNode)
        {
            _nodes.Add(newNode);
            RebuildNodeLookup();
        }

        /// <summary> Удалить узел и обновить связи. </summary>
        /// <param name="nodeToDelete"> Удаляемый узел. </param>
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            _nodes.Remove(nodeToDelete);
            RebuildNodeLookup();
            RemoveDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        /// <summary> Удалить связи удаленного узла с другими узлами. </summary>
        /// <param name="nodeToDelete"> Удаляемый узел. </param>
        private void RemoveDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in Nodes) node.RemoveChild(nodeToDelete.name);
        }
#endif
    }
}