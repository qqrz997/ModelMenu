namespace ModelMenu.Models;

internal readonly struct ModelSearchOptions(int pageIndex, AssetType assetType, SortBy sortyBy, string searchPhrase)
{
    public int PageIndex { get; } = pageIndex;

    public AssetType AssetType { get; } = assetType;

    public SortBy SortyBy { get; } = sortyBy;

    public string SearchPhrase { get; } = searchPhrase;
}
