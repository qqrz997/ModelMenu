using IPA;
using IPA.Loader;
using IPA.Logging;
using ModelMenu.Installers;
using SiraUtil.Zenject;
using Hive.Versioning;
using System.Reflection;
using BeatSaberMarkupLanguage;
using ModelMenu.Menu.CustomTags;
using ModelMenu.Utilities;

namespace ModelMenu;

[Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
internal class Plugin
{
    public static Version Version { get; private set; }

    public static string Name { get; private set; }

    public static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
    
    [Init]
    public void Init(Logger logger, Zenjector zenjector, PluginMetadata metadata)
    {
        Version = metadata.HVersion;
        Name = metadata.Name;

        BSMLParser.instance.RegisterTag(new DownloadButtonTag());

        zenjector.UseLogger(logger);
        zenjector.Install<AppInstaller>(Location.App);
        zenjector.Install<MenuInstaller>(Location.Menu);
    }
}
