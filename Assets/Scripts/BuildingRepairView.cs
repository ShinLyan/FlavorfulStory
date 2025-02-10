using FlavorfulStory.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class BuildingRepairView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private RectTransform _mainWindowContent;

    public bool IsOpen { get; private set; }
    
    public void Open()
    {
        IsOpen = true;
        _background.gameObject.SetActive(IsOpen);
        _mainWindowContent.gameObject.SetActive(IsOpen);
        InputWrapper.BlockAllInput();
    }

    public void Close()
    {
        IsOpen = false;
        _background.gameObject.SetActive(IsOpen);
        _mainWindowContent.gameObject.SetActive(IsOpen);
        InputWrapper.UnblockAllInput();
    }
}