using System.Text;

namespace DP.ShadcnBlazor;

/// <summary>
/// The C# equivalent of shadcn/ui's <c>cn()</c> helper: <c>clsx</c> (join, drop empties)
/// followed by <c>tailwind-merge</c> (the last class of a conflicting group wins).
/// </summary>
/// <remarks>
/// Every component merges its shadcn class list with the consumer's <c>Class</c> through this
/// helper, so <c>&lt;Button Class="h-12 px-8" /&gt;</c> really replaces the default height and
/// padding instead of appending a second, order-dependent declaration.
/// </remarks>
public static class Cn
{
    /// <summary>Joins the non-empty values with a single space (clsx semantics).</summary>
    public static string? Join(params string?[] values)
    {
        StringBuilder? builder = null;
        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if (builder is null)
            {
                builder = new StringBuilder(value.Length);
                builder.Append(value.Trim());
            }
            else
            {
                builder.Append(' ').Append(value.Trim());
            }
        }

        return builder?.ToString();
    }

    /// <summary>Joins and then resolves Tailwind conflicts, last declaration wins.</summary>
    public static string? Merge(params string?[] values) => TwMerge.Merge(Join(values));

    /// <summary>Joins the base class list with the consumer's <paramref name="className"/>.</summary>
    public static string? Merge(string? baseClasses, string? className) => TwMerge.Merge(Join(baseClasses, className));
}
