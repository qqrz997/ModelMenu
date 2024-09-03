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
    private const int PixelatedThumbnailSize = 16;
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

            var tilesToLoadThumbnails = pageTiles.Where(t => t.Model is not NoModel);

            // todo - rate limit this

            foreach (var tile in tilesToLoadThumbnails)
            {
                if (tokenSource.IsCancellationRequested) break;

                var thumbnailData = thumbnailCache.TryGetData(tile.Model.Hash, out var cachedData) ? cachedData
                    : await modelApi.GetThumbnailAsync(tile.Model, tokenSource.Token);

                var (thumbnailSize, filterMode) = searchOptions.AgeOptions.ShouldCensorNsfw && tile.Model is AdultOnlyModel
                    ? (PixelatedThumbnailSize, FilterMode.Point)
                    : (ThumbnailSize, FilterMode.Trilinear);

                tile.Thumbnail = thumbnailCache.TryGetSpriteForDimension(tile.Model.Hash, thumbnailSize, out var cachedSprite) ? cachedSprite
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
