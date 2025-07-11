using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.UI
{
    public class CraftRequirementView : MonoBehaviour
    {
        [SerializeField] private Color _requirementAchievedTextColor;
        [SerializeField] private Color _requirementFailedTextColor;
        
        [SerializeField] private Image _itemImage;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _requirementText;

        public void Setup(Sprite sprite, string itemName, string requirementText, bool requirementAchieved)
        {
            _itemImage.sprite = sprite;
            _itemName.text = itemName;
            _requirementText.color =requirementAchieved ? _requirementAchievedTextColor : _requirementFailedTextColor;
            _requirementText.text = requirementText;
        }
    }
}