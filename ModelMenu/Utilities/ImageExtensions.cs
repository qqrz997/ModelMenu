using System;
using System.Drawing;

namespace ModelMenu.Utilities;

internal static class ImageExtensions
{
    public static Image DownscaleImage(this Image image, int targetDimension) =>
        image.GetSquareArea() <= targetDimension * targetDimension ? image
        : new Bitmap(image, new Size(targetDimension, targetDimension));

    public static int GetSquareArea(this Image image) =>
        image.Size.Width >= image.Size.Height ? (int)Math.Pow(image.Size.Width, 2)
        : (int)Math.Pow(image.Size.Height, 2);
}
