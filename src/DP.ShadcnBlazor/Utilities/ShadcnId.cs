namespace DP.ShadcnBlazor;

/// <summary>
/// Generates the ids components need for ARIA wiring (<c>aria-controls</c>, <c>aria-labelledby</c>,
/// <c>aria-describedby</c>) when the consumer does not pass <c>Id</c> explicitly.
/// </summary>
public static class ShadcnId
{
    private static long _counter;

    /// <summary>Returns a page-unique id such as <c>dp-dialog-content-1f</c>.</summary>
    public static string New(string prefix)
    {
        var normalized = prefix.Length == 0 ? "shadcn" : prefix.ToLowerInvariant();
        return $"dp-{normalized}-{Interlocked.Increment(ref _counter):x}";
    }
}
