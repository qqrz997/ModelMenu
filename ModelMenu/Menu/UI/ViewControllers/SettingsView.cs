using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using ModelMenu.App;
using SiraUtil.Logging;
using Zenject;

namespace ModelMenu.Menu.UI.ViewControllers;

[ViewDefinition("ModelMenu.Menu.UI.BSML.settings.bsml")]
[HotReload(RelativePathToLayout = "../BSML/settings.bsml")]
internal class SettingsView : BSMLAutomaticViewController
{
    [Inject] private readonly SiraLog log;
    [Inject] private readonly PluginConfig config;
    [Inject] private readonly ModelMenuFlowCoordinator modelMenuFlowCoordinator;
    [Inject] private readonly MainView mainView;

    [UIValue("hide-installed-models")]
    private bool HideInstalledModels { get => hideInstalledModels; set => hideInstalledModels = value; }
    private bool hideInstalledModels;

    [UIValue("censor-adult-only-thumbnails")]
    private bool CensorAdultOnlyThumbnails { get => censorAdultOnlyThumbnails; set => censorAdultOnlyThumbnails = value; }
    private bool censorAdultOnlyThumbnails;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        hideInstalledModels = config.HideInstalledModels;
        censorAdultOnlyThumbnails = config.CensorNsfwThumbnails;
        NotifyPropertyChanged(nameof(HideInstalledModels));
        NotifyPropertyChanged(nameof(CensorAdultOnlyThumbnails));
    }

    [UIAction("apply")]
    private void Apply()
    {
        config.CensorNsfwThumbnails = censorAdultOnlyThumbnails;
        config.HideInstalledModels = hideInstalledModels;
        mainView.UpdateFilter();
        mainView.UpdateSelectedModelPreview();
        modelMenuFlowCoordinator.TransitionToView(ModelMenuFlowCoordinator.ViewType.Main);
    }

    [UIAction("cancel")]
    private void Cancel() => 
        modelMenuFlowCoordinator.TransitionToView(ModelMenuFlowCoordinator.ViewType.Main);
}
