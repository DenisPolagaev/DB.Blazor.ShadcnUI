namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>buttonVariants.variant</c>.</summary>
public enum ButtonVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>destructive</c></summary>
    Destructive,
    /// <summary><c>outline</c></summary>
    Outline,
    /// <summary><c>secondary</c></summary>
    Secondary,
    /// <summary><c>ghost</c></summary>
    Ghost,
    /// <summary><c>link</c></summary>
    Link,
}

/// <summary>shadcn/ui <c>buttonVariants.size</c>.</summary>
public enum ButtonSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>xs</c></summary>
    Xs,
    /// <summary><c>sm</c></summary>
    Sm,
    /// <summary><c>lg</c></summary>
    Lg,
    /// <summary><c>icon</c></summary>
    Icon,
    /// <summary><c>icon-xs</c></summary>
    IconXs,
    /// <summary><c>icon-sm</c></summary>
    IconSm,
    /// <summary><c>icon-lg</c></summary>
    IconLg,
}

/// <summary>shadcn/ui <c>badgeVariants.variant</c>.</summary>
public enum BadgeVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>secondary</c></summary>
    Secondary,
    /// <summary><c>destructive</c></summary>
    Destructive,
    /// <summary><c>outline</c></summary>
    Outline,
    /// <summary><c>ghost</c></summary>
    Ghost,
    /// <summary><c>link</c></summary>
    Link,
}

/// <summary>shadcn/ui <c>alertVariants.variant</c>.</summary>
public enum AlertVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>destructive</c></summary>
    Destructive,
}

/// <summary>shadcn/ui <c>avatar.size</c>.</summary>
public enum AvatarSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>sm</c></summary>
    Sm,
    /// <summary><c>lg</c></summary>
    Lg,
}

/// <summary>shadcn/ui <c>switch.size</c>.</summary>
public enum SwitchSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>sm</c></summary>
    Sm,
}

/// <summary>shadcn/ui <c>native-select.size</c>.</summary>
public enum NativeSelectSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>sm</c></summary>
    Sm,
}

/// <summary>shadcn/ui <c>select.size</c>.</summary>
public enum SelectSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>sm</c></summary>
    Sm,
}

/// <summary>The <c>nova</c> style's <c>Card.size</c>.</summary>
public enum CardSize
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>sm</c></summary>
    Sm,
}

/// <summary>Radix <c>SelectContent.position</c>.</summary>
public enum SelectPosition
{
    /// <summary><c>item-aligned</c> (the shadcn default): the list covers the trigger.</summary>
    ItemAligned,
    /// <summary><c>popper</c>: the list floats next to the trigger.</summary>
    Popper,
}

/// <summary>shadcn/ui sheet edge.</summary>
public enum SheetSide
{
    /// <summary><c>top</c></summary>
    Top,
    /// <summary><c>right</c></summary>
    Right,
    /// <summary><c>bottom</c></summary>
    Bottom,
    /// <summary><c>left</c></summary>
    Left,
}

/// <summary>shadcn/ui <c>tabsListVariants.variant</c>.</summary>
public enum TabsListVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>line</c></summary>
    Line,
}

/// <summary>Orientation of tabs and separators.</summary>
public enum ShadcnOrientation
{
    /// <summary><c>horizontal</c></summary>
    Horizontal,
    /// <summary><c>vertical</c></summary>
    Vertical,
}

/// <summary>shadcn/ui <c>DropdownMenuItem.variant</c>.</summary>
public enum DropdownMenuItemVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>destructive</c></summary>
    Destructive,
}

/// <summary>Maps the enums above onto the exact strings shadcn/ui renders into <c>data-*</c> attributes.</summary>
public static class ShadcnNames
{
    /// <summary><c>ButtonVariant</c> → <c>data-variant</c>.</summary>
    public static string ToDataValue(this ButtonVariant value) => value switch
    {
        ButtonVariant.Destructive => "destructive",
        ButtonVariant.Outline => "outline",
        ButtonVariant.Secondary => "secondary",
        ButtonVariant.Ghost => "ghost",
        ButtonVariant.Link => "link",
        _ => "default",
    };

