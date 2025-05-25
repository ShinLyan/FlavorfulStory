using FlavorfulStory.Audio;
using UnityEngine;

namespace FlavorfulStory
{
    public class DestructionEffectsPlayer : MonoBehaviour
    {
        [SerializeField] private DestroyEffectProfile _effectProfile;

        public void PlayEffects()
        {
            if (_effectProfile == null) return;
            
            foreach (var prefab in _effectProfile.particlePrefabs)
            {
                var instance = Instantiate(
                    prefab,
                    transform.position + _effectProfile.particleOffset,
                    Quaternion.identity);

                if (_effectProfile.detachParticles)
                    instance.transform.parent = null;

                Destroy(instance, _effectProfile.particleLifetime);
            }
            
            SfxPlayer.Instance.PlayOneShot(_effectProfile.destructionSfx);
        }
    }
}