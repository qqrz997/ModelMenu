using System;

namespace ModelMenu.Models;

internal class ModelName
{
    private string modelName;

    public string FullName
    {
        get => modelName;
        set => modelName = !string.IsNullOrWhiteSpace(value) ? value : string.Empty;
    }

    public ModelName(string fullName) => 
        FullName = fullName;

    public bool Contains(string name) =>
        FullName.Contains(name, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => FullName;
}
