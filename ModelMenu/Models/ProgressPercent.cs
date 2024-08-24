namespace ModelMenu.Models;

internal class ProgressPercent(int max)
{
    private readonly int max = max;

    public int Percent { get; private set; } = 0;

    public bool CalculateChange(int numerator)
    {
        int newPercent = numerator * 100 / max;
        if (newPercent == Percent)
            return false;

        Percent = newPercent;
        return true;
    }

    public override string ToString() => $"{Percent}%";
}
