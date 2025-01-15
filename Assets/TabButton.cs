using System;
using TMPro;
using UnityEngine;

public class TabButton : CustomButton
{
    [SerializeField] private TMP_Text _nameText;
    
    /// <summary> Цвет текста выбранной вкладки. </summary>
    [SerializeField] private Color _activeLabelColor; // = new(1, 0.8566375f, 0.6745283f)

    /// <summary> Цвет текста вкладки по умолчанию. </summary>
    [SerializeField] private Color _defaultLabelColor;// = Color.white

    public bool IsActive;
    
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
        if (state == false && IsActive == false && !IsMouseOver)
        {
            _nameText.color = _defaultLabelColor;
        }
        else
        {
            _nameText.color = _activeLabelColor;
        }
    }

    private void OnEnable()
    {
        _nameText.color = IsActive ? _activeLabelColor : _defaultLabelColor;
    }
}