using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    /// <summary> Автоматически скрывает скроллбар, если контент полностью помещается в окне. </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollbarAutoHider : MonoBehaviour
    {
        /// <summary> Скроллбар, который нужно скрывать или показывать. </summary>
        [SerializeField] private Scrollbar _scrollbar;
        /// <summary> ScrollRect, к которому относится скроллбар. </summary>
        private ScrollRect _scrollRect;
        
        /// <summary> Получает компонент ScrollRect на объекте. </summary>
        private void Awake() => _scrollRect = GetComponent<ScrollRect>();

        /// <summary> Подписывается на изменение значения скроллбара. </summary>
        private void Start() => _scrollbar.onValueChanged.AddListener((_) => CheckScrollbarForHide());

        /// <summary> Проверяет, нужно ли скрывать скроллбар в зависимости от его размера. </summary>
        private void CheckScrollbarForHide()
        {
            if (_scrollbar == null || _scrollRect == null) return;
            _scrollbar.gameObject.SetActive(_scrollbar.size >= 1f);
        }
    }
}