using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using ModelMenu.Menu.Services;
using ModelMenu.Models;
using SiraUtil.Logging;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

namespace ModelMenu.Menu.UI.ViewControllers;

[ViewDefinition("ModelMenu.Menu.UI.BSML.model-data-loading-screen.bsml")]
[HotReload(RelativePathToLayout = "../BSML/model-data-loading-screen.bsml")]
internal class ModelDataLoadingScreenView : BSMLAutomaticViewController
{
    [Inject] private readonly ModelsaberApi modelApi = null!;
    [Inject] private readonly InstalledAssetCache installedAssetCache = null!;
    [Inject] private readonly ModelMenuFlowCoordinator modelMenuFlowCoordinator = null!;

    [UIComponent("status-text")]
    private readonly TextMeshProUGUI statusText = null!;
    private readonly string cachingStatusTextMessage = "Caching installed models";
    private readonly string apiStatusTextMessage = "Fetching external model data";

    private bool initialized = false;

    [UIAction("cancel")]
    private void Cancel()
    {
        modelMenuFlowCoordinator.DidFinish();
    }

    [UIAction("#post-parse")]
    private async void PostParse()
    {
        var cacheInit = installedAssetCache.ManualInit(
            new Progress<ProgressPercent>((p) => statusText.text = $"{cachingStatusTextMessage} - {p}"));

        // can't get progress to work with modelsaber atm
        var apiInit = modelApi.ManualInit();

        await cacheInit;
        statusText.text = apiStatusTextMessage;
        await apiInit;

        initialized = true;

        if (isActivated)
        {
            StartCoroutine(TransitionToMain());
        }
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

        if (initialized)
        {
            StartCoroutine(TransitionToMain());
        }
    }

    private IEnumerator TransitionToMain()
    {
        yield return new WaitUntil(() => !isInTransition);
        if (isActivated)
        {
            modelMenuFlowCoordinator.TransitionToView(ModelMenuFlowCoordinator.ViewType.Main);
        }
    }
}
