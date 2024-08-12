using ModelMenu.Models;
using ModelMenu.Utilities;
using SiraUtil.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModelMenu.Menu.UI;

internal class ModelInfoTileManager
{
    private readonly SiraLog log;
    private readonly ModelsaberApi modelsaberApi;
    private readonly PlayerDataModel playerDataModel;

    private ModelInfoTileManager(SiraLog log, ModelsaberApi modelsaberApi, PlayerDataModel playerDataModel) => 
        (this.log, this.modelsaberApi, this.playerDataModel) = (log, modelsaberApi, playerDataModel);

    private readonly Dictionary<ModelSearchOptions, IModelInfo[]> cachedPages = [];
    private readonly Dictionary<string, ModelThumbnail> hashThumbnailPairs = [];
    private CancellationTokenSource tokenSource;

    // todo - try using a type to represent a collection of tiles?
    public async Task UpdatePageAsync(ModelInfoTile[] pageTiles, ModelSearchOptions searchOptions)
    {
        foreach (var tile in pageTiles) tile.ModelInfo = new NoModelInfo();

        tokenSource?.Cancel();
        tokenSource?.Dispose();
        tokenSource = new();

        try
        {
            var shouldHideExplicitContent = playerDataModel.playerData.desiredSensitivityFlag != PlayerSensitivityFlag.Explicit;
            var page = await GetPage(pageTiles.Length, searchOptions, tokenSource.Token);

            for (int i = 0; i < pageTiles.Length; i++)
            {
                if (page[i] is not NoModelInfo)
                {
                    pageTiles[i].ModelInfo = page[i].AdultOnly ? new AdultModelInfo(page[i]) : page[i];
                    pageTiles[i].IsInstalled = page[i].IsAssetInstalled();
                    pageTiles[i].Thumbnail = null;
                }
            }

            foreach (var tile in pageTiles)
            {
                tile.SetActive(!(shouldHideExplicitContent && tile.ModelInfo is AdultModelInfo));
            }

            var tilesToLoadThumbnails = shouldHideExplicitContent
                ? pageTiles.Where(t => t.ModelInfo is not NoModelInfo && t.ModelInfo is not AdultModelInfo)
                : pageTiles.Where(t => t.ModelInfo is not NoModelInfo);

            await Task.WhenAll(tilesToLoadThumbnails.Select(t => SetThumbnailAsync(t, tokenSource.Token)));
        }
        catch { }
    }

    public ModelThumbnail GetThumbnailForHash(string hash) =>
        hashThumbnailPairs.TryGetValue(hash, out var thumbnail) ? thumbnail : null;

    private async Task<IModelInfo[]> GetPage(int entriesCount, ModelSearchOptions searchOptions, CancellationToken token)
    {
        if (cachedPages.TryGetValue(searchOptions, out var cachedPage))
        {
            return cachedPage;
        }
        token.ThrowIfCancellationRequested();

        var page = await modelsaberApi.DownloadPageAsync(entriesCount, searchOptions, token);
        cachedPages.Add(searchOptions, page);

        return page;
    }

    private async Task SetThumbnailAsync(ModelInfoTile tile, CancellationToken token)
    {
        if (hashThumbnailPairs.TryGetValue(tile.ModelInfo.AssetHash, out var cachedThumbnail))
        {
            tile.Thumbnail = cachedThumbnail.Sprite128;
            tile.SetLoading(false);
        }
        else
        {
            tile.SetLoading(true);
            await Task.Delay(1000, token);
            token.ThrowIfCancellationRequested();

            var thumbnail = await modelsaberApi.DownloadThumbnailAsync(tile.ModelInfo, token);
            token.ThrowIfCancellationRequested();

            hashThumbnailPairs.Add(tile.ModelInfo.AssetHash, thumbnail);

            tile.Thumbnail = thumbnail.Sprite128;
            tile.SetLoading(false);
        }
    }
}
