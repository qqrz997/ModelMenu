using System;

namespace ModelMenu.Models;

internal class NoModelInfo : IModelInfo
{
    public AssetType AssetType => AssetType.Saber;

    public string Name => string.Empty;

    public string Author => string.Empty;

    public string Description => string.Empty;

    public string AssetHash => string.Empty;

    public bool AdultOnly => false;

    public Uri ThumbnailUri => null;

    public Uri ModelAssetUri => null;
}
