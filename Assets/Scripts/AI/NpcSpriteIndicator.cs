using UnityEngine;

namespace FlavorfulStory.AI
{
    public class NpcSpriteIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject _sprite;

        private void Start()
        {
            if (_sprite == null) Debug.LogError("Sprite indicator needs to be assigned");
        }

        /// <summary> Показать спрайт. </summary>
        public void ShowSprite() => SetSpriteVisible(true);

        /// <summary>  Скрыть лампочку. </summary>
        public void HideSprite() => SetSpriteVisible(false);

        /// <summary> Установить состояние лампочки. </summary>
        private void SetSpriteVisible(bool visible) => _sprite.SetActive(visible);
    }
}