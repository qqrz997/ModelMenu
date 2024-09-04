/*using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ModelMenu.Menu.CustomTags;

internal class DownloadButtonTag : BSMLTag
{
    public override string[] Aliases => ["download-button", "dl-button"];

    public override GameObject CreateObject(Transform parent)
    {
        var backgroundSource = Resources.FindObjectsOfTypeAll<ImageView>()
            .First(img => img.sprite && img.sprite.name == "FullCircle128" && img.gameObject.name == "DownloadBackground").gameObject;
        
        var gameObject = Object.Instantiate(backgroundSource, parent, false).gameObject;
        gameObject.name = "DownloadButton";
        gameObject.layer = 5;

        Object.DestroyImmediate(gameObject.GetComponent<ImageView>());
        var downloadBackground = gameObject.AddComponent<ClickableImage>();
        downloadBackground.rectTransform.anchorMin = new(0, 1);
        downloadBackground.rectTransform.anchorMax = new(0, 1);
        downloadBackground.rectTransform.sizeDelta = new(5, 5);
        downloadBackground.sprite = backgroundSource.GetComponent<ImageView>().sprite;
        downloadBackground.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
        downloadBackground.DefaultColor = new(0.2f, 0.2f, 0.2f, 0.85f);

        gameObject.AddComponent<LayoutElement>();

        return gameObject;
    }
}
*/