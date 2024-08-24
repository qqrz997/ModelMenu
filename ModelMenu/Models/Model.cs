using System;
using System.Linq;

namespace ModelMenu.Models;

internal class Model(string assetType, string name, string author, string[] tags, string thumbnailUriString, string downloadUriString, string assetHash) : IModel
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

    public ModelName Name { get; } = new(name);

    public ModelAuthor Author { get; } = new(author);

    public ModelDescription Description { get; } = new(tags);

    public string Hash { get; } = assetHash;

    public Uri ThumbnailUri =>
        Uri.TryCreate(thumbnailUriString, UriKind.Absolute, out Uri thumbnailUri) ? thumbnailUri
        : new Uri(downloadUriString.Substring(0, downloadUriString.LastIndexOf("/")) + "/" + thumbnailUriString);

    public Uri ModelAssetUri =>
        Uri.TryCreate(downloadUriString, UriKind.Absolute, out Uri downloadUri) ? downloadUri
        : new Uri(downloadUriString.Substring(0, downloadUriString.LastIndexOf("/")) + "/" + downloadUriString);
}
