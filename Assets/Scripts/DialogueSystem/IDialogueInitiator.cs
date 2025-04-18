namespace FlavorfulStory.DialogueSystem
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDialogueInitiator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="dialogue"></param>
        void StartDialogue(NpcSpeaker npc, Dialogue dialogue);

        /// <summary>
        /// 
        /// </summary>
        void EndDialogue();
    }
}