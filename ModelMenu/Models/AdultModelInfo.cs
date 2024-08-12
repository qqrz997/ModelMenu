using System;

namespace ModelMenu.Models;

internal class AdultModelInfo(IModelInfo baseModel) : IModelInfo
{
    public AssetType AssetType => baseModel.AssetType;

    public string AssetHash => baseModel.AssetHash;

    public string Name => baseModel.Name;

    public string Author => baseModel.Author;

    public string Description => baseModel.Description;

    public bool AdultOnly => true;

    public Uri ThumbnailUri => baseModel.ThumbnailUri;

    public Uri ModelAssetUri => baseModel.ModelAssetUri;
}
