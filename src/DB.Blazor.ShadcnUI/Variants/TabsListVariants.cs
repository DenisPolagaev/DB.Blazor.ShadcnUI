namespace DB.Blazor.ShadcnUI.Components;

/// <summary>The C# port of <c>tabsListVariants</c> from the <c>nova</c> style's <c>tabs.tsx</c>.</summary>
public static class TabsListVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "cn-tabs-list group/tabs-list inline-flex w-fit items-center justify-center text-muted-foreground " +
        "group-data-vertical/tabs:h-fit group-data-vertical/tabs:flex-col";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(TabsListVariant variant) =>
        variant == TabsListVariant.Line
            ? "cn-tabs-list-variant-line gap-1 bg-transparent"
            : "cn-tabs-list-variant-default bg-muted";

    /// <summary><c>tabsListVariants({ variant })</c>.</summary>
    public static string Get(TabsListVariant variant = TabsListVariant.Default) => Cn.Join(Base, Variant(variant))!;
}
