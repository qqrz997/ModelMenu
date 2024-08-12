using ModelMenu.Utilities;
using Zenject;

namespace ModelMenu.Installers;

internal class AppInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<EmbeddedResources>().AsSingle();
    }
}
