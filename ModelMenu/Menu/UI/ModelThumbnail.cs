using ModelMenu.Utilities;
using System.Drawing.Imaging;
using UnityEngine;

namespace ModelMenu.Menu.UI;

internal class ModelThumbnail(byte[] data)
{
    private readonly byte[] data = data;

    private Sprite sprite128 = null;
    private Sprite spriteFull = null;

    public Sprite Sprite128 => sprite128 ??= CreateThumbnail(128);

    public Sprite SpriteFull => spriteFull ??= CreateThumbnail();

    private Sprite CreateThumbnail(int dimension = 0) =>
        new Texture2D(2, 2).CreateSprite(dimension > 0 ? ImageManipulation.DownscaleImage(data, dimension, ImageFormat.Jpeg) : data);
}
