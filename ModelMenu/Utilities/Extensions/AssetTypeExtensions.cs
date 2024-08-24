using ModelMenu.Models;

namespace ModelMenu.Utilities.Extensions;

internal static class AssetTypeExtensions
{
    public static string GetAssetDirectory(this AssetType assetType) => assetType switch
    {
        AssetType.Avatar => Directories.CustomAvatars.FullName,
        AssetType.Note => Directories.CustomNotes.FullName,
        AssetType.Platform => Directories.CustomPlatforms.FullName,
        AssetType.Saber or _ => Directories.CustomSabers.FullName
    };
}
