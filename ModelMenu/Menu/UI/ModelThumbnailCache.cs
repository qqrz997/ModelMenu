using System.Collections.Generic;

namespace ModelMenu.Menu.UI;

internal class ModelThumbnailCache
{
    private readonly Dictionary<string, ModelThumbnail> hashThumbnailPairs = [];

    public Thumbnail GetThumbnail(string modelHash) =>
        hashThumbnailPairs.TryGetValue(modelHash, out var thumbnail) ? thumbnail : new NoThumbnail();

    public void Add(string modelHash, ModelThumbnail thumbnail)
    {
        if (!hashThumbnailPairs.ContainsKey(modelHash)) hashThumbnailPairs[modelHash] = thumbnail;
    }
}
