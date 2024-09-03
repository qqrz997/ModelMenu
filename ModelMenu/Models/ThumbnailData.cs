namespace ModelMenu.Models;

internal record ThumbnailData(byte[] Data)
{
    public static ThumbnailData Create(byte[] data) =>
        data is null || data is [] ? new ThumbnailData([])
        : new ThumbnailData(data);
}
