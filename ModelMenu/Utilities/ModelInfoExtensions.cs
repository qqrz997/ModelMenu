using ModelMenu.Models;
using System.IO;

namespace ModelMenu.Utilities;

internal static class ModelInfoExtensions
{
    public static bool IsAssetInstalled(this IModelInfo modelInfo) =>
        File.Exists(modelInfo.GetInstallPath());

    public static string GetInstallPath(this IModelInfo modelInfo) =>
        Path.Combine(modelInfo.GetAssetDirectory(), modelInfo.GetFileName());

    private static string GetAssetDirectory(this IModelInfo modelInfo) => modelInfo.AssetType switch
    {
        AssetType.Avatar => Directories.CustomAvatars.FullName,
        AssetType.Note => Directories.CustomNotes.FullName,
        AssetType.Platform => Directories.CustomPlatforms.FullName,
        AssetType.Saber or _ => Directories.CustomSabers.FullName
    };

    private static string GetFileName(this IModelInfo modelInfo)
    {
        var assetDownloadUri = modelInfo.ModelAssetUri.ToString();
        return FileUtils.RemoveInvalidChars(assetDownloadUri.Substring(assetDownloadUri.LastIndexOf(Path.AltDirectorySeparatorChar) + 1));
    }
}
