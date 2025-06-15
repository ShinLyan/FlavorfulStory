using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace FlavorfulStory.AI
{
    /// <summary> Фабрика для создания экземпляров NpcNavigator. </summary>
    public class NpcNavigatorFactory : PlaceholderFactory<NavMeshAgent, Transform, MonoBehaviour, NpcNavigator>
    {
    }
}