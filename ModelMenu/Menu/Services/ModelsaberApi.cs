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
    private readonly ModelThumbnailCache thumbnailCache;

    private readonly string infoAddress = @"https://modelsaber.com/api/v2/get.php?type=all&sort=date&sortDirection=desc";

    private readonly HttpClient modelsaberClient = new();
    private readonly HttpClient thumbnailClient = new() { BaseAddress = new("https://modelsaber.com/files/") };

    private ModelsaberApi(SiraLog log, ModelCache modelCache, ModelThumbnailCache thumbnailCache)
    {
        this.log = log;
        this.modelCache = modelCache;
        this.thumbnailCache = thumbnailCache;

        modelsaberClient.DefaultRequestHeaders.Add("User-Agent", $"{Plugin.Name}/{Plugin.Version}");
        thumbnailClient.DefaultRequestHeaders.Add("User-Agent", $"{Plugin.Name}/{Plugin.Version}");
    }

    public async Task GetAllModelInfoAsync(/*IProgress<ProgressPercent> progress*/) // todo - add cancellation, percent progress
    {
#if DEBUG
        await Task.Delay(2000);
#endif
        try
        {
            using var responseMessage = await modelsaberClient.GetAsync(infoAddress);
            var result = await responseMessage.Content.ReadAsStringAsync();

            // progress-based crap (generic) if i can get this to work it will be moved somewhere else
            // * seems like it is not possible with modelsaber
            // * might be possible to update modelsaber for the first time in years?
            /*using var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            using var destinationStream = new MemoryStream();
            var contentLength = responseMessage.Content.Headers.ContentLength 
                ?? (responseStream.CanSeek ? responseStream.Length : null);

            if (contentLength != null)
            {
                var progressPercent = new ProgressPercent((int)contentLength);
                var buffer = new byte[4096];
                long totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
                {
                    await destinationStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                    totalBytesRead += bytesRead;
                    if (progressPercent.CalculateChange((int)totalBytesRead))
                        progress.Report(progressPercent);
                }
            }
            else
            {
                await responseStream.CopyToAsync(destinationStream);
            }
            var result = Encoding.UTF8.GetString(destinationStream.ToArray());*/

            var entries = JsonConvert.DeserializeObject<Dictionary<string, ModelsaberEntryModel>>(result).Values;

            modelCache.CachedModels = entries.Select(MatchEntry).ToArray();
        }
        catch (Exception ex)
        {
            log.Critical($"Issue encountered when trying to fetch model data\n{ex}");
        }
    }

    public async Task<ThumbnailData> GetThumbnailAsync(IModel model, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return null;

        if (thumbnailCache.TryGetData(model.Hash, out var thumbnailData))
            return thumbnailData;

        var thumbnailResponse = await thumbnailClient.GetAsync(model.ThumbnailUri, token);
        if (!thumbnailResponse.IsSuccessStatusCode)
        {
            log.Warn($"Problem encountered when trying to get thumbnail for {model.Name}\n{thumbnailResponse.ReasonPhrase}");
            return null;
        }
        var responseData = await thumbnailResponse.Content.ReadAsByteArrayAsync();
        var imageData = ImageManipulation.DownscaleImage(responseData, 512, ImageFormat.Jpeg);

        return thumbnailCache.AddData(model.Hash, imageData);
    }

    private static IModel MatchEntry(ModelsaberEntryModel entry) =>
        entry is null ? new NoModel()
        : entry.Tags.Any(t => t.ToLower().Contains("nsfw")) ? new AdultOnlyModel(Create(entry))
        : Create(entry);

    private static Model Create(ModelsaberEntryModel info) =>
        new(info.AssetType, info.Name, info.Author, info.Tags, info.Thumbnail, info.Download, info.Hash);
}
