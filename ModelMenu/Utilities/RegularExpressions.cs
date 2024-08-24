using System.Text.RegularExpressions;

namespace ModelMenu.Utilities;

internal class RegularExpressions
{
    public static Regex RichText => new(@"<[^>]*>");
}
