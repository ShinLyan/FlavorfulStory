namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Интерфейс для объектов, способных инициировать диалог. </summary>
    public interface IDialogueInitiator
    {
        /// <summary> Запускает диалог с указанным NPC. </summary>
        /// <param name="npc"> Персонаж, с которым начинается диалог. </param>
        /// <param name="dialogue"> Диалог, который необходимо воспроизвести. </param>
        void StartDialogue(NpcSpeaker npc, Dialogue dialogue);

        /// <summary> Завершает активный диалог. </summary>
        void EndDialogue();
    }
}