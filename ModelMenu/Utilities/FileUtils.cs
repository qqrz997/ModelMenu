using System.IO;
using System.Linq;

namespace ModelMenu.Utilities;

internal class FileUtils
{
    private static char[] InvalidFileNameChars => Path.GetInvalidFileNameChars();

    public static string RemoveInvalidChars(string path) =>
        new(path.Where(c => !InvalidFileNameChars.Contains(c)).ToArray());

    public static string ReplaceInvalidChars(string filePath, string replacement) =>
        string.Join(replacement, filePath.Split(InvalidFileNameChars));
}
