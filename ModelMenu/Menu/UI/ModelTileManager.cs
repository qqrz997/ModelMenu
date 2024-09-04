using ModelMenu.Menu.Services;
using ModelMenu.Models;
using ModelMenu.Utilities.Extensions;
using SiraUtil.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ModelMenu.Menu.UI;

internal class ModelTileManager
{
    private readonly SiraLog log;
    private readonly ModelsaberApi modelApi;
    private readonly ModelCache modelCache;
    private readonly InstalledAssetCache installedAssetCache;
    private readonly ModelThumbnailCache thumbnailCache;

    private ModelTileManager(SiraLog log, ModelsaberApi modelApi, ModelCache modelCache, InstalledAssetCache installedAssetCache, ModelThumbnailCache thumbnailCache)
    {
        this.log = log;
        this.modelApi = modelApi;
        this.modelCache = modelCache;
        this.installedAssetCache = installedAssetCache;
        this.thumbnailCache = thumbnailCache;
    }

    private const int ThumbnailSize = 128;
    private const int PixelatedThumbnailSize = 10; // todo - there is another way to censor sprites using KawaseBlurRendererSO
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

            foreach (var tile in pageTiles)
            {
                tile.SetLoading(true);
                tile.Thumbnail = null;
            }

            foreach (var tile in pageTiles.Where(t => t.Model is not NoModel))
            {
                if (tokenSource.IsCancellationRequested) break;

                var shouldCensor = searchOptions.AgeOptions.ShouldCensorNsfw && tile.Model is AdultOnlyModel;
                var (thumbnailSize, filterMode) = shouldCensor ? (PixelatedThumbnailSize, FilterMode.Point)
                    : (ThumbnailSize, FilterMode.Trilinear);

                if (!thumbnailCache.TryGetData(tile.Model.Hash, out var thumbnailData))
                {
                    // todo - slow down api requests in a more graceful manner
                    await Task.Delay(0xA0, tokenSource.Token);
                    if (tokenSource.IsCancellationRequested) break;
                    thumbnailData = await modelApi.GetThumbnailAsync(tile.Model, tokenSource.Token);
                }

                tile.Thumbnail = thumbnailCache.TryGetSpriteForDimension(tile.Model.Hash, thumbnailSize, out var cached) ? cached
                    : thumbnailData is null || thumbnailData.Data is [] ? null
                    : thumbnailCache.AddSprite(tile.Model.Hash, thumbnailData.ToSprite(thumbnailSize, filterMode));

                tile.SetLoading(false);
            }
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
}
