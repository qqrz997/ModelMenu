using ModelMenu.Menu.UI;
using ModelMenu.Models;
using ModelMenu.Utilities;
using Newtonsoft.Json;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ModelMenu.Menu.Services;

internal class ModelsaberApi
{
    private readonly SiraLog log;
    private readonly ModelCache modelCache;

    private readonly string infoAddress = "https://modelsaber.com/api/v2/get.php?type=all&sort=date&sortDirection=desc";

    private readonly HttpClient modelsaberClient = new();
    private readonly HttpClient thumbnailClient = new() { BaseAddress = new("https://modelsaber.com/files/") };

    private ModelsaberApi(SiraLog log, ModelCache modelCache)
    {
        this.log = log;
        this.modelCache = modelCache;

        modelsaberClient.DefaultRequestHeaders.Add("User-Agent", $"{Plugin.Name}/{Plugin.Version}");
        thumbnailClient.DefaultRequestHeaders.Add("User-Agent", $"{Plugin.Name}/{Plugin.Version}");
    }

    public async Task GetAllModelInfoAsync(IProgress<bool> finished) // todo - add cancellation, percent progress
    {
#if DEBUG
        await Task.Delay(2000);
#endif
        try
        {
            using var response = await modelsaberClient.GetStreamAsync(infoAddress);
            using var reader = new StreamReader(response);

            var result = await reader.ReadToEndAsync();
            var entries = JsonConvert.DeserializeObject<Dictionary<string, ModelsaberEntryModel>>(result).Values;

            modelCache.CachedModels = entries.Select(MatchEntry).ToArray();
            finished.Report(true);
        }
        catch (Exception ex)
        {
            log.Critical($"Issue encountered when trying to fetch model data\n{ex}");
            finished.Report(false);
        }
    }

    public async Task<Thumbnail> GetThumbnailAsync(IModel model, CancellationToken token)
    {
        // a bit of spam prevention
        await Task.Delay(1000, token);

        if (token.IsCancellationRequested)
        {
            return new NoThumbnail();
        }

        var thumbnailResponse = await thumbnailClient.GetAsync(model.ThumbnailUri, token);
        thumbnailResponse.EnsureSuccessStatusCode();

        var thumbnailData = await thumbnailResponse.Content.ReadAsByteArrayAsync();

        return thumbnailData.Length == 0 ? new NoThumbnail()
            : new ModelThumbnail(ImageManipulation.DownscaleImage(thumbnailData, 512, ImageFormat.Jpeg));
    }

    private static IModel MatchEntry(ModelsaberEntryModel entry) =>
        entry is null ? new NoModel()
        : entry.Tags.Any(t => t.ToLower().Contains("nsfw")) ? new AdultOnlyModel(Create(entry))
        : Create(entry);

    private static Model Create(ModelsaberEntryModel info) =>
        new(info.AssetType, info.Name, info.Author, info.Tags, info.Thumbnail, info.Download, info.Hash);
}
