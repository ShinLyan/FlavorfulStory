using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlavorfulStory.InventorySystem.PickupSystem
{
    public class PickupNotificationView : BaseNotificationView
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _icon;

        public override float Height => RectTransform.rect.height;

        public override void Initialize(INotificationData data)
        {
            if (data is not PickupNotificationData d) return;
            _label.text = $"x{d.Amount} {d.ItemName}";
            _icon.sprite = d.Icon;
        }
    }
}