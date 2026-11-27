namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>inputGroupAddonVariants.align</c>.</summary>
public enum InputGroupAlign
{
    /// <summary><c>inline-start</c></summary>
    InlineStart,
    /// <summary><c>inline-end</c></summary>
    InlineEnd,
    /// <summary><c>block-start</c></summary>
    BlockStart,
    /// <summary><c>block-end</c></summary>
    BlockEnd,
}

/// <summary>shadcn/ui <c>inputGroupButtonVariants.size</c>.</summary>
public enum InputGroupButtonSize
{
    /// <summary><c>xs</c></summary>
    Xs,
    /// <summary><c>sm</c></summary>
    Sm,
    /// <summary><c>icon-xs</c></summary>
    IconXs,
    /// <summary><c>icon-sm</c></summary>
    IconSm,
}

/// <summary>The C# port of <c>inputGroupAddonVariants</c> and <c>inputGroupButtonVariants</c>.</summary>
public static class InputGroupVariants
{
    /// <summary>The addon base class list.</summary>
    public const string AddonBase =
        "flex h-auto cursor-text items-center justify-center gap-2 py-1.5 text-sm font-medium " +
        "text-muted-foreground select-none group-data-[disabled=true]/input-group:opacity-50 " +
        "[&>kbd]:rounded-[calc(var(--radius)-5px)] [&>svg:not([class*='size-'])]:size-4";

    /// <summary>Addon classes for <paramref name="align"/>.</summary>
    public static string AddonAlign(InputGroupAlign align) => align switch
    {
        InputGroupAlign.InlineEnd => "order-last pr-3 has-[>button]:mr-[-0.45rem] has-[>kbd]:mr-[-0.35rem]",
        InputGroupAlign.BlockStart =>
            "order-first w-full justify-start px-3 pt-3 group-has-[>input]/input-group:pt-2.5 [.border-b]:pb-3",
        InputGroupAlign.BlockEnd =>
            "order-last w-full justify-start px-3 pb-3 group-has-[>input]/input-group:pb-2.5 [.border-t]:pt-3",
        _ => "order-first pl-3 has-[>button]:ml-[-0.45rem] has-[>kbd]:ml-[-0.35rem]",
    };

    /// <summary>Addon value for <c>data-align</c>.</summary>
    public static string AddonAlignValue(InputGroupAlign align) => align switch
    {
        InputGroupAlign.InlineEnd => "inline-end",
        InputGroupAlign.BlockStart => "block-start",
        InputGroupAlign.BlockEnd => "block-end",
        _ => "inline-start",
    };

    /// <summary>The button base class list.</summary>
    public const string ButtonBase = "flex items-center gap-2 text-sm shadow-none";

    /// <summary>Button classes for <paramref name="size"/>.</summary>
    public static string ButtonSize(InputGroupButtonSize size) => size switch
    {
        InputGroupButtonSize.Sm => "h-8 gap-1.5 rounded-md px-2.5 has-[>svg]:px-2.5",
        InputGroupButtonSize.IconXs => "size-6 rounded-[calc(var(--radius)-5px)] p-0 has-[>svg]:p-0",
        InputGroupButtonSize.IconSm => "size-8 p-0 has-[>svg]:p-0",
        _ => "h-6 gap-1 rounded-[calc(var(--radius)-5px)] px-2 has-[>svg]:px-2 [&>svg:not([class*='size-'])]:size-3.5",
    };

    /// <summary>Button value for <c>data-size</c>.</summary>
    public static string ButtonSizeValue(InputGroupButtonSize size) => size switch
    {
        InputGroupButtonSize.Sm => "sm",
        InputGroupButtonSize.IconXs => "icon-xs",
        InputGroupButtonSize.IconSm => "icon-sm",
        _ => "xs",
    };
}
