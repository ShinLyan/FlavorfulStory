using UnityEngine;

namespace FlavorfulStory.QuestSystem
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Quest")]
    public class Quest : ScriptableObject
    {
        [field: SerializeField] public string QuestName { get; private set; }

        [field: SerializeField] public string QuestDescription { get; private set; }

        [field: SerializeField] public string QuestType { get; private set; }

        [field: SerializeField] public string[] Objectives { get; private set; }
    }
}