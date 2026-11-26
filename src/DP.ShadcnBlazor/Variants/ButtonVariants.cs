namespace DP.ShadcnBlazor.Components;

/// <summary>
/// The C# port of shadcn/ui's <c>buttonVariants</c> cva definition. Class strings are copied
/// verbatim from <c>registry/new-york-v4/ui/button.tsx</c>; only the cva plumbing is replaced
/// with a switch expression.
/// </summary>
public static class ButtonVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "inline-flex shrink-0 items-center justify-center gap-2 rounded-md text-sm font-medium whitespace-nowrap " +
        "transition-all outline-none focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 " +
        "disabled:pointer-events-none disabled:opacity-50 aria-invalid:border-destructive aria-invalid:ring-destructive/20 " +
        "dark:aria-invalid:ring-destructive/40 [&_svg]:pointer-events-none [&_svg]:shrink-0 " +
        "[&_svg:not([class*='size-'])]:size-4";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(ButtonVariant variant) => variant switch
    {
        ButtonVariant.Destructive =>
            "bg-destructive text-white hover:bg-destructive/90 focus-visible:ring-destructive/20 dark:bg-destructive/60 dark:focus-visible:ring-destructive/40",
        ButtonVariant.Outline =>
            "border bg-background shadow-xs hover:bg-accent hover:text-accent-foreground dark:border-input dark:bg-input/30 dark:hover:bg-input/50",
        ButtonVariant.Secondary => "bg-secondary text-secondary-foreground hover:bg-secondary/80",
        ButtonVariant.Ghost => "hover:bg-accent hover:text-accent-foreground dark:hover:bg-accent/50",
        ButtonVariant.Link => "text-primary underline-offset-4 hover:underline",
        _ => "bg-primary text-primary-foreground hover:bg-primary/90",
    };

    /// <summary>Classes for <paramref name="size"/>.</summary>
    public static string Size(ButtonSize size) => size switch
    {
        ButtonSize.Xs => "h-6 gap-1 rounded-md px-2 text-xs has-[>svg]:px-1.5 [&_svg:not([class*='size-'])]:size-3",
        ButtonSize.Sm => "h-8 gap-1.5 rounded-md px-3 has-[>svg]:px-2.5",
        ButtonSize.Lg => "h-10 rounded-md px-6 has-[>svg]:px-4",
        ButtonSize.Icon => "size-9",
        ButtonSize.IconXs => "size-6 rounded-md [&_svg:not([class*='size-'])]:size-3",
        ButtonSize.IconSm => "size-8",
        ButtonSize.IconLg => "size-10",
        _ => "h-9 px-4 py-2 has-[>svg]:px-3",
    };

    /// <summary><c>buttonVariants({ variant, size })</c>.</summary>
    public static string Get(ButtonVariant variant = ButtonVariant.Default, ButtonSize size = ButtonSize.Default) =>
        Cn.Join(Base, Variant(variant), Size(size))!;
}
