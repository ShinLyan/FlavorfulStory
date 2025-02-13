using System.Collections.Generic;
using FlavorfulStory.AI.Scheduling;
using UnityEngine;

namespace FlavorfulStory.AI
{
    public class NpcController : MonoBehaviour
    {
        [SerializeField] private NPC[] _npcs;
        
        [SerializeField] private NpcSchedule[] _schedules;
        
        private static Dictionary<NPC, NpcSchedule> _npcSchedules;
        

        private void Awake()
        {
            if (_npcSchedules == null)
                _npcSchedules = CreateDictOfNpcSchedules(_npcs, _schedules);
        }

        private Dictionary<NPC, NpcSchedule> CreateDictOfNpcSchedules(NPC[] npcs, NpcSchedule[] schedules)
        {
            Dictionary<NPC, NpcSchedule> dict = new Dictionary<NPC, NpcSchedule>();
            foreach (var npc in npcs)
            {
                foreach (var schedule in schedules)
                {
                    if (!dict.ContainsKey(npc) && npc.Name == schedule.Params[0].NpcName)
                    {
                        dict.Add(npc, schedule);
                        break;
                    }
                }
            }
            return dict;
        }

        private void CalculateCurrentScene()
        {
            
        }
    }
}