using IPA.Utilities;
using System.IO;

namespace ModelMenu.Utilities;

internal class Directories
{
    public static DirectoryInfo CustomAvatars => Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomAvatars"));

    public static DirectoryInfo CustomNotes => Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomNotes"));

    public static DirectoryInfo CustomPlatforms => Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomPlatforms"));

    public static DirectoryInfo CustomSabers => Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomSabers"));
}
