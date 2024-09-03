namespace ModelMenu.Models;

internal record ModelSearchOptions
(
    int PageIndex,
    string SearchPhrase,
    AssetType AssetType,
    SortOptions SortOptions,
    AgeOptions AgeOptions,
    bool HideInstalled
);
