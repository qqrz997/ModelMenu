using System;

namespace ModelMenu.Models;

internal interface IModel
{
    AssetType AssetType { get; }

    string Hash { get; }

    ModelName Name { get; }

    ModelAuthor Author { get; }

    ModelDescription Description { get; }

    Uri ThumbnailUri { get; }

    Uri ModelAssetUri { get; }
}


