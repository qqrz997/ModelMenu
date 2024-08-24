using ModelMenu.App;
using Zenject;

namespace ModelMenu.Installers;

internal class AppInstaller(PluginConfig config) : Installer
{
    private readonly PluginConfig config = config;

    public override void InstallBindings()
    {
        Container.BindInstance(config);
        Container.BindInterfacesAndSelfTo<EmbeddedResources>().AsSingle();
    }
}
