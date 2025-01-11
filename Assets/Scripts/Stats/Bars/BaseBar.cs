using System;
using TMPro;
using UnityEngine;

namespace FlavorfulStory.UI.Bars
{
    /// <summary> Базовый класс для баров. </summary>
    public class BaseBar : MonoBehaviour
    {
        /// <summary> Объект текста. </summary>
        [SerializeField] protected TMP_Text _textObject;

        /// <summary> Установка текстового значения. </summary>
        /// <param name="value"> Текущее значение параметра. </param>
        protected void SetBarText(float value)
        {
            _textObject.text = value.ToString();
        }
    }
}
