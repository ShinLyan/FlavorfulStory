using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory
{
    [CreateAssetMenu(menuName = "FlavorfulStory/Destruction/Effect Profile", fileName = "DestroyEffectProfile")]
    public class DestroyEffectProfile : ScriptableObject
    {
        //TODO: Назначить в инспекторе SO для деревьев и камней.
        public GameObject[] particlePrefabs;
        public float particleLifetime = 2f;
        public Vector3 particleOffset;
        public bool detachParticles = true;

        [Tooltip("Тип звука для разрушения.")]
        public SfxType destructionSfx;
    }
}