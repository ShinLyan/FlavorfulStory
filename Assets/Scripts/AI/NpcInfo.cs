using FlavorfulStory.DialogueSystem;
using UnityEngine;

namespace FlavorfulStory.AI
{
    /// <summary> Информация о NPC. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/NPC/Info")]
    public class NpcInfo : ScriptableObject
    {
        /// <summary> Имя NPC. </summary>
        [field: Tooltip("Имя NPC."), SerializeField]
        public NpcName NpcName { get; private set; }

        /// <summary> Доступен ли NPC для романтических отношений? </summary>
        [field: Tooltip("Доступен ли NPC для романтических отношений?"), SerializeField]
        public bool IsRomanceable { get; private set; }

        /// <summary> Префаб персонажа в диалоге. </summary>
        [field: Tooltip("Префаб персонажа в диалоге."), SerializeField]
        public GameObject DialogueModelPrefab { get; private set; }

        /// <summary> Иконка NPC для отображения в UI. </summary>
        [field: Tooltip("Иконка NPC для отображения в UI."), SerializeField]
        public Sprite Icon { get; private set; }

        /// <summary> Конфигурация диалога, содержащая приветственные и условные диалоги NPC. </summary>
        [field: SerializeField]
        public DialogueConfig DialogueConfig { get; private set; }
    }
}