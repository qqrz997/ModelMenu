using System;

namespace ModelMenu.Models;

internal interface IModelInfo
{
    AssetType AssetType { get; }

    string AssetHash { get; }

    string Name { get; }

    string Author { get; }

    string Description { get; }

    bool AdultOnly { get; }

    Uri ThumbnailUri { get; }

    Uri ModelAssetUri { get; }
}
