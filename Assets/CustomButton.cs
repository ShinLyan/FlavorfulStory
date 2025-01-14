using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected Image ButtonImage { get; private set; }
    protected bool IsMouseOver { get; private set; }

    protected virtual void Awake()
    {
        ButtonImage = GetComponent<Image>();
        Initialize();
    }

    protected abstract void Initialize();
    
    protected abstract void HoverStart();
    
    protected abstract void HoverEnd();
    
    protected abstract void Click();

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseOver = true;
        HoverStart();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOver = false;
        HoverEnd();
    }
    
    // private void OnDisable()
    // {
    //     IsMouseOver = false;
    // }
    //
    // private void OnEnable()
    // {
    //     IsMouseOver = false;
    // }
}