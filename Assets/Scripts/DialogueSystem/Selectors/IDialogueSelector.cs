using FlavorfulStory.AI;

namespace FlavorfulStory.DialogueSystem.Selectors
{
    /// <summary> Интерфейс для селекторов диалогов. </summary>
    public interface IDialogueSelector
    {
        /// <summary> Выбирает подходящий диалог для NPC. </summary>
        /// <param name="npcName"> Имя NPC для выбора диалога. </param>
        /// <returns> Выбранный диалог или null. </returns>
        Dialogue SelectDialogue(NpcName npcName);
    }
}