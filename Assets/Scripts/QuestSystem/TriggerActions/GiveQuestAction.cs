using UnityEngine;

namespace FlavorfulStory.QuestSystem.TriggerActions
{
    /// <summary> Действие, выдающее игроку указанный квест. </summary>
    public class GiveQuestAction : QuestTriggerAction
    {
        /// <summary> Квест, который нужно выдать. </summary>
        [SerializeField] private Quest _questToGive;

        /// <summary> Выполняет действие — выдаёт квест, если он ещё не выдан. </summary>
        public override void Execute(QuestExecutionContext context)
        {
            var questList = context.QuestList;
            if (_questToGive && !questList.HasQuest(_questToGive)) questList.AddQuest(_questToGive);
        }
    }
}