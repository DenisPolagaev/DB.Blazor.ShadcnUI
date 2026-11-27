namespace DB.Blazor.ShadcnUI.Components;

/// <summary>The C# port of <c>buttonGroupVariants</c> from the <c>nova</c> style's <c>button-group.tsx</c>.</summary>
public static class ButtonGroupVariants
{
    /// <summary>The orientation-independent class list.</summary>
    public const string Base =
        "cn-button-group group/button-group flex w-fit items-stretch *:focus-visible:relative " +
        "*:focus-visible:z-10 [&>[data-slot=select-trigger]:not([class*='w-'])]:w-fit [&>input]:flex-1";

    /// <summary>Classes for <paramref name="orientation"/>.</summary>
    public static string Orientation(ShadcnOrientation orientation) =>
        orientation == ShadcnOrientation.Vertical
            ? "cn-button-group-orientation-vertical flex-col [&>*:not(:first-child)]:rounded-t-none " +
              "[&>*:not(:first-child)]:border-t-0 [&>*:not(:last-child)]:rounded-b-none"
            : "cn-button-group-orientation-horizontal [&>*:not(:first-child)]:rounded-l-none " +
              "[&>*:not(:first-child)]:border-l-0 [&>*:not(:last-child)]:rounded-r-none";

    /// <summary><c>buttonGroupVariants({ orientation })</c>.</summary>
    public static string Get(ShadcnOrientation orientation = ShadcnOrientation.Horizontal) =>
        Cn.Join(Base, Orientation(orientation))!;
}
