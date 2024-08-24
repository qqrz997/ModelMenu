using ModelMenu.Models;
using ModelMenu.Utilities;
using ModelMenu.Utilities.Extensions;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModelMenu.Menu.Services;

internal class ModelAssetDownloader
{
    private readonly SiraLog log;
    private readonly InstalledAssetCache installedAssetCache;
    private readonly HttpClient httpClient = new();

    private ModelAssetDownloader(SiraLog log, InstalledAssetCache installedAssetCache)
    {
        this.log = log;
        this.installedAssetCache = installedAssetCache;
        httpClient.DefaultRequestHeaders.Add("User-Agent", $"{Plugin.Name}/{Plugin.Version}");
    }

    public Action<int> AssetsDownloadingChanged;

    private readonly IList<IModel> downloadingModels = [];

    public async Task InstallAssetAsync(IModel model, Action<IModel, bool> callback)
    {
        if (IsModelDownloading(model))
        {
            callback?.Invoke(model, false);
            return;
        }

        downloadingModels.Add(model);
        AssetsDownloadingChanged?.Invoke(downloadingModels.Count);

        var fullPath = PathUtils.FormatExistingFilePath(model.GetInstallPath());

        try
        {
            /*var uriString = model.ModelAssetUri.ToString();

            int i = uriString.LastIndexOf(Path.AltDirectorySeparatorChar);
            var uriBase = uriString.Substring(0, i + 1);
            var uriEnd = uriString.Substring(i + 1);
            var escapedEnd = Uri.EscapeDataString(uriEnd);
            var escapedUri = new Uri(uriBase + escapedEnd);*/

            using var response = await httpClient.GetAsync(model.ModelAssetUri);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(fullPath, FileMode.CreateNew);
            await responseStream.CopyToAsync(fileStream);

            installedAssetCache.AddHash(model.Hash);
            callback(model, true);
        }
        catch (Exception ex)
        {
            log.Warn(ex);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            callback(model, false);
        }

        downloadingModels.Remove(model);
        AssetsDownloadingChanged?.Invoke(downloadingModels.Count);
    }

    public bool IsModelDownloading(IModel model) =>
        downloadingModels.Contains(model);
}
