using FlavorfulStory.Saving;
using UnityEngine;
using Zenject;

namespace FlavorfulStory
{
    public class ItemDropServiceSaver : MonoBehaviour, ISaveable
    {
        private ISaveable _dropService;
        
        [Inject]
        private void Construct(IItemDropService dropService)
        {
            _dropService = dropService as ISaveable;
        }

        public object CaptureState() => _dropService?.CaptureState();

        public void RestoreState(object state) => _dropService?.RestoreState(state);
    }
}