namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>itemVariants.variant</c>.</summary>
public enum ItemVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>outline</c></summary>
    Outline,
    /// <summary><c>muted</c></summary>
    Muted,
}

/// <summary>shadcn/ui <c>itemVariants.size</c>.</summary>
public enum ItemSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>sm</c></summary>
    Sm,
    /// <summary><c>xs</c></summary>
    Xs,
}

/// <summary>shadcn/ui <c>itemMediaVariants.variant</c>.</summary>
public enum ItemMediaVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>icon</c></summary>
    Icon,
    /// <summary><c>image</c></summary>
    Image,
}

/// <summary>The C# port of <c>itemVariants</c> from the <c>nova</c> style's <c>item.tsx</c>.</summary>
public static class ItemVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "cn-item group/item flex w-full flex-wrap items-center transition-colors duration-100 outline-none " +
        "focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 [a]:transition-colors";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(ItemVariant variant) => variant switch
    {
        ItemVariant.Outline => "cn-item-variant-outline",
        ItemVariant.Muted => "cn-item-variant-muted",
        _ => "cn-item-variant-default",
    };

    /// <summary>Classes for <paramref name="size"/>.</summary>
    public static string Size(ItemSize size) => size switch
    {
        ItemSize.Sm => "cn-item-size-sm",
        ItemSize.Xs => "cn-item-size-xs",
        _ => "cn-item-size-default",
    };

    /// <summary><c>itemVariants({ variant, size })</c>.</summary>
    public static string Get(ItemVariant variant = ItemVariant.Default, ItemSize size = ItemSize.Default) =>
        Cn.Join(Base, Variant(variant), Size(size))!;
}

/// <summary>The C# port of <c>itemMediaVariants</c> for the <c>nova</c> style.</summary>
public static class ItemMediaVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base = "cn-item-media flex shrink-0 items-center justify-center [&_svg]:pointer-events-none";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(ItemMediaVariant variant) => variant switch
    {
        ItemMediaVariant.Icon => "cn-item-media-variant-icon",
        ItemMediaVariant.Image => "cn-item-media-variant-image",
        _ => "cn-item-media-variant-default",
    };

    /// <summary><c>itemMediaVariants({ variant })</c>.</summary>
    public static string Get(ItemMediaVariant variant = ItemMediaVariant.Default) => Cn.Join(Base, Variant(variant))!;
}

