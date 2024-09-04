using ModelMenu.Models;
using System.Drawing.Imaging;
using UnityEngine;

namespace ModelMenu.Utilities.Extensions;

internal static class ThumbnailDataExtensions
{
    public static Sprite ToSprite(this ThumbnailData thumbnailData, int size, FilterMode filterMode = FilterMode.Trilinear) =>
        thumbnailData is null ? null
        : new Texture2D(2, 2) { filterMode = filterMode }
        .CreateSprite(ImageManipulation.DownscaleImage(thumbnailData.Data, size, ImageFormat.Jpeg));
}
