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

/// <summary>The C# port of <c>toggleVariants</c> from the <c>nova</c> style's <c>toggle.tsx</c>.</summary>
public static class ToggleVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "cn-toggle group/toggle inline-flex items-center justify-center whitespace-nowrap outline-none " +
        "hover:bg-muted focus-visible:ring-[3px] " +
        "disabled:pointer-events-none disabled:opacity-50 " +
        "[&_svg]:pointer-events-none [&_svg]:shrink-0";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(ToggleVariant variant) => variant == ToggleVariant.Outline
        ? "cn-toggle-variant-outline"
        : "cn-toggle-variant-default";

    /// <summary>Classes for <paramref name="size"/>.</summary>
    public static string Size(ToggleSize size) => size switch
    {
        ToggleSize.Sm => "cn-toggle-size-sm",
        ToggleSize.Lg => "cn-toggle-size-lg",
        _ => "cn-toggle-size-default",
    };

    /// <summary><c>toggleVariants({ variant, size })</c>.</summary>
    public static string Get(ToggleVariant variant = ToggleVariant.Default, ToggleSize size = ToggleSize.Default) =>
        Cn.Join(Base, Variant(variant), Size(size))!;
}


