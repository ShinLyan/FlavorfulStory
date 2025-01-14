using System;
using TMPro;
using UnityEngine;

public class TabButton : CustomButton
{
    [SerializeField] private TMP_Text _nameText;
    
    /// <summary> Цвет текста выбранной вкладки. </summary>
    [SerializeField] private Color _activeNameColor; // = new(1, 0.8566375f, 0.6745283f)

    /// <summary> Цвет текста вкладки по умолчанию. </summary>
    [SerializeField] private Color _defaultNameColor;// = Color.white
    
    public event Action OnClick;
    
    protected override void Initialize()
    {
        SetNameState(false);
    }

    protected override void HoverStart()
    {
        SetNameState(true);
    }

    protected override void HoverEnd()
    {
        SetNameState(false);
    }

    protected override void Click()
    {
        SetNameState(true);
        OnClick?.Invoke();
    }

    public void SetNameState(bool state)
    {
        _nameText.color = state ? _activeNameColor : _defaultNameColor;
    }
}