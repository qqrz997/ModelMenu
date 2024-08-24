using HMUI;
using ModelMenu.Menu.Services;
using ModelMenu.Menu.UI.ViewControllers;
using SiraUtil.Logging;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace ModelMenu.Menu.UI;

internal class ModelMenuFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly MainView mainView;
    [Inject] private readonly ModelDataLoadingScreenView modelDataLoadingScreenView;
    [Inject] private readonly ModelAssetDownloader modelDownloader;
    
    private const string menuTitle = "Model Menu";
    private ViewController currentView;

    public Action DidFinish;

    public void TransitionToMainView()
    {
        currentView = mainView;
        if (!isActivated) return;
        showBackButton = true;
        PresentViewController(mainView);
    }

    public void Exit()
    {
        DismissChildViewControllersRecursively(true);
        DidFinish?.Invoke();
    }

    private void Awake()
    {
        SetTitle(menuTitle);
        currentView = modelDataLoadingScreenView;
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        modelDownloader.AssetsDownloadingChanged += AssetsDownloadingChanged;

        if (addedToHierarchy)
        {
            ProvideInitialViewControllers(currentView);
            showBackButton = currentView == mainView;
        }
    }

    protected override void BackButtonWasPressed(ViewController topViewController) => Exit();

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) =>
        modelDownloader.AssetsDownloadingChanged -= AssetsDownloadingChanged;

    private void AssetsDownloadingChanged(int modelsDownloading)
    {
        if (modelsDownloading != 0)
        {
            SetTitle(FormatTitle(modelsDownloading));
        }
        else
        {
            StartCoroutine(FormatTitleOnDownloadingFinish());
        }
    }

    private IEnumerator FormatTitleOnDownloadingFinish()
    {
        SetTitle("<color=#CFC>Downloads complete</color>");
        yield return new WaitForSeconds(1);
        SetTitle(menuTitle);
    }

    private string FormatTitle(int modelsDownloading) => modelsDownloading switch
    {
        _ => $"<color=#FCC>Downloads in progress: {modelsDownloading}</color>"
    };
}
