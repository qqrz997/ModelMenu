using ModelMenu.App;
using ModelMenu.Models;
using ModelMenu.Utilities.Extensions;
using SiraUtil.Logging;
using System;
using System.IO;
using System.Linq;

namespace ModelMenu.Menu.Services;

// todo - model this better
internal class ModelCache
{
    private readonly SiraLog log;
    private readonly PluginConfig config;
    private readonly InstalledAssetCache installedAssetCache;

    private ModelCache(SiraLog log, PluginConfig config, InstalledAssetCache installedAssetCache) =>
        (this.log, this.config, this.installedAssetCache) = (log, config, installedAssetCache);

    public IModel[] CachedModels { get; set; } = [];

    internal record PageRequestInfo(IModel[] Models, int TotalPages);

    public PageRequestInfo GetPage(int entriesCount, ModelSearchOptions searchOptions)
    {
        // get assets of a type
        var ofType = CachedModels.Where(info => info.AssetType == searchOptions.FilterOptions.AssetType);

        // find assets that match the search
        var searchPhrase = searchOptions.FilterOptions.SearchPhrase.ToLower();
        var filtered = ofType.Where(info => $"{info.Name}{info.Author}".ToLower().Contains(searchPhrase));

        if (searchOptions.FilterOptions.FilterInstalled)
        {
            // remove installed assets from results
            filtered = filtered.WhereNot(installedAssetCache.IsAssetInstalled);
        }

        if (searchOptions.AgeOptions.AgeRating != AgeRating.AdultOnly)
        {
            // remove nsfw models from results
            filtered = filtered.Where(info => info is not AdultOnlyModel);
        }

        if (searchOptions.SortOptions.SortBy != SortBy.Date)
        {
            filtered = filtered.OrderBy(info => searchOptions.SortOptions.SortBy switch
            {
                SortBy.Name => info.Name.FullName,
                SortBy.Author or _ => info.Author.FullName,
            });
        }

        if (searchOptions.SortOptions.OrderBy == OrderBy.Ascending)
        {
            filtered = filtered.Reverse();
        }

        // take a page of the results
        var pageInfo = filtered.Skip(searchOptions.PageIndex * entriesCount).Take(entriesCount);

        // fill the null entries in the page
        var page = Enumerable.Range(0, entriesCount)
                    .Select(i => pageInfo.ElementAtOrDefault(i) ?? new NoModel())
                    .ToArray();
        return new PageRequestInfo(page, Convert.ToInt32(Math.Ceiling((float)filtered.Count() / entriesCount)));
    }
}
