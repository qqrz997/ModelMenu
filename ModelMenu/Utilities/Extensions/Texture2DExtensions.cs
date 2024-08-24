using IPA.Utilities;
using UnityEngine;

namespace ModelMenu.Utilities.Extensions;

internal static class Texture2DExtensions
{
    public static Sprite CreateSprite(this Texture2D texture, byte[] image, float pixelsPerUnit = 100f) =>
        !UnityGame.OnMainThread || !texture.LoadImage(image) ? null
        : Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, pixelsPerUnit);
}
