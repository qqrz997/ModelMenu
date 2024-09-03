using BeatSaberMarkupLanguage.Components.Settings;
using UnityEngine;

namespace ModelMenu.Utilities
{
    internal static class BSMLHelpers
    {
        public static void SetDropdownSize(this DropDownListSetting dropdown, float width, float height) =>
            (dropdown.transform as RectTransform).sizeDelta = new(width, height);
    }
}
