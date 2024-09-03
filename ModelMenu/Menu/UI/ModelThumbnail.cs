/*namespace ModelMenu.Menu.UI;

internal abstract class Thumbnail
{
    public abstract Sprite Sprite { get; }
}

internal sealed class NoThumbnail : Thumbnail
{
    public override Sprite Sprite => null;
}

internal sealed class CensoredThumbnail(Sprite sprite) : Thumbnail
{
    public override Sprite Sprite { get; } = sprite;

    private Sprite CreateThumbnail(int dimension)
    {
        var imageData = dimension == 0 ? data : ImageManipulation.DownscaleImage(data, dimension, ImageFormat.Jpeg);
        return new Texture2D(2, 2).CreateSprite(imageData);
    }

    private Sprite CreateCensoredThumbnail()
    {
        var downscaledData = ImageManipulation.DownscaleImage(data, 16, ImageFormat.Jpeg);
        var sprite = new Texture2D(2, 2).CreateSprite(downscaledData);
        sprite.texture.filterMode = FilterMode.Point;
        return sprite;
    }
}

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
}*/
