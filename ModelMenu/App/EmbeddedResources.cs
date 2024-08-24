using ModelMenu.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace ModelMenu.App;

internal class EmbeddedResources : IInitializable
{
    private readonly Dictionary<string, object> cachedResources = [];

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
