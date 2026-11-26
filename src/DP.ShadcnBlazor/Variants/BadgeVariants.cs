namespace DP.ShadcnBlazor.Components;

/// <summary>The C# port of <c>badgeVariants</c> from <c>registry/new-york-v4/ui/badge.tsx</c>.</summary>
public static class BadgeVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "inline-flex w-fit shrink-0 items-center justify-center gap-1 overflow-hidden rounded-full border border-transparent " +
        "px-2 py-0.5 text-xs font-medium whitespace-nowrap transition-[color,box-shadow] focus-visible:border-ring " +
        "focus-visible:ring-[3px] focus-visible:ring-ring/50 aria-invalid:border-destructive aria-invalid:ring-destructive/20 " +
        "dark:aria-invalid:ring-destructive/40 [&>svg]:pointer-events-none [&>svg]:size-3";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(BadgeVariant variant) => variant switch
    {
        BadgeVariant.Secondary => "bg-secondary text-secondary-foreground [a&]:hover:bg-secondary/90",
        BadgeVariant.Destructive =>
            "bg-destructive text-white focus-visible:ring-destructive/20 dark:bg-destructive/60 dark:focus-visible:ring-destructive/40 [a&]:hover:bg-destructive/90",
        BadgeVariant.Outline => "border-border text-foreground [a&]:hover:bg-accent [a&]:hover:text-accent-foreground",
        BadgeVariant.Ghost => "[a&]:hover:bg-accent [a&]:hover:text-accent-foreground",
        BadgeVariant.Link => "text-primary underline-offset-4 [a&]:hover:underline",
        _ => "bg-primary text-primary-foreground [a&]:hover:bg-primary/90",
    };

    /// <summary><c>badgeVariants({ variant })</c>.</summary>
    public static string Get(BadgeVariant variant = BadgeVariant.Default) => Cn.Join(Base, Variant(variant))!;
}
