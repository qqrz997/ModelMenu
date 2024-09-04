using System.Collections.Generic;
using ModelMenu.Models;
using ModelMenu.Utilities.Extensions;
using UnityEngine;

namespace ModelMenu.Menu.UI;

internal class ModelThumbnailCache
{
    // 'temporary'
    // at least give Dictionary<int, Sprite> a name? what does Dictionary<int, Sprite> even mean???

    private readonly Dictionary<string, ThumbnailData> hashThumbnailPairs = [];
    private readonly Dictionary<ThumbnailData, Dictionary<int, Sprite>> cachedThumbnailSprites = [];

    public ThumbnailData AddData(string modelHash, byte[] imageData)
    {
        var thumbnailData = ThumbnailData.Create(imageData);
        hashThumbnailPairs.TryAdd(modelHash, thumbnailData);
        cachedThumbnailSprites.TryAdd(thumbnailData, []);
        return thumbnailData;
    }

    public bool TryGetData(string modelHash, out ThumbnailData thumbnailData)
    {
        if (hashThumbnailPairs.TryGetValue(modelHash, out thumbnailData))
        {
            return true;
        }
        thumbnailData = ThumbnailData.Empty;
        return false;
    }

    public Sprite GetSprite(string modelHash, ThumbnailData data, int size, FilterMode filterMode = FilterMode.Trilinear) =>
        TryGetSpritesForHash(modelHash, out var sprites) && sprites.TryGetValue(size, out var cached) ? cached
        : AddSprite(modelHash, data.ToSprite(size, filterMode));

    private Sprite AddSprite(string modelHash, Sprite sprite)
    {
        int size = sprite.texture.width;
        if (TryGetSpritesForHash(modelHash, out var thumbnailSprites)
            && !thumbnailSprites.ContainsKey(size))
        {
            thumbnailSprites.TryAdd(size, sprite);
        }
        return sprite;
    }

    private bool TryGetSpritesForHash(string modelHash, out Dictionary<int, Sprite> thumbnailSprites)
    {
        thumbnailSprites = [];
        return hashThumbnailPairs.TryGetValue(modelHash, out var thumbnailData)
            && cachedThumbnailSprites.TryGetValue(thumbnailData, out thumbnailSprites)
            && thumbnailSprites is not null;
    }
}
