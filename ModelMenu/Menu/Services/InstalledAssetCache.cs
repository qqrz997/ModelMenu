using ModelMenu.Models;
using ModelMenu.Utilities;
using ModelMenu.Utilities.Extensions;
using Newtonsoft.Json;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModelMenu.Menu.Services;

internal class InstalledAssetCache
{
    private readonly SiraLog log;
    private readonly string cacheFilePath;

    private InstalledAssetCache(SiraLog log)
    {
        this.log = log;
        cacheFilePath = Path.Combine(Directories.ModData.FullName, "cache");
    }

    public Dictionary<string, string> CachedModels { get; private set; } = [];

    public HashSet<string> CachedModelHashes { get; private set; } = [];

    public bool TryGetHash(string assetPath, out string hash) =>
        CachedModels.TryGetValue(assetPath, out hash);

    public bool IsAssetInstalled(IModel model) =>
        CachedModelHashes.Contains(model.Hash);

    public bool AddHash(string hash) =>
        CachedModelHashes.Add(hash);

    public async Task ManualInit(IProgress<ProgressPercent> progress)
    {
        log.Debug($"Getting all model info");
        var sw = Stopwatch.StartNew();

        // load the cached models
        var cache = !File.Exists(cacheFilePath) ? []
            : JsonConvert.DeserializeObject<Dictionary<string, string>>(await File.ReadAllTextAsync(cacheFilePath));

        // check if the cached model is installed
        // remove it from the list if it isn't
        var keysToRemove = new List<string>();
        cache.Keys.WhereNot(File.Exists).ForEach(keysToRemove.Add);
        keysToRemove.ForEach(key => cache.Remove(key));

        // get all file paths not contained within cache
        var modelsToHash = Directories.EnumerateInstalledAssetPaths().WhereNot(cache.ContainsKey);

        try
        {
            (await GetFileHashes(modelsToHash, progress))
                .ForEach(t => cache.Add(t.directory, t.hash));
        }
        catch (Exception e)
        {
            log.Error(e);
        }

        var jsonString = JsonConvert.SerializeObject(cache);
        File.WriteAllText(cacheFilePath, jsonString);

        sw.Stop();
        log.Debug($"Cache reinitialization done in {sw.Elapsed}");

        CachedModelHashes = [.. cache.Values];
    }


    private async Task<List<(string directory, string hash)>> GetFileHashes(IEnumerable<string> directories, IProgress<ProgressPercent> progress)
    {
        var results = new List<(string, string)>();

        var progressPercent = new ProgressPercent(directories.Count());
        int tasksCompleted = 0;

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2 - 1),
            CancellationToken = CancellationToken.None
        };

        await Task.Run(() =>
        {
            Parallel.ForEach(directories, parallelOptions, (path) =>
            {
                try
                {
                    var hash = Hashing.MD5Checksum(path, "x2");
                    if (!string.IsNullOrEmpty(hash))
                        results.Add((path, hash));
                }
                catch (Exception ex)
                {
                    log.Warn($"Problem encountered when getting hash for file\n{path}\n{ex.Message}");
                }
                tasksCompleted++;
                if (progressPercent.CalculateChange(tasksCompleted))
                    progress.Report(progressPercent);
            });
        });
        return results;
    }
}
