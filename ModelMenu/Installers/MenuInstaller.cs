using ModelMenu.Menu.Services;
using ModelMenu.Menu.UI;
using ModelMenu.Menu.UI.ViewControllers;
using Zenject;

namespace ModelMenu.Installers;

internal class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        Container.Bind<ModelMenuFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<ModelDataLoadingScreenView>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<MainView>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SettingsView>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<ModelTileManager>().AsSingle();

        Container.BindInterfacesAndSelfTo<ModelsaberApi>().AsSingle();
        Container.Bind<ModelCache>().AsSingle();
        Container.Bind<InstalledAssetCache>().AsSingle();
        Container.Bind<ModelThumbnailCache>().AsSingle();
        Container.Bind<ModelAssetDownloader>().AsSingle();
    }
}
