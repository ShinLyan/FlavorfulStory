using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    [RequireComponent(typeof(Image))]
    public class ToolbarSlotUI : MonoBehaviour, IItemHolder,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> ������ ����� ���������. </summary>
        [SerializeField] private InventoryItemIcon _icon;

        /// <summary> ����� �������. </summary>
        [SerializeField] private TMP_Text _keyText;

        /// <summary> Изображение обводки тулбар слота. </summary>
        [SerializeField] private Image _hoverImage;

        /// <summary> ������ ����� � HUD. </summary>
        private int _index;

        /// <summary>
        /// 
        /// </summary>
        private Toolbar _toolbar;

        /// <summary>
        /// 
        /// </summary>
        private bool _isSelected;

        /// <summary> Флаг нахождение курсора на тулбар слоте. Типо как Hover. </summary>
        private bool _isMouseOver;

        private void Awake()
        {
            _index = transform.GetSiblingIndex();
            _keyText.text = $"{_index + 1}";
            _toolbar = transform.parent.GetComponent<Toolbar>();
        }

        public void Redraw()
        {
            _icon.SetItem(Inventory.PlayerInventory.GetItemInSlot(_index), 
                Inventory.PlayerInventory.GetNumberInSlot(_index));
        }

        /// <summary> �������� �������, ������� � ������ ������ ��������� � ���� ���������. </summary>
        /// <returns> ���������� �������, ������� � ������ ������ ��������� � ���� ���������. </returns>
        public InventoryItem GetItem() => Inventory.PlayerInventory.GetItemInSlot(_index);

        public void Select()
        {
            _isSelected = true;
            FadeToColor(Color.white);
            HoverStart();
        }

        public void ResetSelection()
        {
            _isSelected = false;
            FadeToColor(Color.gray);
            if (!_isMouseOver) HoverEnd();
        }
        private void FadeToColor(Color color)
        {
            const float FadeDuration = 0.2f;
            GetComponent<Image>().CrossFadeColor(color, FadeDuration, true, true);
        }

        /// <summary> Наведение курсора на тулбар слот. Плавно проявляет рамку тулбар слота. </summary>
        private void HoverStart()
        {
            //FadeToColor(Color.white);
            const float FadeDuration = 0.2f;
            _hoverImage.CrossFadeAlpha(1.0f, FadeDuration, true);
        }

        /// <summary> Уведение курсора с тулбар слота. Плавно убирает рамку с тулбар слота. </summary>
        private void HoverEnd()
        {
            //FadeToColor(Color.gray);
            const float FadeDuration = 0.2f;
            _hoverImage.CrossFadeAlpha(0.0f, FadeDuration, true);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _toolbar.SelectItem(_index);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseOver = true;
            HoverStart();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver = false;
            if (!_isSelected) HoverEnd();
        }
    }
}