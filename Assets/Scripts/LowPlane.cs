using CheckListScreen;
using MainScreen;
using UnityEngine;
using UnityEngine.UI;

public class LowPlane : MonoBehaviour
{
    [SerializeField] private Button _checkListButton;
    [SerializeField] private Button _listButton;
    [SerializeField] private Button _settingsButton;

    [SerializeField] private MainScreenController _mainScreenController;
    [SerializeField] private CheckListController _checkListController;
    [SerializeField] private Settings _settings;

    private void OnEnable()
    {
        _checkListButton.onClick.AddListener(OnCheckListClicked);
        _listButton.onClick.AddListener(OnListClicked);
        _settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    private void OnDisable()
    {
        _checkListButton.onClick.RemoveListener(OnCheckListClicked);
        _listButton.onClick.RemoveListener(OnListClicked);
        _settingsButton.onClick.RemoveListener(OnSettingsClicked);
    }

    private void OnCheckListClicked()
    {
        DisableAllScreens();
        _checkListController.EnableScreen();
    }

    private void OnListClicked()
    {
        DisableAllScreens();
        _mainScreenController.EnableScreen();
    }

    private void OnSettingsClicked()
    {
        DisableAllScreens();
        _settings.ShowSettings();
    }

    private void DisableAllScreens()
    {
        _mainScreenController.DisableScreen();
        _checkListController.DisableScreen();
        _settings.HideSettings();
    }
}