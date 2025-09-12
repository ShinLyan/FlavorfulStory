using FlavorfulStory.AI;

namespace FlavorfulStory.DialogueSystem
{
    /// <summary> Интерфейс сервиса, предоставляющего подходящий диалог для NPC. </summary>
    public interface IDialogueService
    {
        /// <summary> Возвращает наиболее подходящий диалог для заданного NPC. </summary>
        /// <param name="npcInfo"> Информация об NPC, для которого подбирается диалог. </param>
        /// <returns> Найденный диалог или null, если подходящего не найдено. </returns>
        Dialogue GetDialogue(NpcInfo npcInfo);
    }
}