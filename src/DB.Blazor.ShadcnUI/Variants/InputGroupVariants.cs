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

/// <summary>The C# port of the <c>nova</c> style's input-group variants.</summary>
public static class InputGroupVariants
{
    /// <summary>The addon base class list.</summary>
    public const string AddonBase = "cn-input-group-addon flex cursor-text items-center justify-center select-none";

    /// <summary>Addon classes for <paramref name="align"/>.</summary>
    public static string AddonAlign(InputGroupAlign align) => align switch
    {
        InputGroupAlign.InlineEnd => "cn-input-group-addon-align-inline-end order-last",
        InputGroupAlign.BlockStart => "cn-input-group-addon-align-block-start order-first w-full justify-start",
        InputGroupAlign.BlockEnd => "cn-input-group-addon-align-block-end order-last w-full justify-start",
        _ => "cn-input-group-addon-align-inline-start order-first",
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
    public const string ButtonBase = "cn-input-group-button flex items-center shadow-none";

    /// <summary>Button classes for <paramref name="size"/>.</summary>
    public static string ButtonSize(InputGroupButtonSize size) => size switch
    {
        InputGroupButtonSize.Sm => "cn-input-group-button-size-sm",
        InputGroupButtonSize.IconXs => "cn-input-group-button-size-icon-xs",
        InputGroupButtonSize.IconSm => "cn-input-group-button-size-icon-sm",
        _ => "cn-input-group-button-size-xs",
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
