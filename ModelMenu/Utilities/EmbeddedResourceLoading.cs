using System.Reflection;
using System.Threading.Tasks;
using ModelMenu.Utilities.Extensions;
using UnityEngine;

namespace ModelMenu.Utilities;

internal class EmbeddedResourceLoading
{
    public static async Task<Sprite> LoadSpriteFromResourcesAsync(string resourcePath, float pixelsPerUnit = 100.0f)
    {
        var spriteData = await LoadFromResourceAsync(resourcePath);
        return new Texture2D(2, 2).CreateSprite(spriteData, pixelsPerUnit);
    }

    private static async Task<byte[]> LoadFromResourceAsync(string resourcePath) =>
        await GetResourceAsync(Plugin.ExecutingAssembly, resourcePath);

    private static async Task<byte[]> GetResourceAsync(Assembly assembly, string resourcePath)
    {
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        var data = new byte[stream.Length];
        await stream.ReadAsync(data, 0, (int)stream.Length);
        return data;
    }
}
