using UnityEngine;

namespace FlavorfulStory.AI.States
{
    /// <summary> Информация о NPC. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/NPC Info")]
    public class NpcInfo : ScriptableObject
    {
        /// <summary> Имя NPC (используется в игровых системах и интерфейсе). </summary>
        [field: Tooltip("Имя NPC."), SerializeField]
        public NpcName NpcName { get; private set; }

        /// <summary> Доступен ли NPC для романтических отношений. </summary>
        [field: Tooltip("Доступен ли NPC для романтических отношений."), SerializeField]
        public bool IsRomanceable { get; private set; }

        /// <summary> Объект предпросмотра диалога (может отображаться в UI или для отладки). </summary>
        [field: Tooltip("Объект предпросмотра диалога (например, иконка или модель для UI)."), SerializeField]
        public GameObject DialoguePreviewObject { get; private set; }

        /// <summary> Иконка NPC для отображения в UI. </summary>
        [field: Tooltip("Иконка NPC для отображения в пользовательском интерфейсе (например, на мини-карте)."),
                SerializeField]
        public Sprite Icon { get; private set; }
    }
}