using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor;

/// <summary>
/// Base class of every component in the library. It owns the three parameters every shadcn/ui
/// component accepts (<c>className</c>, <c>style</c> and pass-through HTML attributes) plus the two
/// helpers a port needs: <see cref="Cx"/> (clsx join) and <see cref="MergeClass"/> (clsx +
/// tailwind-merge).
/// </summary>
/// <remarks>
/// <para>
/// It owns the three parameters every shadcn/ui component accepts (<c>className</c>, <c>style</c>
/// and pass-through HTML attributes) plus the two helpers a port needs:
/// <see cref="Cx"/> (clsx join) and <see cref="MergeClass"/> (clsx + tailwind-merge).
/// </para>
/// <para>
/// Blazor binds <c>class="…"</c> to the <see cref="Class"/> parameter case-insensitively, so a
/// consumer overrides the component's classes exactly the way they would in React:
/// <c>&lt;Button class="h-12"&gt;</c>.
/// </para>
/// </remarks>
public abstract class ShadcnComponent : ComponentBase
{
    private string? _generatedId;

    /// <summary>The element id. Generated (and stable for the component's lifetime) when omitted.</summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>Extra classes, merged with the component's own class list through <see cref="Cn.Merge(string?, string?)"/>.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>Any unmatched attribute is splatted onto the root element (<c>aria-*</c>, <c>data-*</c>, <c>@onclick</c>, …).</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Namespace of the generated ids; overridden by components that need a meaningful prefix.</summary>
    protected virtual string SlotName => "component";

    /// <summary>A generated-or-explicit id for this component instance.</summary>
    protected string ElementId => Id ?? (_generatedId ??= ShadcnId.New(SlotName));

    /// <summary>Joins classes without conflict resolution (use for nested fragments of one element).</summary>
    protected static string? Cx(params string?[] values) => Cn.Join(values);

    /// <summary>Merges the component's shadcn class list with the consumer's <see cref="Class"/>.</summary>
    protected string? MergeClass(string? baseClasses) => Cn.Merge(baseClasses, Class);

    /// <summary>Joins the consumer's <see cref="Style"/> with component-computed declarations.</summary>
    protected string? StyleWith(params string?[] values)
    {
        var parts = new List<string>(values.Length + 1);
        if (!string.IsNullOrWhiteSpace(Style))
        {
            parts.Add(Style);
        }

        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                parts.Add(value);
            }
        }

        return parts.Count == 0 ? null : string.Join(' ', parts);
    }
}
