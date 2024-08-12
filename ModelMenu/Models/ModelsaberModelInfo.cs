using System;
using System.Linq;

namespace ModelMenu.Models;

internal class ModelsaberModelInfo(string assetType, string name, string author, string[] tags, string thumbnailUriString, string downloadUriString, string assetHash) : IModelInfo
{
    private readonly string thumbnailUriString = thumbnailUriString;
    private readonly string downloadUriString = downloadUriString;

    public AssetType AssetType { get; } = assetType switch
    {
        "avatar" => AssetType.Avatar,
        "bloq" => AssetType.Note,
        "platform" => AssetType.Platform,
        "saber" or _ => AssetType.Saber,
    };

    public string Name { get; } = name;

    public string Author { get; } = author;

    public string Description => tags switch
    {
        null or [] => string.Empty,
        [var one] => one,
        [var one, var other] => $"{one} and {other}",
        [.. var tags, var last] => $"{string.Join(", ", tags)}, and {last}"
    };

    public string AssetHash { get; } = assetHash;

    public bool AdultOnly => tags.Any(t => t.ToLower() == "nsfw");

    public Uri ThumbnailUri =>
        Uri.TryCreate(thumbnailUriString, UriKind.Absolute, out Uri thumbnailUri) ? thumbnailUri
        : new Uri(downloadUriString.Substring(0, downloadUriString.LastIndexOf("/")) + "/" + thumbnailUriString);

    public Uri ModelAssetUri =>
        Uri.TryCreate(downloadUriString, UriKind.Absolute, out Uri downloadUri) ? downloadUri
        : new Uri(downloadUriString.Substring(0, downloadUriString.LastIndexOf("/")) + "/" + downloadUriString);
}
