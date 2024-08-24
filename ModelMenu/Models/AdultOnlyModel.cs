using System;

namespace ModelMenu.Models;

internal class AdultOnlyModel(IModel baseModel) : IModel
{
    public AssetType AssetType => baseModel.AssetType;

    public string Hash => baseModel.Hash;

    public ModelName Name => baseModel.Name;

    public ModelAuthor Author => baseModel.Author;

    public ModelDescription Description => baseModel.Description;

    public Uri ThumbnailUri => baseModel.ThumbnailUri;

    public Uri ModelAssetUri => baseModel.ModelAssetUri;
}
