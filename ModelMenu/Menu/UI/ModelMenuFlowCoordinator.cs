using HMUI;
using ModelMenu.Menu.UI.ViewControllers;
using ModelMenu.Utilities;
using SiraUtil.Logging;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace ModelMenu.Menu.UI;

internal class ModelMenuFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly SiraLog log;
    [Inject] private readonly MainView mainView;
    [Inject] private readonly MainFlowCoordinator mainFlowCoordinator;
    [Inject] private readonly ModelAssetDownloader modelDownloader;

    private const string menuTitle = "Model Menu";

    private void Awake()
    {
        SetTitle(menuTitle);
        showBackButton = true;
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        modelDownloader.AssetsDownloadingChanged += ModelsDownloadingChanged;
        try
        {
            ProvideInitialViewControllers(mainView);
        }
        catch (Exception ex)
        {
            log.Error(ex);
            mainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }

    protected override void BackButtonWasPressed(ViewController topViewController)
    {
        modelDownloader.AssetsDownloadingChanged -= ModelsDownloadingChanged;
        SetTitle(menuTitle);
        mainFlowCoordinator.DismissFlowCoordinator(this);
    }

    private int prevModelsDownloading = 0;

    private void ModelsDownloadingChanged(int modelsDownloading)
    {
        if (modelsDownloading != 0)
        {
            SetTitle(FormatTitle(modelsDownloading));
        }
        else
        {
            StartCoroutine(DownloadingFinishedCoroutine());
        }

        prevModelsDownloading = modelsDownloading;
    }

    private IEnumerator DownloadingFinishedCoroutine()
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
