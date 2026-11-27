namespace DB.Blazor.ShadcnUI.Components;

/// <summary>The C# port of <c>badgeVariants</c> from the <c>nova</c> style's <c>badge.tsx</c>.</summary>
public static class BadgeVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "cn-badge group/badge inline-flex w-fit shrink-0 items-center justify-center overflow-hidden " +
        "whitespace-nowrap focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 " +
        "aria-invalid:border-destructive aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 " +
        "[&>svg]:pointer-events-none";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(BadgeVariant variant) => variant switch
    {
        BadgeVariant.Secondary => "cn-badge-variant-secondary",
        BadgeVariant.Destructive => "cn-badge-variant-destructive",
        BadgeVariant.Outline => "cn-badge-variant-outline",
        BadgeVariant.Ghost => "cn-badge-variant-ghost",
        BadgeVariant.Link => "cn-badge-variant-link",
        _ => "cn-badge-variant-default",
    };

    /// <summary><c>badgeVariants({ variant })</c>.</summary>
    public static string Get(BadgeVariant variant = BadgeVariant.Default) => Cn.Join(Base, Variant(variant))!;
}
