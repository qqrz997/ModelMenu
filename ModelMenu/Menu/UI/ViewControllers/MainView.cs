using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ModelMenu.App;
using ModelMenu.Menu.Services;
using ModelMenu.Models;
using ModelMenu.Utilities;
using ModelMenu.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ModelMenu.Menu.UI.ViewControllers;

[ViewDefinition("ModelMenu.Menu.UI.BSML.main.bsml")]
[HotReload(RelativePathToLayout = "../BSML/main.bsml")]
internal class MainView : BSMLAutomaticViewController
{
    [Inject] private readonly PluginConfig config;
    [Inject] private readonly EmbeddedResources resources;
    [Inject] private readonly ModelTileManager modelTileManager;
    [Inject] private readonly ModelThumbnailCache modelThumbnailCache;
    [Inject] private readonly ModelAssetDownloader modelDownloader;
    [Inject] private readonly InstalledAssetCache installedAssetCache;
    [Inject] private readonly PlayerDataModel playerDataModel;
    [Inject] private readonly ModelMenuFlowCoordinator modelMenuFlowCoordinator;

    private AssetType modelTypeFilter = AssetType.Saber;
    private SortBy sortTypeFilter = SortBy.Date;
    private OrderBy orderByFilter = OrderBy.Descending;
    private string searchFilter = string.Empty;

    [UIValue("model-type-choices")] private List<object> modelTypeChoices = Enum.GetNames(typeof(AssetType)).ToList<object>();
    [UIValue("model-type-filter")]
    private string ModelTypeFilter
    {
        get => modelTypeFilter.ToString();
        set
        {
            modelTypeFilter = (AssetType)Enum.Parse(typeof(AssetType), value);
            UpdateFilter();
        }
    }

    [UIValue("sort-type-choices")] private List<object> sortTypeChoices = Enum.GetNames(typeof(SortBy)).ToList<object>();
    [UIValue("sort-type-filter")]
    private string SortTypeFilter
    {
        get => sortTypeFilter.ToString();
        set
        {
            sortTypeFilter = (SortBy)Enum.Parse(typeof(SortBy), value);
            UpdateFilter();
        }
    }

    [UIValue("order-by-choices")] private List<object> orderByChoices = Enum.GetNames(typeof(OrderBy)).ToList<object>();
    [UIValue("order-by-filter")]
    private string OrderByFilter
    {
        get => orderByFilter.ToString();
        set
        {
            orderByFilter = (OrderBy)Enum.Parse(typeof(OrderBy), value);
            UpdateFilter();
        }
    }

    [UIComponent("model-type-filter")]
    private DropDownListSetting modelTypeDropdown;
    [UIComponent("sort-type-filter")]
    private DropDownListSetting sortTypeDropdown;
    [UIComponent("order-by-filter")]
    private DropDownListSetting orderByDropdown;

    [UIValue("search-filter")]
    private string SearchFilter
    {
        get => searchFilter;
        set
        {
            searchFilter = value;
            UpdateFilter();
        }
    }

    [UIComponent("search-modal")]
    private ModalView searchModal;

    [UIComponent("model-info-modal")]
    private ModalView modelInfoModal;
    [UIComponent("info-modal-title")]
    private TextMeshProUGUI infoModalTitle;
    [UIComponent("info-modal-description")]
    private TextMeshProUGUI infoModalDescription;

    [UIComponent("big-preview")]
    private ClickableImage previewImage;
    [UIComponent("download-button")]
    private ClickableImage downloadButton;
    [UIComponent("page-down-button")]
    private ClickableImage pageDownButton;
    [UIComponent("page-up-button")]
    private ClickableImage pageUpButton;

    [UIComponent("page-index")]
    private TextMeshProUGUI pageIndexText;

    [UIComponent("model-grid")]
    private GridLayoutGroup modelGrid;

    [UIObject("model-tile")]
    private GameObject modelTileOriginal;
    private readonly ModelTile[] gridModelTiles = new ModelTile[24];
    [UIComponent("model-image")]
    private ImageView modelTileImage;
    [UIComponent("checkmark-icon")]
    private ImageView modelCheckmark;

    private const int TilesPerPage = 24;
    private const int BigPreviewSize = 512;
    private const int PixelatedPreviewSize = 10;

    private IModel selectedModel = null;
    private int currentPageIndex = 0;
    private int totalPages = -1;

