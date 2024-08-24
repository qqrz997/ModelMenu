using System;

namespace ModelMenu.Models;

internal class ModelDescription
{
    private string modelDescription = string.Empty;

    public string FullName
    {
        get => modelDescription;
        set => modelDescription = !string.IsNullOrWhiteSpace(value) ? value : string.Empty;
    }

    public ModelDescription(string description) =>
        FullName = description;

    public ModelDescription(string[] descriptions) =>
        FullName = descriptions switch
        {
            null or [] => string.Empty,
            [var one] => one,
            [var one, var other] => $"{one} and {other}",
            [.. var tags, var last] => $"{string.Join(", ", tags)}, and {last}"
        };

    public bool Contains(string name) =>
        FullName.Contains(name, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => FullName;
}
