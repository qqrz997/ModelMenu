namespace ModelMenu.Models;

internal record ModelSearchOptions
(
    int PageIndex,
    FilterOptions FilterOptions,
    SortOptions SortOptions,
    AgeOptions AgeOptions
);
