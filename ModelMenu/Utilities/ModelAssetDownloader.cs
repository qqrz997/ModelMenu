using ModelMenu.Models;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModelMenu.Utilities;

internal class ModelAssetDownloader
{
    private readonly SiraLog log;
    private readonly HttpClient httpClient = new();

    private ModelAssetDownloader(SiraLog log)
    {
        this.log = log;
        httpClient.DefaultRequestHeaders.Add("User-Agent", $"{Plugin.Name}/{Plugin.Version}");
    }

    public Action<int> AssetsDownloadingChanged;

    private readonly IList<IModelInfo> downloadingModels = [];

    public async Task InstallAssetAsync(IModelInfo modelInfo, Action<IModelInfo, bool> callback)
    {
        if (IsModelDownloading(modelInfo))
        {
            callback?.Invoke(modelInfo, false);
            return;
        }

        downloadingModels.Add(modelInfo);
        AssetsDownloadingChanged?.Invoke(downloadingModels.Count);

        var fullPath = modelInfo.GetInstallPath();

        try
        {
            var uriString = modelInfo.ModelAssetUri.ToString();

            int i = uriString.LastIndexOf(Path.AltDirectorySeparatorChar);
            var uriBase = uriString.Substring(0, i + 1);
            var uriEnd = uriString.Substring(i + 1);
            var escapedEnd = Uri.EscapeDataString(uriEnd);
            var escapedUri = new Uri(uriBase + escapedEnd);

            using var response = await httpClient.GetAsync(escapedUri);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(fullPath, FileMode.CreateNew);
            await responseStream.CopyToAsync(fileStream);

            callback?.Invoke(modelInfo, true);
        }
        catch (Exception ex)
        {
            log.Warn(ex);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            callback?.Invoke(modelInfo, false);
        }

        downloadingModels.Remove(modelInfo);
        AssetsDownloadingChanged?.Invoke(downloadingModels.Count);
    }

    public bool IsModelDownloading(IModelInfo modelInfo) =>
        downloadingModels.Contains(modelInfo);
}
