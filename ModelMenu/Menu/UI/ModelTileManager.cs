using IPA.Utilities.Async;
using ModelMenu.Menu.Services;
using ModelMenu.Models;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModelMenu.Menu.UI;

internal class ModelTileManager
{
    private readonly SiraLog log;
    private readonly ModelsaberApi modelApi;
    private readonly ModelCache modelCache;
    private readonly PlayerDataModel playerDataModel;
    private readonly ModelThumbnailCache modelThumbnailCache;
    private readonly InstalledAssetCache installedAssetCache;

    private ModelTileManager(SiraLog log, ModelsaberApi modelApi, ModelCache modelCache, PlayerDataModel playerDataModel, ModelThumbnailCache modelThumbnailCache, InstalledAssetCache installedAssetCache)
    {
        this.log = log;
        this.modelApi = modelApi;
        this.modelCache = modelCache;
        this.playerDataModel = playerDataModel;
        this.modelThumbnailCache = modelThumbnailCache;
        this.installedAssetCache = installedAssetCache;
    }

    private CancellationTokenSource tokenSource;

    // todo - try using a type to represent a collection of tiles?
    public async Task UpdatePageAsync(ModelTile[] pageTiles, ModelSearchOptions searchOptions, Action<ModelCache.PageRequestInfo> callback)
    {
        foreach (var tile in pageTiles) tile.Model = new NoModel();

        tokenSource?.Cancel();
        tokenSource?.Dispose();
        tokenSource = new();

        try
        {
            var shouldHideExplicitContent = playerDataModel.playerData.desiredSensitivityFlag != PlayerSensitivityFlag.Explicit;
            var pageInfo = modelCache.GetPage(pageTiles.Length, searchOptions);

            callback(pageInfo);

            for (int i = 0; i < pageTiles.Length; i++)
            {
                if (pageInfo.Models[i] is NoModel)
                {
                    pageTiles[i].SetActive(false);
                    continue;
                }

                pageTiles[i].SetActive(true);
                pageTiles[i].Model = pageInfo.Models[i];
                pageTiles[i].IsInstalled = installedAssetCache.IsAssetInstalled(pageInfo.Models[i]);
                pageTiles[i].Thumbnail = null;
            }

            var tilesToLoadThumbnails = shouldHideExplicitContent
                ? pageTiles.Where(t => t.Model is not NoModel && t.Model is not AdultOnlyModel)
                : pageTiles.Where(t => t.Model is not NoModel);

            var tasks = tilesToLoadThumbnails.Select(tile => SetThumbnailAsync(tile, tokenSource.Token));
            await Task.WhenAll(tasks);
        }
#if DEBUG
        catch (Exception e)
        {
            log.Debug(e); 
        }
#else
        catch { }
#endif
    }

    private async Task SetThumbnailAsync(ModelTile tile, CancellationToken token)
    {
        tile.SetLoading(true);
        tile.Thumbnail = null;

        var thumbnail = modelThumbnailCache.GetThumbnail(tile.Model.Hash) is ModelThumbnail cached ? cached
            : await modelApi.GetThumbnailAsync(tile.Model, token) is ModelThumbnail loaded ? loaded
            : null;

        if (thumbnail is not null)
        {
            modelThumbnailCache.Add(tile.Model.Hash, thumbnail);
            tile.Thumbnail = thumbnail.GetSprite(128);
        }
        tile.SetLoading(false);
    }
}
