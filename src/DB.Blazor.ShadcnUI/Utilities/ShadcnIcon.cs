using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI;

/// <summary>
/// Base of the inline SVG icons the components use internally (lucide, the icon set shadcn/ui
/// renders). Exposed publicly so a consumer can reuse the exact same glyphs in menus and alerts.
/// </summary>
public abstract class ShadcnIcon : ComponentBase
{
    /// <summary>Icon classes — a size utility such as <c>size-4</c> is what shadcn passes.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Inline style.</summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>Any extra attribute, splatted onto the <c>&lt;svg&gt;</c>.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
