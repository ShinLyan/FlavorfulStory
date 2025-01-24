using TMPro;
using UnityEngine;

public class InteractableObjectToolTip : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    
    [SerializeField] private TMP_Text _description;

    [SerializeField] private Vector3 _offset;
    
    public void SetTitleAndDescription(ITooltipable tooltipable)
    {
        _title.text = tooltipable.GetTooltipTitle();
        _description.text = tooltipable.GetTooltipDescription();
        
        // _title.text = title;
        // _description.text = description;
    }

    public void SetPositionWithOffset(ITooltipable tooltipable)
    {
        transform.position = tooltipable.GetWorldPosition() + _offset;
        //transform.position = worldPosition + _offset;
    }
}