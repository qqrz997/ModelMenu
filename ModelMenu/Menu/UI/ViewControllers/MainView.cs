using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Utilities;
using IPA.Utilities.Async;
using ModelMenu.App;
using ModelMenu.Menu.Services;
using ModelMenu.Models;
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

    private AssetType modelTypeFilter = AssetType.Saber;
    private SortBy sortTypeFilter = SortBy.Date;
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
    [UIObject("download-button")]
    private GameObject downloadButton;

    [UIComponent("page-index")]
    private TextMeshProUGUI pageIndexText;

    [UIComponent("model-grid")]
    private GridLayoutGroup modelGrid;

    [UIObject("model-tile")]
    private GameObject modelTileOriginal;
    private readonly ModelTile[] gridModelTiles = new ModelTile[24];

    private const int tilesPerPage = 24;

    private IModel selectedModel = null;
    private int currentPage = 0;

    [UIAction("#post-parse")]
    private async void PostParse()
    {
        modelGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        modelGrid.constraintCount = 6;

        var roundEdgeMaterial = Resources.FindObjectsOfTypeAll<Material>().First(m => m.name == "UINoGlowRoundEdge");
        previewImage.material = roundEdgeMaterial;

        var tileImages = modelTileOriginal.GetComponentsInChildren<ImageView>();
        tileImages.First(i => i.gameObject.name == "ModelImage").material = roundEdgeMaterial;
        tileImages.First(i => i.gameObject.name == "CheckmarkIcon").sprite = resources.GetSpriteResource("ModelMenu.Resources.checkmark-icon-low.png");

        for (int i = 0; i < tilesPerPage; i++)
        {
            var currentTile = i == 0 ? modelTileOriginal : Instantiate(modelTileOriginal, modelGrid.transform, false);
            gridModelTiles[i] = new(currentTile, i);
            gridModelTiles[i].TileClicked += TileClicked;
        }

        await ShowPage(currentPage);
    }

    private void TileClicked(int gridIndex)
    {
        selectedModel = gridModelTiles[gridIndex].Model;
        previewImage.sprite = modelThumbnailCache.GetThumbnail(selectedModel.Hash) is ModelThumbnail modelThumbnail 
            ? modelThumbnail.GetSprite() : null;
        downloadButton.gameObject.SetActive(!installedAssetCache.IsAssetInstalled(selectedModel)
            && !modelDownloader.IsModelDownloading(selectedModel));
    }

    [UIAction("show-model-info")]
    private void ShowModelInfo()
    {
        if (selectedModel != null)
        {
            infoModalTitle.text = $"{selectedModel.Name} by {selectedModel.Author}";
            infoModalDescription.text = !string.IsNullOrWhiteSpace(selectedModel.Description.FullName) 
                ? selectedModel.Description.FullName : "No description";
            modelInfoModal.Show(true);
        }
    }

    [UIAction("hide-model-info")]
    private void HideModelInfo() => modelInfoModal.Hide(true);

    [UIAction("page-up")]
    private async void PageUp()
    {
        // todo - need a definitive way to know when it is on the last page
        if (currentPage >= int.MaxValue || gridModelTiles.Any(t => t.Model is NoModel)) return;
        currentPage++;
        await ShowPage(currentPage);
    }

    [UIAction("page-down")]
    private async void PageDown()
    {
        if (currentPage <= 0) return;
        currentPage--;
        await ShowPage(currentPage);
    }

    [UIAction("download")]
    private async void Download()
    {
        if (!installedAssetCache.IsAssetInstalled(selectedModel))
        {
            downloadButton.SetActive(false);
            await modelDownloader.InstallAssetAsync(selectedModel, OnDownloadCompleted);
        }
    }

    [UIAction("open-search-modal")]
    private void OpenSearchModal() =>
        searchModal.Show(true);

    private async void UpdateFilter()
    {
        currentPage = 0;
        await ShowPage(currentPage);
    }

    private void OnDownloadCompleted(IModel downloadedModel, bool success)
    {
        var gridModels = gridModelTiles.Select(t => t.Model);
        if (success && gridModels.Contains(downloadedModel))
        {
            gridModelTiles.First(t => t.Model == downloadedModel).IsInstalled = true;
            if (gridModels.Contains(selectedModel) && selectedModel == downloadedModel)
            {
                downloadButton.SetActive(false);
            }
        }
        // todo - do something on fail?
    }

    private async Task ShowPage(int pageNumber)
    {
        var showAdultOnly = playerDataModel.playerData.desiredSensitivityFlag == PlayerSensitivityFlag.Explicit;
        var searchOptions = new ModelSearchOptions(
            pageNumber,
            searchFilter,
            modelTypeFilter, 
            new SortOptions(sortTypeFilter),
            new AgeOptions(showAdultOnly ? AgeRating.AdultOnly : AgeRating.AllAges),
            HideInstalled: false); // todo - figure out where this setting should live
                                   // could either be config or another view controller 
        await modelTileManager.UpdatePageAsync(gridModelTiles, searchOptions, 
            (page) => pageIndexText.text = $"{pageNumber + 1} / {page.TotalPages}");
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        if (!firstActivation) UnityMainThreadTaskScheduler.Factory.StartNew(() => ShowPage(currentPage));
    }
}
