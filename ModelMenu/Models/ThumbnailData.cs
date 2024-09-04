namespace ModelMenu.Models;

internal record ThumbnailData(byte[] Data)
{
    public static ThumbnailData Create(byte[] data) =>
        data is null ? new ThumbnailData([])
        : new ThumbnailData(data);

    public static ThumbnailData Empty { get; } = Create([]);
}
