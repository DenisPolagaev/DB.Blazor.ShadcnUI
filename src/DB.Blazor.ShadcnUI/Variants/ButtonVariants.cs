namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// The C# port of shadcn/ui's <c>buttonVariants</c> cva definition for the <c>nova</c> style.
/// Class strings are copied verbatim from <c>registry/bases/radix/ui/button.tsx</c>; only the cva
/// plumbing is replaced with a switch expression.
/// </summary>
public static class ButtonVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "cn-button group/button inline-flex shrink-0 items-center justify-center whitespace-nowrap " +
        "transition-all outline-none select-none disabled:pointer-events-none disabled:opacity-50 " +
        "[&_svg]:pointer-events-none [&_svg]:shrink-0";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(ButtonVariant variant) => variant switch
    {
        ButtonVariant.Destructive => "cn-button-variant-destructive",
        ButtonVariant.Outline => "cn-button-variant-outline",
        ButtonVariant.Secondary => "cn-button-variant-secondary",
        ButtonVariant.Ghost => "cn-button-variant-ghost",
        ButtonVariant.Link => "cn-button-variant-link",
        _ => "cn-button-variant-default",
    };

    /// <summary>Classes for <paramref name="size"/>.</summary>
    public static string Size(ButtonSize size) => size switch
    {
        ButtonSize.Xs => "cn-button-size-xs",
        ButtonSize.Sm => "cn-button-size-sm",
        ButtonSize.Lg => "cn-button-size-lg",
        ButtonSize.Icon => "cn-button-size-icon",
        ButtonSize.IconXs => "cn-button-size-icon-xs",
        ButtonSize.IconSm => "cn-button-size-icon-sm",
        ButtonSize.IconLg => "cn-button-size-icon-lg",
        _ => "cn-button-size-default",
    };

    /// <summary><c>buttonVariants({ variant, size })</c>.</summary>
    public static string Get(ButtonVariant variant = ButtonVariant.Default, ButtonSize size = ButtonSize.Default) =>
        Cn.Join(Base, Variant(variant), Size(size))!;
}