    [UIAction("#post-parse")]
    private async void PostParse()
    {
        modelGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        modelGrid.constraintCount = 6;

        var roundEdgeMaterial = Resources.FindObjectsOfTypeAll<Material>().First(m => m.name == "UINoGlowRoundEdge");
        previewImage.material = roundEdgeMaterial;

        pageDownButton.transform.rotation = Quaternion.Euler(0, 0, 270);
        pageUpButton.transform.rotation = Quaternion.Euler(0, 0, 90);

        modelTileImage.material = roundEdgeMaterial;
        modelCheckmark.sprite = resources.GetSpriteResource("ModelMenu.Resources.checkmark-icon-low.png");

        modelTypeDropdown.SetDropdownSize(26f, 0f);
        sortTypeDropdown.SetDropdownSize(26f, 0f);
        orderByDropdown.SetDropdownSize(26f, 0f);

        for (int i = 0; i < TilesPerPage; i++)
        {
            var currentTile = i == 0 ? modelTileOriginal : Instantiate(modelTileOriginal, modelGrid.transform, false);
            gridModelTiles[i] = new(currentTile, i);
            gridModelTiles[i].TileClicked += TileClicked;
        }

        // until this is fixed https://github.com/monkeymanboy/BeatSaberMarkupLanguage/issues/148
        downloadButton.PointerExitEvent += delegate { downloadButton.color = new(0.878f, 0.878f, 0.878f, 0.314f); };

        await ShowCurrentPage();
    }

    [UIAction("show-settings")]
    private void ShowSettings() => 
        modelMenuFlowCoordinator.TransitionToView(ModelMenuFlowCoordinator.ViewType.Settings);

    [UIAction("show-model-info")]
    private void ShowModelInfo()
    {
        if (selectedModel != null)
        {
            infoModalTitle.text = $"{selectedModel.Name} by {selectedModel.Author}";
            infoModalDescription.text = string.IsNullOrWhiteSpace(selectedModel.Description.FullName) ? "No description" 
                : selectedModel.Description.FullName;
            modelInfoModal.Show(true);
        }
    }

    [UIAction("hide-model-info")]
    private void HideModelInfo() => modelInfoModal.Hide(true);

    [UIAction("page-up")]
    private async void PageUp()
    {
        if (currentPageIndex >= totalPages - 1) return;
        currentPageIndex++;
        await ShowCurrentPage();
    }

    [UIAction("page-down")]
    private async void PageDown()
    {
        if (currentPageIndex <= 0) return;
        currentPageIndex--;
        await ShowCurrentPage();
    }

    [UIAction("download")]
    private async void Download()
    {
        if (!installedAssetCache.IsAssetInstalled(selectedModel))
        {
            downloadButton.gameObject.SetActive(false);
            await modelDownloader.InstallAssetAsync(selectedModel, OnDownloadCompleted);
        }
    }

    [UIAction("open-search-modal")]
    private void OpenSearchModal() =>
        searchModal.Show(true);

    public async void UpdateFilter()
    {
        currentPageIndex = 0;
        await ShowCurrentPage();
    }

    private void TileClicked(int gridIndex)
    {
        selectedModel = gridModelTiles[gridIndex].Model;

        var (previewSize, filterMode) = config.CensorNsfwThumbnails && selectedModel is AdultOnlyModel
            ? (PixelatedPreviewSize, FilterMode.Point) 
            : (BigPreviewSize, FilterMode.Trilinear); 

        previewImage.sprite = modelThumbnailCache.TryGetSpriteForDimension(selectedModel.Hash, previewSize, out var sprite) ? sprite 
            : !modelThumbnailCache.TryGetData(selectedModel.Hash, out var thumbnailData) ? null
            : thumbnailData.ToSprite(previewSize, filterMode);

        downloadButton.gameObject.SetActive(!installedAssetCache.IsAssetInstalled(selectedModel)
            && !modelDownloader.IsModelDownloading(selectedModel));
    }

    public async Task ShowCurrentPage()
    {
        var ageRating = playerDataModel.playerData.desiredSensitivityFlag == PlayerSensitivityFlag.Explicit
            ? AgeRating.AdultOnly
            : AgeRating.AllAges;

        var filterOptions = new FilterOptions(searchFilter, modelTypeFilter, config.HideInstalledModels);
        var sortOptions = new SortOptions(sortTypeFilter, orderByFilter);
        var ageOptions = new AgeOptions(ageRating, config.CensorNsfwThumbnails);
        var searchOptions = new ModelSearchOptions(currentPageIndex, filterOptions, sortOptions, ageOptions);

        await modelTileManager.UpdatePageAsync(gridModelTiles, searchOptions, OnPageUpdated);
    }

    private void OnPageUpdated(ModelCache.PageRequestInfo pageInfo)
    {
        totalPages = pageInfo.TotalPages;
        var currentPage = totalPages > 0 ? currentPageIndex + 1 : 0;
        pageIndexText.text = $"{currentPage} / {totalPages}";
    }

    private void OnDownloadCompleted(IModel downloadedModel, bool success)
    {
        var gridModels = gridModelTiles.Select(t => t.Model);
        if (success && gridModels.Contains(downloadedModel))
        {
            gridModelTiles.First(t => t.Model == downloadedModel).IsInstalled = true;
            if (gridModels.Contains(selectedModel) && selectedModel == downloadedModel)
            {
                downloadButton.gameObject.SetActive(false);
            }
        }
        // todo - do something on fail?
    }
}
