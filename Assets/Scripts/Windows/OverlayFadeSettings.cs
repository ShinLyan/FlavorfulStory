using DG.Tweening;
using UnityEngine;

namespace FlavorfulStory.Windows
{
    /// <summary> Настройки фейдов HUD и фонового затемнения. </summary>
    [CreateAssetMenu(menuName = "FlavorfulStory/StaticData/UI/OverlayFadeSettings")]
    public class OverlayFadeSettings : ScriptableObject
    {
        /// <summary> Длительность появления HUD при закрытии всех окон. </summary>
        [field: Header("HUD (исчезает при первом окне)")]
        [field: SerializeField, Min(0f), Tooltip("Длительность появления HUD при закрытии всех окон.")]
        public float HudFadeInDuration { get; private set; }

        /// <summary> Длительность скрытия HUD при открытии первого окна. </summary>
        [field: SerializeField, Min(0f), Tooltip("Длительность скрытия HUD при открытии первого окна.")]
        public float HudFadeOutDuration { get; private set; }

        /// <summary> Максимальная альфа-прозрачность HUD. </summary>
        [field: SerializeField, Range(0f, 1f), Tooltip("Максимальная прозрачность HUD.")]
        public float HudMaxAlpha { get; private set; }

        /// <summary> Easing-кривая для HUD. </summary>
        [field: SerializeField, Tooltip("Easing для анимации HUD.")]
        public Ease HudEase { get; private set; }

        /// <summary> Длительность появления затемнения при открытии первого окна. </summary>
        [field: Header("Background (появляется при первом окне)")]
        [field: SerializeField, Min(0f), Tooltip("Длительность появления фона при первом окне.")]
        public float BackgroundFadeInDuration { get; private set; }

        /// <summary> Длительность исчезновения затемнения при закрытии последнего окна. </summary>
        [field: SerializeField, Min(0f), Tooltip("Длительность скрытия фона при закрытии всех окон.")]
        public float BackgroundFadeOutDuration { get; private set; }

        /// <summary> Максимальная альфа-прозрачность затемнения. </summary>
        [field: SerializeField, Range(0f, 1f), Tooltip("Максимальная прозрачность фонового затемнения.")]
        public float BackgroundMaxAlpha { get; private set; }

        /// <summary> Easing-кривая для фона. </summary>
        [field: SerializeField, Tooltip("Easing для анимации фонового затемнения.")]
        public Ease BackgroundEase { get; private set; }

        /// <summary> Блокирует ли фон клики мыши при затемнении. </summary>
        [field: SerializeField, Tooltip("Блокирует ли фон Raycasts во время затемнения.")]
        public bool BackgroundBlocksRaycasts { get; private set; }
    }
}