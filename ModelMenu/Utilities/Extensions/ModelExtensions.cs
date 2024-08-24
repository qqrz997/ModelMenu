using ModelMenu.Models;
using System.IO;

namespace ModelMenu.Utilities.Extensions;

internal static class ModelExtensions
{
    public static string GetInstallPath(this IModel model) =>
        Path.Combine(model.AssetType.GetAssetDirectory(), model.GetFileName());

    private static string GetFileName(this IModel model)
    {
        var assetDownloadUri = model.ModelAssetUri.ToString();
        return PathUtils.RemoveInvalidChars(assetDownloadUri.Substring(assetDownloadUri.LastIndexOf(Path.AltDirectorySeparatorChar) + 1));
    }
}