    /// <summary><c>ButtonSize</c> → <c>data-size</c>.</summary>
    public static string ToDataValue(this ButtonSize value) => value switch
    {
        ButtonSize.Xs => "xs",
        ButtonSize.Sm => "sm",
        ButtonSize.Lg => "lg",
        ButtonSize.Icon => "icon",
        ButtonSize.IconXs => "icon-xs",
        ButtonSize.IconSm => "icon-sm",
        ButtonSize.IconLg => "icon-lg",
        _ => "default",
    };

    /// <summary><c>BadgeVariant</c> → <c>data-variant</c>.</summary>
    public static string ToDataValue(this BadgeVariant value) => value switch
    {
        BadgeVariant.Secondary => "secondary",
        BadgeVariant.Destructive => "destructive",
        BadgeVariant.Outline => "outline",
        BadgeVariant.Ghost => "ghost",
        BadgeVariant.Link => "link",
        _ => "default",
    };

    /// <summary><c>AvatarSize</c> → <c>data-size</c>.</summary>
    public static string ToDataValue(this AvatarSize value) => value switch
    {
        AvatarSize.Sm => "sm",
        AvatarSize.Lg => "lg",
        _ => "default",
    };

    /// <summary><c>SwitchSize</c> → <c>data-size</c>.</summary>
    public static string ToDataValue(this SwitchSize value) => value switch
    {
        SwitchSize.Sm => "sm",
        _ => "default",
    };

    /// <summary><c>NativeSelectSize</c> → <c>data-size</c>.</summary>
    public static string ToDataValue(this NativeSelectSize value) => value switch
    {
        NativeSelectSize.Sm => "sm",
        _ => "default",
    };

    /// <summary><c>SelectSize</c> → <c>data-size</c>.</summary>
    public static string ToDataValue(this SelectSize value) => value switch
    {
        SelectSize.Sm => "sm",
        _ => "default",
    };

    /// <summary><c>CardSize</c> → <c>data-size</c>.</summary>
    public static string ToDataValue(this CardSize value) => value switch
    {
        CardSize.Sm => "sm",
        _ => "default",
    };

    /// <summary><c>SheetSide</c> → the side-specific class switch.</summary>
    public static string ToDataValue(this SheetSide value) => value switch
    {
        SheetSide.Top => "top",
        SheetSide.Bottom => "bottom",
        SheetSide.Left => "left",
        _ => "right",
    };

    /// <summary><c>TabsListVariant</c> → <c>data-variant</c>.</summary>
    public static string ToDataValue(this TabsListVariant value) => value switch
    {
        TabsListVariant.Line => "line",
        _ => "default",
    };

    /// <summary><c>ShadcnOrientation</c> → <c>data-orientation</c>.</summary>
    public static string ToDataValue(this ShadcnOrientation value) =>
        value == ShadcnOrientation.Vertical ? "vertical" : "horizontal";

    /// <summary><c>DropdownMenuItemVariant</c> → <c>data-variant</c>.</summary>
    public static string ToDataValue(this DropdownMenuItemVariant value) =>
        value == DropdownMenuItemVariant.Destructive ? "destructive" : "default";

    /// <summary><c>ToggleVariant</c> → <c>data-variant</c>.</summary>
    public static string ToDataValue(this ToggleVariant value) =>
        value == ToggleVariant.Outline ? "outline" : "default";

    /// <summary><c>ToggleSize</c> → <c>data-size</c>.</summary>
    public static string ToDataValue(this ToggleSize value) => value switch
    {
        ToggleSize.Sm => "sm",
        ToggleSize.Lg => "lg",
        _ => "default",
    };
}

/// <summary>shadcn/ui <c>emptyMediaVariants.variant</c>.</summary>
public enum EmptyMediaVariant
{
    /// <summary><c>default</c></summary>
    Default,
    /// <summary><c>icon</c></summary>
    Icon,
}