using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor;

/// <summary>
/// Base class of every component in the library. It owns the three parameters every shadcn/ui
/// component accepts (<c>className</c>, <c>style</c> and pass-through HTML attributes) plus the two
/// helpers a port needs: <see cref="Cx"/> (clsx join) and <see cref="MergeClass"/> (clsx +
/// tailwind-merge).
/// </summary>
/// <remarks>
/// There is deliberately no <c>Id</c> parameter: in React <c>id</c> is just another prop, so here
/// it flows through <see cref="AdditionalAttributes"/> and lands on the root element unchanged.
/// Components that need an id for ARIA wiring generate one internally (<see cref="ElementId"/>).
/// </remarks>
public abstract class ShadcnComponent : ComponentBase
{
    private string? _generatedId;

    /// <summary>Extra classes, merged with the component's own class list through <see cref="Cn.Merge(string?, string?)"/>.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>Any unmatched attribute is splatted onto the root element (<c>aria-*</c>, <c>data-*</c>, <c>id</c>, <c>@onclick</c>, …).</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Namespace of the generated ids; overridden by components that need a meaningful prefix.</summary>
    protected virtual string SlotName => "component";

    /// <summary>A generated id for this component instance, used by <c>aria-controls</c>/<c>aria-labelledby</c>.</summary>
    protected string ElementId => _generatedId ??= ShadcnId.New(SlotName);

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
