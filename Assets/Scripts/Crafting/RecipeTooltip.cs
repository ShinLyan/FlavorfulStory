using UnityEngine;
using TMPro;

namespace FlavorfulStory.Crafting
{
    /// <summary> Визуальное представление тултипа рецепта. </summary>
    public class RecipeTooltip : MonoBehaviour
    {
        /// <summary> Текстовое поле заголовка рецепта. </summary>
        [SerializeField] private TMP_Text _titleText;
        /// <summary> Текстовое поле описания рецепта. </summary>
        [SerializeField] private TMP_Text _descriptionText;

        /// <summary> Настроить содержимое тултипа. </summary>
        public void Setup(string title, string description)
        {
            _titleText.text = title;
            _descriptionText.text = description;
        }
    }
}