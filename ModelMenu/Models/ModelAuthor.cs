using System;

namespace ModelMenu.Models;

internal class ModelAuthor
{
    private string modelAuthor = string.Empty;

    public string FullName
    {
        get => modelAuthor;
        set => modelAuthor = !string.IsNullOrWhiteSpace(value) ? value : string.Empty;
    }

    public ModelAuthor(string fullName) =>
        FullName = fullName;

    public bool Contains(string name) =>
        FullName.Contains(name, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => FullName;
}
