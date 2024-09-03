using System.Collections.Generic;
using ModelMenu.Models;
using UnityEngine;

namespace ModelMenu.Menu.UI;

internal class ModelThumbnailCache
{
    // whatever uses this should be responsible for creating sprites from their data
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

    public Sprite AddSprite(string modelHash, Sprite sprite)
    {
        int size = sprite.texture.width;
        if (TryGetSpritesForHash(modelHash, out var thumbnailSprites)
            && !thumbnailSprites.ContainsKey(size))
        {
            thumbnailSprites.TryAdd(size, sprite);
        }
        // data doesn't exist for this model, don't do anything with the sprite
        return sprite;
    }

    public bool TryGetData(string modelHash, out ThumbnailData thumbnailData)
    {
        if (hashThumbnailPairs.TryGetValue(modelHash, out thumbnailData))
        {
            return true;
        }
        thumbnailData = ThumbnailData.Create([]);
        return false;
    }

    public bool TryGetSpriteForDimension(string modelHash, int dimension, out Sprite sprite)
    {
        sprite = null;
        return TryGetSpritesForHash(modelHash, out var thumbnailSprites)
            && thumbnailSprites.TryGetValue(dimension, out sprite);
    }

    private bool TryGetSpritesForHash(string modelHash, out Dictionary<int, Sprite> thumbnailSprites)
    {
        thumbnailSprites = [];
        return hashThumbnailPairs.TryGetValue(modelHash, out var thumbnailData)
            && cachedThumbnailSprites.TryGetValue(thumbnailData, out thumbnailSprites)
            && thumbnailSprites is not null;
    }
}
