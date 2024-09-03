using HMUI;
using ModelMenu.Menu.Services;
using ModelMenu.Menu.UI.ViewControllers;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace ModelMenu.Menu.UI;

internal class ModelMenuFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly MainView mainView;
    [Inject] private readonly ModelDataLoadingScreenView modelDataLoadingScreenView;
    [Inject] private readonly SettingsView settingsView;
    [Inject] private readonly ModelAssetDownloader modelDownloader;
    
    private const string menuTitle = "Model Menu";
    private ViewController currentView;

    public Action DidFinish;

    private void Awake()
    {
        SetTitle(menuTitle);
        currentView = modelDataLoadingScreenView;
    }

    public enum ViewType
    {
        ModelDataLoadingScreen,
        Main,
        Settings
    }

    public void TransitionToView(ViewType viewType)
    {
        ViewController view = viewType switch
        {
            ViewType.ModelDataLoadingScreen => modelDataLoadingScreenView,
            ViewType.Main => mainView,
            ViewType.Settings or _ => settingsView
        };
        if (!isActivated || isInTransition || currentView == view) return;
        showBackButton = view is MainView;
        ReplaceTopViewController(view, finishedCallback: () => currentView = view);
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        modelDownloader.AssetsDownloadingChanged += AssetsDownloadingChanged;

        if (addedToHierarchy)
        {
            showBackButton = currentView is MainView;
            ProvideInitialViewControllers(currentView);
        }
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) =>
        modelDownloader.AssetsDownloadingChanged -= AssetsDownloadingChanged;

    protected override void BackButtonWasPressed(ViewController topViewController) =>
        DidFinish?.Invoke();

    private void AssetsDownloadingChanged(int modelsDownloading)
    {
        if (modelsDownloading != 0) SetTitle($"<color=#FCC>Downloads in progress: {modelsDownloading}</color>");
        else StartCoroutine(FormatTitleOnDownloadingFinish());
    }

    private IEnumerator FormatTitleOnDownloadingFinish()
    {
        SetTitle("<color=#CFC>Downloads complete</color>");
        yield return new WaitForSeconds(1);
        SetTitle(menuTitle);
    }
}
