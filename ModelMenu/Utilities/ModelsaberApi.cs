using ModelMenu.Menu.UI;
using ModelMenu.Models;
using Newtonsoft.Json;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModelMenu.Utilities;

internal class ModelsaberApi
{
    private readonly SiraLog log;
    private readonly HttpClient modelsaberClient = new() { BaseAddress = new("https://modelsaber.com/api/v2/get.php") };
    private readonly HttpClient thumbnailClient = new() { BaseAddress = new("https://modelsaber.com/files/") };

    private ModelsaberApi(SiraLog log)
    {
        this.log = log;
        modelsaberClient.DefaultRequestHeaders.Add("User-Agent", $"{Plugin.Name}/{Plugin.Version}");
        thumbnailClient.DefaultRequestHeaders.Add("User-Agent", $"{Plugin.Name}/{Plugin.Version}");
    }

    public async Task<IModelInfo[]> DownloadPageAsync(int entriesCount, ModelSearchOptions searchOptions, CancellationToken token)
    {
        var foundEntries = new IModelInfo[entriesCount];

        if (token.IsCancellationRequested)
        {
            return foundEntries.Select(info => info = new NoModelInfo()).ToArray();
        }

        var type = searchOptions.AssetType switch
        {
            AssetType.Avatar => "avatar",
            AssetType.Note => "bloq",
            AssetType.Platform => "platform",
            AssetType.Saber => "saber",
            _ => throw new ArgumentException()
        };

        var sort = searchOptions.SortyBy switch
        {
            SortBy.Date => "date",
            SortBy.Author => "author",
            SortBy.Name => "name",
            _ => throw new ArgumentException()
        };

        var requestBuilder = new StringBuilder();
        requestBuilder.Append($"?type={type}&start={searchOptions.PageIndex * entriesCount}&end={searchOptions.PageIndex * entriesCount + entriesCount}&sort={sort}&sortDirection=desc");
        if (!string.IsNullOrEmpty(searchOptions.SearchPhrase)) requestBuilder.Append($"&filter={searchOptions.SearchPhrase}");
        string requestString = requestBuilder.ToString();

        log.Trace($"Getting response from {modelsaberClient.BaseAddress}{requestString}");
        using var response = await modelsaberClient.GetAsync(requestString, token);
        response.EnsureSuccessStatusCode();

        var modelsaberEntryString = await response.Content.ReadAsStringAsync();
        var entries = JsonConvert.DeserializeObject<Dictionary<string, ModelsaberEntryModel>>(modelsaberEntryString).Values;

        for (int i = 0; i < entriesCount; ++i)
        {
            var entry = entries.ElementAtOrDefault(i);
            foundEntries[i] = entry is null ? new NoModelInfo()
                : new ModelsaberModelInfo(entry.AssetType, entry.Name, entry.Author, entry.Tags, entry.Thumbnail, entry.Download, entry.Hash);
        }
        return foundEntries;
    }

    public async Task<ModelThumbnail> DownloadThumbnailAsync(IModelInfo modelInfo, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return null;
        }

        log.Trace($"Getting response from {modelInfo.ThumbnailUri}");
        var thumbnailResponse = await thumbnailClient.GetAsync(modelInfo.ThumbnailUri, token);
        thumbnailResponse.EnsureSuccessStatusCode();

        var thumbnailData = await thumbnailResponse.Content.ReadAsByteArrayAsync();

        return thumbnailData.Length == 0 ? null
            : new ModelThumbnail(ImageManipulation.DownscaleImage(thumbnailData, 256, ImageFormat.Jpeg));
    }
}
