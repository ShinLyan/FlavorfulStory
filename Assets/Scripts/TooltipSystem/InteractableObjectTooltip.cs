using FlavorfulStory.InteractionSystem;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Отвечает за отображение тултипа для взаимодействуемых объектов. </summary>
    public class InteractableObjectTooltip : MonoBehaviour, IActionTooltipShower
    {
        /// <summary> Поле для отображения заголовка тултипа. </summary>
        [SerializeField] private TMP_Text _actionDescription;

        /// <summary> Инициализация тултипа. Сокрытие окна при старте игры. </summary>
        private void Start() => gameObject.SetActive(false);

        /// <summary> Показать список возможных действий с объектом. </summary>
        /// <param name="interactable"> Объект взаимодействия. </param>
        public void Show(IInteractable interactable)
        {
            gameObject.SetActive(true);
            _actionDescription.text = CreateActionDescription(interactable);
        }

        /// <summary> Скрыть тултип. </summary>
        public void Hide() => gameObject.SetActive(false);
        
        /// <summary> Устанавливает заголовок и описание тултипа на основе данных из переданного объекта. </summary>
        /// <param name="tooltip"> Объект, предоставляющий данные для тултипа. </param>
        private string CreateActionDescription(ITooltipableAction tooltip) => $"{tooltip.ActionDescription.Action} {tooltip.ActionDescription.Target}";
    }
}