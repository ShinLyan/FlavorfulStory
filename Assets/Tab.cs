using System;
using FlavorfulStory.UI;
using UnityEngine;

public class Tab : MonoBehaviour
{
    [SerializeField] private TabButton _tabButton;
    
    [SerializeField] private GameObject _tabContent;

    [SerializeField] private TabType _tabType;

    public event Action<TabType> OnTabSelected;

    private void SwitchTo()
    {
        OnTabSelected?.Invoke(_tabType);
        Select();
    }

    public void Select()
    {
        _tabButton.IsActive = true;
        _tabButton.SetNameState(true);
        _tabContent.SetActive(true);
    }

    public void ResetSelection()
    {
        _tabButton.IsActive = false;
        _tabButton.SetNameState(false);
        _tabContent.SetActive(false);
    }

    public void Initialize()
    {
        _tabButton.OnClick += SwitchTo;
    }
}