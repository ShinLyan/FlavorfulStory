using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI.Bars
{
    /// <summary> Базовый класс для полос прогресса и индикаторов. </summary>
    public class BaseBar : MonoBehaviour
    {
        /// <summary> Текстовый элемент, отображающий значение бара. </summary>
        [SerializeField] protected TMP_Text _textObject;

        /// <summary> Устанавливает текстовое значение для отображения на баре. </summary>
        /// <param name="value"> Текущее значение параметра. </param>
        protected void SetBarText(float value)
        {
            _textObject.text = value.ToString();
        }
    }
}