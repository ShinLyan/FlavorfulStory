using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI
{
    public class NpcMovementControllerFactory : PlaceholderFactory<NavMeshAgent,
        Transform,
        NpcAnimatorController,
        NpcScheduleHandler,
        MonoBehaviour,
        NpcMovementController>
    {
    }
}