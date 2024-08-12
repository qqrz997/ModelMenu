using Newtonsoft.Json;

namespace ModelMenu.Menu.UI;

internal class ModelsaberEntryModel
{
    public string[] Tags { get; private set; }

    public string AssetType { get; private set; }

    public string Name { get; private set; }

    public string Author { get; private set; }

    public string Thumbnail { get; private set; }

    public string Hash { get; private set; }

    public string Download { get; private set; }

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
