using TMPro;
using UnityEngine;

namespace FlavorfulStory.TooltipSystem
{
    /// <summary> Отображает одну строку действия с клавишей и описанием в тултипе. </summary>
    public class TooltipActionView : MonoBehaviour
    {
        /// <summary> Поле для отображения клавиши действия. </summary>
        [SerializeField] private TMP_Text _key;

        /// <summary> Поле для отображения текста подсказки. </summary>
        [SerializeField] private TMP_Text _tooltipMessage;

        /// <summary> Устанавливает текст клавиши и подсказки. </summary>
        public void Setup(string key, string tooltipMessage)
        {
            _key.text = key;
            _tooltipMessage.text = tooltipMessage;
        }

        /// <summary> Показать объект. </summary>
        public void Show() => gameObject.SetActive(true);

        /// <summary> Скрыть объект. </summary>
        public void Hide() => gameObject.SetActive(false);
    }
}