using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ModelMenu.Configuration;

internal class PluginConfig
{
    // to be implemented
    public virtual bool CensorAdultOnlyThumbnails { get; set; } = true;
}
