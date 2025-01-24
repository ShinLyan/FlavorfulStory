using UnityEngine;

public interface ITooltipable
{
    
    public string GetTooltipTitle();

    public string GetTooltipDescription();
    
    public Vector3 GetWorldPosition();
}