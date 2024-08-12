using ModelMenu.Menu.UI;
using ModelMenu.Menu.UI.ViewControllers;
using ModelMenu.Utilities;
using Zenject;

namespace ModelMenu.Installers;

internal class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        Container.Bind<ModelMenuFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<MainView>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<ModelInfoTileManager>().AsSingle();

        Container.Bind<ModelsaberApi>().AsSingle();
        Container.Bind<ModelAssetDownloader>().AsSingle();
    }
}
