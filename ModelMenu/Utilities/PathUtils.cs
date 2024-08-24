using IPA.Utilities;
using ModelMenu.Models;
using System.IO;
using System.Linq;

namespace ModelMenu.Utilities;

internal class PathUtils
{
    private static char[] InvalidFileNameChars => Path.GetInvalidFileNameChars();

    public static string RemoveInvalidChars(string path) =>
        new(path.Where(c => !InvalidFileNameChars.Contains(c)).ToArray());

    public static string ReplaceInvalidChars(string filePath, string replacement) =>
        string.Join(replacement, filePath.Split(InvalidFileNameChars));

    public static string GetRelativeGamePath(string path) =>
        TrimPath(path, UnityGame.InstallPath);

    public static string TrimPath(string path, string trimPath) =>
        path == trimPath || !path.Contains(trimPath) ? path
        : path.Replace(trimPath, string.Empty).Trim();

    public static AssetType TypeForFilePath(string path) =>
        Path.GetExtension(path) switch
        {
            ".avatar" => AssetType.Avatar,
            ".bloq" => AssetType.Note,
            ".plat" => AssetType.Platform,
            ".saber" => AssetType.Saber
        };

    public static string FormatExistingFilePath(string path)
    {
        if (!File.Exists(path))
            return path;

        int fileCounter = 0;
        string formattedPath;
        while (true)
        {
            fileCounter++;
            formattedPath = Path.Combine(Path.GetDirectoryName(path),
                $"{Path.GetFileNameWithoutExtension(path)} ({fileCounter}){Path.GetExtension(path)}");
            if (!File.Exists(formattedPath)) break;
        }
        return formattedPath;
    }
}
