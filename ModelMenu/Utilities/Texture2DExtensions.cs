using UnityEngine;

namespace ModelMenu.Utilities;

internal static class Texture2DExtensions
{
    public static Sprite CreateSprite(this Texture2D texture, byte[] image, float pixelsPerUnit = 100f) =>
        !texture.LoadImage(image) ? null
        : Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, pixelsPerUnit);
}
