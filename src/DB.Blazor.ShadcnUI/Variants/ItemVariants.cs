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

/// <summary>The C# port of <c>itemVariants</c> from <c>registry/new-york-v4/ui/item.tsx</c>.</summary>
public static class ItemVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "group/item flex flex-wrap items-center rounded-md border border-transparent text-sm " +
        "transition-colors duration-100 outline-none focus-visible:border-ring focus-visible:ring-[3px] " +
        "focus-visible:ring-ring/50 [a]:transition-colors [a]:hover:bg-accent/50";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(ItemVariant variant) => variant switch
    {
        ItemVariant.Outline => "border-border",
        ItemVariant.Muted => "bg-muted/50",
        _ => "bg-transparent",
    };

    /// <summary>Classes for <paramref name="size"/>.</summary>
    public static string Size(ItemSize size) => size == ItemSize.Sm ? "gap-2.5 px-4 py-3" : "gap-4 p-4";

    /// <summary><c>itemVariants({ variant, size })</c>.</summary>
    public static string Get(ItemVariant variant = ItemVariant.Default, ItemSize size = ItemSize.Default) =>
        Cn.Join(Base, Variant(variant), Size(size))!;
}

/// <summary>The C# port of <c>itemMediaVariants</c>.</summary>
public static class ItemMediaVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "flex shrink-0 items-center justify-center gap-2 " +
        "group-has-[[data-slot=item-description]]/item:translate-y-0.5 " +
        "group-has-[[data-slot=item-description]]/item:self-start [&_svg]:pointer-events-none";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(ItemMediaVariant variant) => variant switch
    {
        ItemMediaVariant.Icon => "size-8 rounded-sm border bg-muted [&_svg:not([class*='size-'])]:size-4",
        ItemMediaVariant.Image => "size-10 overflow-hidden rounded-sm [&_img]:size-full [&_img]:object-cover",
        _ => "bg-transparent",
    };

    /// <summary><c>itemMediaVariants({ variant })</c>.</summary>
    public static string Get(ItemMediaVariant variant = ItemMediaVariant.Default) => Cn.Join(Base, Variant(variant))!;
}

/// <summary>The C# port of <c>buttonGroupVariants</c> from <c>registry/new-york-v4/ui/button-group.tsx</c>.</summary>
public static class ButtonGroupVariants
{
    /// <summary>The orientation-independent class list.</summary>
    public const string Base =
        "flex w-fit items-stretch has-[>[data-slot=button-group]]:gap-2 [&>*]:focus-visible:relative " +
        "[&>*]:focus-visible:z-10 has-[select[aria-hidden=true]:last-child]:[&>[data-slot=select-trigger]:last-of-type]:rounded-r-md " +
        "[&>[data-slot=select-trigger]:not([class*='w-'])]:w-fit [&>input]:flex-1";

    /// <summary>Classes for <paramref name="orientation"/>.</summary>
    public static string Orientation(ShadcnOrientation orientation) =>
        orientation == ShadcnOrientation.Vertical
            ? "flex-col [&>*:not(:first-child)]:rounded-t-none [&>*:not(:first-child)]:border-t-0 [&>*:not(:last-child)]:rounded-b-none"
            : "[&>*:not(:first-child)]:rounded-l-none [&>*:not(:first-child)]:border-l-0 [&>*:not(:last-child)]:rounded-r-none";

    /// <summary><c>buttonGroupVariants({ orientation })</c>.</summary>
    public static string Get(ShadcnOrientation orientation = ShadcnOrientation.Horizontal) =>
        Cn.Join(Base, Orientation(orientation))!;
}
