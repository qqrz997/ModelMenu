namespace ModelMenu.Models;

internal readonly record struct ModelSearchOptions
(
    int PageIndex,
    string SearchPhrase,
    AssetType AssetType,
    SortOptions SortOptions,
    AgeOptions AgeOptions,
    bool HideInstalled
);
