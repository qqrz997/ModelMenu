namespace ModelMenu.Models;

internal class ProgressPercent(long max)
{
    private readonly long max = max;

    public int Percent { get; private set; } = 0;

    public bool CalculateChange(long numerator)
    {
        int newPercent = (int)(numerator * 100 / max);
        if (newPercent == Percent)
            return false;

        Percent = newPercent;
        return true;
    }

    public override string ToString() => $"{Percent}%";
}
