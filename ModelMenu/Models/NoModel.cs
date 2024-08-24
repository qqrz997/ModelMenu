using System;

namespace ModelMenu.Models;

internal class NoModel : IModel
{
    public AssetType AssetType => AssetType.Saber;

    public ModelName Name => new(string.Empty);

    public ModelAuthor Author => new(string.Empty);

    public ModelDescription Description => new(string.Empty);

    public string Hash => string.Empty;

    public Uri ThumbnailUri => null;

    public Uri ModelAssetUri => null;
}
