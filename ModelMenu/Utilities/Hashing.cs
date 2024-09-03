using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ModelMenu.Utilities;

internal static class Hashing
{
    public static string MD5Checksum(string filePath, string format)
    {
        using var fileStream = File.OpenRead(filePath);
        using var md5 = MD5.Create();
        return HashToString(md5.ComputeHash(fileStream), format);
    }

    public static string HashToString(byte[] hashBytes, string format)
    {
        var sb = new StringBuilder();
        foreach (byte b in hashBytes) sb.Append(b.ToString(format));
        return sb.ToString();
    }
}
