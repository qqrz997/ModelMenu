using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using ModelMenu.Menu.Services;
using ModelMenu.Models;
using SiraUtil.Logging;
using System;
using TMPro;
using Zenject;

namespace ModelMenu.Menu.UI.ViewControllers;

[ViewDefinition("ModelMenu.Menu.UI.BSML.model-data-loading-screen.bsml")]
[HotReload(RelativePathToLayout = "../BSML/model-data-loading-screen.bsml")]
internal class ModelDataLoadingScreenView : BSMLAutomaticViewController
{
    [Inject] private readonly SiraLog log;
    [Inject] private readonly ModelsaberApi modelApi;
    [Inject] private readonly InstalledAssetCache installedAssetCache;
    [Inject] private readonly ModelMenuFlowCoordinator modelMenuFlowCoordinator;

    [UIComponent("status-text")]
    private readonly TextMeshProUGUI statusText;
    private readonly string cachingStatusTextMessage = "Caching installed models";
    private readonly string apiStatusTextMessage = "Fetching external model data";

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
        var apiInit = modelApi.GetAllModelInfoAsync(); 
        
        await cacheInit;
        statusText.text = apiStatusTextMessage;
        await apiInit;

        modelMenuFlowCoordinator.TransitionToView(ModelMenuFlowCoordinator.ViewType.Main);
    }
}
