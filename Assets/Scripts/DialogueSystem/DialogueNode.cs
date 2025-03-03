using System;
using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    [Serializable]
    public class DialogueNode
    {
        [SerializeField] private string _uniqueId;
        [SerializeField] private string _text;
        [SerializeField] private string[] _children;
    }
}