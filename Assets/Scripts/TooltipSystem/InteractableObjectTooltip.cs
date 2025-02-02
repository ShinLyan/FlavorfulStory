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

        /// <summary> Устанавливает заголовок и описание тултипа на основе данных из переданного объекта. </summary>
        /// <param name="tooltipable"> Объект, предоставляющий данные для тултипа. </param>
        public void SetTitleAndDescription(ITooltipable tooltipable)
        {
            _title.text = tooltipable.GetTooltipTitle();
            _description.text = tooltipable.GetTooltipDescription();
        }

        /// <summary> Устанавливает позицию тултипа с учетом смещения. </summary>
        /// <param name="tooltipable"> Объект, предоставляющий позицию для тултипа. </param>
        public void SetPositionWithOffset(ITooltipable tooltipable)
        {
            transform.position = tooltipable.GetWorldPosition() + _offset;
        }
    }
}