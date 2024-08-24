using Newtonsoft.Json;

namespace ModelMenu.Menu.UI;

internal class ModelsaberEntryModel
{
    public string[] Tags { get; }

    public string AssetType { get; }

    public string Name { get; }

    public string Author { get; }

    public string Thumbnail { get; }

    public string Hash { get; }

    public string Download { get; }

    [JsonConstructor]
    private ModelsaberEntryModel(string[] tags, string type, string name, string author, string thumbnail, string hash, string download)
    {
        Tags = tags;
        AssetType = type;
        Name = !string.IsNullOrWhiteSpace(name) ? name : "Unknown";
        Author = !string.IsNullOrWhiteSpace(author) ? author : "Unknown";
        Thumbnail = thumbnail;
        Hash = hash;
        Download = download;
    }
}
