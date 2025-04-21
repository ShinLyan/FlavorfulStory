using TMPro;
using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Отвечает за отображение тултипа для взаимодействуемых объектов. </summary>
    public class InteractableObjectTooltip : MonoBehaviour
    {
        /// <summary> Поле для отображения заголовка тултипа. </summary>
        [SerializeField] private TMP_Text _title;

        /// <summary> Поле для отображения описания тултипа. </summary>
        [SerializeField] private TMP_Text _description;

        /// <summary> Смещение позиции тултипа относительно объекта. </summary>
        [SerializeField] private Vector3 _offset;

        /// <summary> Инициализация тултипа. Сокрытие окна при старте игры. </summary>
        private void Start() => Hide();

        /// <summary> Показать тултип. </summary>
        /// <param name="tooltip"> Объект с тултипом. </param>
        public void Show(ITooltipable tooltip)
        {
            gameObject.SetActive(true);
            SetTitleAndDescription(tooltip);
            SetPositionWithOffset(tooltip);
        }

        /// <summary> Устанавливает заголовок и описание тултипа на основе данных из переданного объекта. </summary>
        /// <param name="tooltip"> Объект, предоставляющий данные для тултипа. </param>
        private void SetTitleAndDescription(ITooltipable tooltip)
        {
            _title.text = tooltip.TooltipTitle;
            _description.text = tooltip.TooltipDescription;
        }

        /// <summary> Устанавливает позицию тултипа с учетом смещения. </summary>
        /// <param name="tooltip"> Объект, предоставляющий позицию для тултипа. </param>
        private void SetPositionWithOffset(ITooltipable tooltip) =>
            transform.position = tooltip.WorldPosition + _offset;

        /// <summary> Скрыть тултип. </summary>
        public void Hide() => gameObject.SetActive(false);
    }
}