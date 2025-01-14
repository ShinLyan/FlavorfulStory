using FlavorfulStory.UI;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private KeyCode _switchKey;
    
    [SerializeField] private GameObject _content;
    
    private Tab[] _tabs;

    private TabType _currentTabType;
    
    private const KeyCode PreviousTabKey = KeyCode.Q;
    
    private const KeyCode NextTabKey = KeyCode.R;
    
    private void Awake()
    {
        _tabs = GetComponentsInChildren<Tab>();
        foreach (var tab in _tabs)
        {
            tab.OnTabSelected += SelectTab;
        }
    }

    private void Start()
    {
        _currentTabType = TabType.MainTab;
        _tabs[(int) _currentTabType].Select();
    }

    private void Update()
    {
        HandleSwitchInput();
        HandleAdjacentTabsInput();
    }

    private void SelectTab(TabType tabType)
    {
        HideCurrentTab();
        _currentTabType = tabType;
        ShowCurrentTab();
    }

    private void ShowCurrentTab()
    {
        _tabs[(int) _currentTabType].Select();
    }
    
    private void HideCurrentTab()
    {
        _tabs[(int) _currentTabType].ResetSelection();
    }

    private void Show()
    {
        _content.SetActive(true);
    }

    private void Hide()
    {
        _content.SetActive(false);
    }
    
    private void Switch(bool state) => _content.SetActive(state);

    private void HandleSwitchInput()
    {
        if (Input.GetKeyDown(_switchKey))
        {
            Switch(!_content.activeSelf);
        }
    }
    
    private void HandleAdjacentTabsInput()
    {
        if (Input.GetKeyDown(PreviousTabKey) || Input.GetKeyDown(NextTabKey))
        {
            bool isPreviousTab = Input.GetKeyDown(PreviousTabKey);
            int direction = isPreviousTab ? -1 : 1;
            var currentTabIndex = (int) _currentTabType;
            
            int newTabIndex = (currentTabIndex + _tabs.Length + direction) % _tabs.Length;
            SelectTab((TabType) newTabIndex);
        }
    }
}