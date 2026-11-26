using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>
/// shadcn/ui <c>Button</c>. Renders <c>&lt;button data-slot="button"&gt;</c>, or an
/// <c>&lt;a&gt;</c> when <see cref="Href"/> is set — the Blazor equivalent of
/// <c>asChild</c> + <c>&lt;Slot&gt;</c>, and what Radzen does with its <c>Link</c> components.
/// </summary>
public partial class Button : ShadcnComponent
{
    /// <summary>Visual variant, <c>default</c> out of the box.</summary>
    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Default;

    /// <summary>Control size, <c>default</c> out of the box.</summary>
    [Parameter]
    public ButtonSize Size { get; set; } = ButtonSize.Default;

    /// <summary>Native <c>type</c>; <c>button</c> by default so a form is never submitted by accident.</summary>
    [Parameter]
    public string Type { get; set; } = "button";

    /// <summary>Disables the native button (<c>disabled</c> + the shadcn disabled classes).</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>When set, the button renders as an anchor instead of a native button.</summary>
    [Parameter]
    public string? Href { get; set; }

    /// <summary>Anchor target, only used with <see cref="Href"/>.</summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>Button content (label and/or an icon).</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Click handler.</summary>
    [Parameter]
    public EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs> OnClick { get; set; }

    /// <inheritdoc />
    protected override string SlotName => "button";

    private string? Classes => MergeClass(ButtonVariants.Get(Variant, Size));
}
