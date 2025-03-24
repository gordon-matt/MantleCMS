namespace Mantle.Web.Navigation;

internal class FlatPositionComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y)
        {
            return 0;
        }

        // null == "before; "" == "0"
        x = x == null
            ? "before"
            : x.Trim().Length == 0 ? "0" : x.Trim(':').TrimEnd('.'); // ':' is _sometimes_ used as a partition identifier
        y = y == null
            ? "before"
            : y.Trim().Length == 0 ? "0" : y.Trim(':').TrimEnd('.');

        string[] xParts = x.Split(['.', ':']);
        string[] yParts = y.Split(['.', ':']);

        for (int i = 0; i < xParts.Length; i++)
        {
            if (yParts.Length < i + 1) // x is further defined meaning it comes after y (e.g. x == 1.2.3 and y == 1.2)
            {
                return 1;
            }

            string xPart = string.IsNullOrWhiteSpace(xParts[i]) ? "before" : xParts[i];
            string yPart = string.IsNullOrWhiteSpace(yParts[i]) ? "before" : yParts[i];

            xPart = NormalizeKnownPartitions(xPart);
            yPart = NormalizeKnownPartitions(yPart);

            bool xIsInt = int.TryParse(xPart, out int xPos);
            bool yIsInt = int.TryParse(yPart, out int yPos);

            if (!xIsInt && !yIsInt)
            {
                return string.Compare(string.Join(".", xParts), string.Join(".", yParts), StringComparison.OrdinalIgnoreCase);
            }

            if (!xIsInt || (yIsInt && xPos > yPos)) // non-int after int or greater x pos than y pos (which is an int)
            {
                return 1;
            }

            if (!yIsInt || xPos < yPos)
            {
                return -1;
            }
        }

        return xParts.Length < yParts.Length ? -1 : 0;
    }

    private static string NormalizeKnownPartitions(string partition) => partition.Length < 5
        ? partition
        : string.Compare(partition, "before", StringComparison.OrdinalIgnoreCase) == 0
            ? "-9999"
            : string.Compare(partition, "after", StringComparison.OrdinalIgnoreCase) == 0 ? "9999" : partition;
}