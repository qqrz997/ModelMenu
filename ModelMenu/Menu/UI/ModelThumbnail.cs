using ModelMenu.Utilities;
using ModelMenu.Utilities.Extensions;
using System.Collections.Generic;
using System.Drawing.Imaging;
using UnityEngine;

namespace ModelMenu.Menu.UI;

internal abstract class Thumbnail;

internal sealed class NoThumbnail : Thumbnail;

internal sealed class ModelThumbnail(byte[] data) : Thumbnail
{
    private readonly byte[] data = data;
    private readonly Dictionary<int, Sprite> dimensionSpritePairs = [];

    public Sprite GetSprite(int dimension = 0) =>
        dimensionSpritePairs.ContainsKey(dimension) ? dimensionSpritePairs[dimension]
        : dimensionSpritePairs[dimension] = CreateThumbnail(dimension);

    private Sprite CreateThumbnail(int dimension)
    {
        var imageData = dimension == 0 ? data : ImageManipulation.DownscaleImage(data, dimension, ImageFormat.Jpeg);
        return new Texture2D(2, 2).CreateSprite(imageData);
    }
}
