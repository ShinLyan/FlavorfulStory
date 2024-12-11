using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    [RequireComponent(typeof(Image))]
    public class TabSwitherButton : MonoBehaviour,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _selectedSprite;

        [SerializeField] private UnityEvent _onSelected;
        [SerializeField] private UISwitcher _switcher;

        private int _index;

        private Image _image;

        private bool _isSelected;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _index = transform.GetSiblingIndex();
        }

        public void Select()
        {
            _isSelected = true;
            _image.sprite = _selectedSprite;
            _onSelected?.Invoke();
        }

        public void ResetSelection()
        {
            _isSelected = false;
            _image.sprite = _defaultSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _switcher.SelectTab(_index);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.sprite = _selectedSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isSelected) _image.sprite = _defaultSprite;
        }
    }
}
