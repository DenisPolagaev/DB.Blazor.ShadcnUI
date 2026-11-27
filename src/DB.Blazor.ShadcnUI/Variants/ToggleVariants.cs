namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>toggleVariants.variant</c>.</summary>
public enum ToggleVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>outline</c></summary>
    Outline,
}

/// <summary>shadcn/ui <c>toggleVariants.size</c>.</summary>
public enum ToggleSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>sm</c></summary>
    Sm,
    /// <summary><c>lg</c></summary>
    Lg,
}

/// <summary>Radix's <c>ToggleGroup type</c>.</summary>
public enum ToggleGroupType
{
    /// <summary>One pressed item at a time.</summary>
    Single,
    /// <summary>Any number of pressed items.</summary>
    Multiple,
}

/// <summary>The C# port of <c>toggleVariants</c> from <c>registry/new-york-v4/ui/toggle.tsx</c>.</summary>
public static class ToggleVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "inline-flex items-center justify-center gap-2 rounded-md text-sm font-medium whitespace-nowrap " +
        "transition-[color,box-shadow] outline-none hover:bg-muted hover:text-muted-foreground " +
        "focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 " +
        "disabled:pointer-events-none disabled:opacity-50 aria-invalid:border-destructive " +
        "aria-invalid:ring-destructive/20 data-[state=on]:bg-accent data-[state=on]:text-accent-foreground " +
        "dark:aria-invalid:ring-destructive/40 [&_svg]:pointer-events-none [&_svg]:shrink-0 " +
        "[&_svg:not([class*='size-'])]:size-4";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(ToggleVariant variant) => variant == ToggleVariant.Outline
        ? "border border-input bg-transparent shadow-xs hover:bg-accent hover:text-accent-foreground"
        : "bg-transparent";

    /// <summary>Classes for <paramref name="size"/>.</summary>
    public static string Size(ToggleSize size) => size switch
    {
        ToggleSize.Sm => "h-8 min-w-8 px-1.5",
        ToggleSize.Lg => "h-10 min-w-10 px-2.5",
        _ => "h-9 min-w-9 px-2",
    };

    /// <summary><c>toggleVariants({ variant, size })</c>.</summary>
    public static string Get(ToggleVariant variant = ToggleVariant.Default, ToggleSize size = ToggleSize.Default) =>
        Cn.Join(Base, Variant(variant), Size(size))!;
}
