using FlavorfulStory.AI.States;
using UnityEngine;

namespace FlavorfulStory.AI
{
    public class Npc : MonoBehaviour
    {
        [field: SerializeField] public NpcInfo NpcInfo { get; private set; }
    }
}