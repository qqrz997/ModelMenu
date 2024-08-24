using IPA;
using IPA.Loader;
using IPA.Logging;
using ModelMenu.Installers;
using SiraUtil.Zenject;
using Hive.Versioning;
using System.Reflection;
using ModelMenu.Menu.CustomTags;
using IPA.Config;
using IPA.Config.Stores;
using ModelMenu.App;

namespace ModelMenu;

[Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
internal class Plugin
{
    public static Version Version { get; private set; }

    public static string Name { get; private set; }

    public static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
    
    [Init]
    public void Init(Logger logger, Config config, Zenjector zenjector, PluginMetadata metadata)
    {
        Version = metadata.HVersion;
        Name = metadata.Name;

        BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTag(new DownloadButtonTag());

        var pluginConfig = config.Generated<PluginConfig>();

        zenjector.UseLogger(logger);
        zenjector.Install<AppInstaller>(Location.App, pluginConfig);
        zenjector.Install<MenuInstaller>(Location.Menu);
    }
}
