using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory
{
    /// <summary> Настройки фейдов HUD и фонового затемнения. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/UI/OverlayFadeSettings")]
    public class OverlayFadeSettings : ScriptableObject
    {
        [Header("HUD (исчезает при первом окне)")]
        [field: SerializeField, Min(0f)] public float HudFadeInDuration { get; private set; }
        [field: SerializeField, Min(0f)] public float HudFadeOutDuration { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float HudMaxAlpha { get; private set; }
        [field: SerializeField] public Ease HudEase { get; private set; }

        [Header("Background (появляется при первом окне)")]
        [field: SerializeField, Min(0f)] public float BackgroundFadeInDuration { get; private set; }
        [field: SerializeField, Min(0f)] public float BackgroundFadeOutDuration { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float BackgroundMaxAlpha { get; private set; }
        [field: SerializeField] public Ease BackgroundEase { get; private set; }
        
        [field: SerializeField] public bool BackgroundBlocksRaycasts { get; private set; }
    }
}