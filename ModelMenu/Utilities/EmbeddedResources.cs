using SiraUtil.Logging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace ModelMenu.Utilities;

internal class EmbeddedResources : IInitializable
{
    private readonly SiraLog log;
    private readonly Dictionary<string, object> cachedResources = [];

    private EmbeddedResources(SiraLog log) =>
        this.log = log;

    public async void Initialize()
    {
        var resourceImageNames = Plugin.ExecutingAssembly.GetManifestResourceNames().Where(s => s.EndsWith(".png"));
        foreach (var resourceName in resourceImageNames)
        {
            if (cachedResources.ContainsKey(resourceName)) continue;
            cachedResources.Add(resourceName, await EmbeddedResourceLoading.LoadSpriteFromResourcesAsync(resourceName));
        }
    }

    public Sprite GetSpriteResource(string resourceName) =>
        cachedResources.TryGetValue(resourceName, out var sprite) ? (Sprite)sprite : null;
}
