namespace DB.Blazor.ShadcnUI.Components;

/// <summary>The C# port of <c>tabsListVariants</c> from <c>registry/new-york-v4/ui/tabs.tsx</c>.</summary>
public static class TabsListVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "group/tabs-list inline-flex w-fit items-center justify-center rounded-lg p-[3px] text-muted-foreground " +
        "group-data-[orientation=horizontal]/tabs:h-9 group-data-[orientation=vertical]/tabs:h-fit " +
        "group-data-[orientation=vertical]/tabs:flex-col data-[variant=line]:rounded-none";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(TabsListVariant variant) =>
        variant == TabsListVariant.Line ? "gap-1 bg-transparent" : "bg-muted";

    /// <summary><c>tabsListVariants({ variant })</c>.</summary>
    public static string Get(TabsListVariant variant = TabsListVariant.Default) => Cn.Join(Base, Variant(variant))!;
}
