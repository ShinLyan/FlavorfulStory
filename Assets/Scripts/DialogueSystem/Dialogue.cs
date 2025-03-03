using UnityEngine;

namespace FlavorfulStory.DialogueSystem
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private DialogueNode[] _nodes;
    }
}