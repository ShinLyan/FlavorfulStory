using FlavorfulStory.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class BuildingRepairView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private RectTransform _mainWindowContent;
    
    public void Open()
    {
        _background.gameObject.SetActive(true);
        _mainWindowContent.gameObject.SetActive(true);
        InputWrapper.BlockAllInput();
    }

    public void Close()
    {
        _background.gameObject.SetActive(false);
        _mainWindowContent.gameObject.SetActive(false);
        InputWrapper.UnblockAllInput();
    }
}