using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.UI
{
    /// <summary> Слот панели инструментов для отображения предметов инвентаря. </summary>
    [RequireComponent(typeof(Image))]
    public class ToolbarSlotUI : MonoBehaviour, IItemHolder,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> Отображение иконки предмета в слоте. </summary>
        [SerializeField] private InventoryItemIcon _icon;

        /// <summary> Текст с номером слота. </summary>
        [SerializeField] private TMP_Text _keyText;

        /// <summary> Индекс текущего слота на панели. </summary>
        private int _index;

        /// <summary> Панель инструментов, которой принадлежит слот. </summary>
        private Toolbar _toolbar;

        /// <summary> Выбран ли слот? </summary>
        private bool _isSelected;

        /// <summary> Инициализация индекса слота, текста номера и панели инструментов. </summary>
        private void Awake()
        {
            _index = transform.GetSiblingIndex();
            _keyText.text = $"{_index + 1}";
            _toolbar = transform.parent.GetComponent<Toolbar>();
        }

        /// <summary> Обновление содержимого слота на основе текущего состояния инвентаря. </summary>
        public void Redraw()
        {
            _icon.SetItem(Inventory.PlayerInventory.GetItemInSlot(_index), 
                Inventory.PlayerInventory.GetNumberInSlot(_index));
        }

        /// <summary> Получение предмета, находящегося в текущем слоте. </summary>
        /// <returns> Предмет, находящийся в текущем слоте. </returns>
        public InventoryItem GetItem() => Inventory.PlayerInventory.GetItemInSlot(_index);

        /// <summary> Установка состояния слота как выбранного. </summary>
        public void Select()
        {
            _isSelected = true;
            FadeToColor(Color.white);
        }

        /// <summary> Сброс состояния слота как выбранного. </summary>
        public void ResetSelection()
        {
            _isSelected = false;
            FadeToColor(Color.gray);
        }
        
        /// <summary> Изменение цвета слота с анимацией. </summary>
        /// <param name="color"> Новый цвет слота. </param>
        private void FadeToColor(Color color)
        {
            const float FadeDuration = 0.2f;
            GetComponent<Image>().CrossFadeColor(color, FadeDuration, true, true);
        }

        /// <summary> Обработка клика по слоту. </summary>
        /// <param name="eventData"> Данные события клика. </param>
        public void OnPointerClick(PointerEventData eventData)
        {
            _toolbar.SelectItem(_index);
        }

        /// <summary> Обработка наведения курсора на слот. </summary>
        /// <param name="eventData"> Данные события наведения. </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            FadeToColor(Color.white);
        }

        /// <summary> Обработка выхода курсора из слота. </summary>
        /// <param name="eventData"> Данные события выхода. </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isSelected) FadeToColor(Color.gray);
        }
    }
}