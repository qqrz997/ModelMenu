using BeatSaberMarkupLanguage.Components;
using HMUI;
using ModelMenu.Models;
using ModelMenu.Utilities;
using SiraUtil.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ModelMenu.Menu.UI;

internal class ModelInfoTile
{
    private readonly GameObject tileHost;
    private readonly ClickableImage clickableImage;
    private readonly TextMeshProUGUI text;
    private readonly ImageView loadingIndicator;
    private readonly ImageView checkmarkIcon;
    private readonly int gridIndex;
    private readonly Regex richTextRegex = new(@"<[^>]*>");

    private IModelInfo modelInfo = new NoModelInfo();
    private bool isInstalled;

    public ModelInfoTile(GameObject tileHost, int gridIndex)
    {
        this.tileHost = tileHost;
        this.gridIndex = gridIndex;
        clickableImage = tileHost.GetComponentInChildren<ClickableImage>();
        clickableImage.OnClickEvent += TileClickEvent;
        text = tileHost.GetComponentInChildren<FormattableText>();
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        text.richText = false;
        var images = tileHost.transform.GetComponentsInChildren<ImageView>();
        loadingIndicator = images.First(i => i.sprite.name == "LoadingIndicator");
        checkmarkIcon = images.First(i => i.gameObject.name == "CheckmarkIcon");
        checkmarkIcon.raycastTarget = false;
        loadingIndicator.raycastTarget = false;
    }

    public Action<int> TileClicked;

    public IModelInfo ModelInfo
    {
        get => modelInfo;
        set
        {
            modelInfo = value;
            text.text = richTextRegex.IsMatch(value.Name) ? richTextRegex.Replace(value.Name, string.Empty) : value.Name;
        }
    }

    public Sprite Thumbnail
    {
        get => clickableImage.sprite;
        set
        {
            clickableImage.sprite = value;
            text.gameObject.SetActive(value == null);
        }
    }

    public bool IsInstalled
    {
        get => isInstalled;
        set
        {
            isInstalled = value;
            checkmarkIcon.gameObject.SetActive(value);
        }
    }

    public void SetLoading(bool value)
    {
        if (loadingIndicator.isActiveAndEnabled) loadingIndicator.gameObject.SetActive(false);
        loadingIndicator.gameObject.SetActive(value);
    }

    public void SetActive(bool active) =>
        tileHost.SetActive(active);

    public override string ToString() =>
        ModelInfo is null ? "No data" : $"{ModelInfo.Name} by {ModelInfo.Author}";

    private void TileClickEvent(PointerEventData _) =>
        TileClicked?.Invoke(gridIndex);
}
