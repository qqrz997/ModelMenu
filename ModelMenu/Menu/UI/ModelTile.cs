using BeatSaberMarkupLanguage.Components;
using HMUI;
using ModelMenu.Models;
using ModelMenu.Utilities;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ModelMenu.Menu.UI;

internal class ModelTile
{
    private readonly GameObject tileHost;
    private readonly ClickableImage clickableImage;
    private readonly TextMeshProUGUI text;
    private readonly ImageView loadingIndicator;
    private readonly ImageView checkmarkIcon;
    private readonly int gridIndex;

    private IModel model = new NoModel();
    private bool isInstalled;

    public ModelTile(GameObject tileHost, int gridIndex)
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
    }

    public Action<int> TileClicked;

    public IModel Model
    {
        get => model;
        set
        {
            model = value;
            text.text = RegularExpressions.RichText.IsMatch(value.Name.FullName) 
                ? RegularExpressions.RichText.Replace(value.Name.FullName, string.Empty) 
                : value.Name.FullName;
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

    public void SetLoading(bool value) => 
        loadingIndicator.gameObject.SetActive(value);

    public void SetActive(bool active) =>
        tileHost.SetActive(active);

    private void TileClickEvent(PointerEventData _) =>
        TileClicked?.Invoke(gridIndex);
}
