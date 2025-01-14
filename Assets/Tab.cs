using System;
using FlavorfulStory.UI;
using UnityEngine;

public class Tab : MonoBehaviour
{
    private TabButton _tabButton;
    
    [SerializeField] private GameObject _tabContent;

    [SerializeField] private TabType _tabType;

    public event Action<TabType> OnTabSelected;
    
    private void Awake()
    {
        _tabButton = GetComponentInChildren<TabButton>();
        _tabButton.OnClick += SwitchTo;
    }

    private void SwitchTo()
    {
        OnTabSelected?.Invoke(_tabType);
        Select();
    }

    public void Select()
    {
        _tabButton.SetNameState(true);
        _tabContent.SetActive(true);
    }

    public void ResetSelection()
    {
        _tabButton.SetNameState(false);
        _tabContent.SetActive(false);
    }
}