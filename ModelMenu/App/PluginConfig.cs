using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ModelMenu.App;

internal class PluginConfig
{
    public virtual bool CensorNsfwThumbnails { get; set; } = true;

    public virtual bool HideInstalledModels { get; set; } = false;
}
