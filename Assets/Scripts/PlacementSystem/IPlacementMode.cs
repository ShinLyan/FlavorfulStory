using UnityEngine;

namespace FlavorfulStory.PlacementSystem
{
    public interface IPlacementMode
    {
        void Enter();

        void Exit();

        void Apply(Vector3Int gridPosition);

        void Refresh(Vector3Int gridPosition);
    }
}