using IPA.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelMenu.Utilities;

internal class Directories
{
    public static DirectoryInfo UserData => Directory.CreateDirectory(UnityGame.UserDataPath);
    public static DirectoryInfo ModData => Directory.CreateDirectory(Path.Combine(UserData.FullName, "ModelMenu"));

    public static DirectoryInfo CustomAvatars => Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomAvatars"));
    public static DirectoryInfo CustomNotes => Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomNotes"));
    public static DirectoryInfo CustomPlatforms => Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomPlatforms"));
    public static DirectoryInfo CustomSabers => Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomSabers"));

    public static IEnumerable<string> EnumerateInstalledAssetPaths() =>
        AssetDirs.SelectMany(d => Directory.EnumerateFiles(d.Directory, $"*{d.FileExtension}", SearchOption.AllDirectories));
    
    private static PathExtensionPair[] AssetDirs =>
    [
        new(CustomAvatars.FullName, ".avatar"),
        new(CustomPlatforms.FullName, ".plat"),
        new(CustomNotes.FullName, ".bloq"),
        new(CustomSabers.FullName, ".saber")
    ];

    private readonly record struct PathExtensionPair(string Directory, string FileExtension);
}
