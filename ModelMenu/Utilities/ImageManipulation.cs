using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ModelMenu.Utilities;

internal class ImageManipulation
{
    public static byte[] DownscaleImage(byte[] imageBytes, int targetDimension, ImageFormat format)
    {
        using var memoryStream = new MemoryStream(imageBytes);
        using var image = Image.FromStream(memoryStream);
        using var smallimage = image.DownscaleImage(targetDimension);
        using var imageStream = new MemoryStream();
        smallimage.Save(imageStream, format);
        return imageStream.ToArray();
    }
}
