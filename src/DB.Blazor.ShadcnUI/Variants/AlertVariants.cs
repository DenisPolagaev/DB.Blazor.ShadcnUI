namespace DB.Blazor.ShadcnUI.Components;

/// <summary>The C# port of <c>alertVariants</c> from <c>registry/new-york-v4/ui/alert.tsx</c>.</summary>
public static class AlertVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base =
        "relative grid w-full grid-cols-[0_1fr] items-start gap-y-0.5 rounded-lg border px-4 py-3 text-sm " +
        "has-[>svg]:grid-cols-[calc(var(--spacing)*4)_1fr] has-[>svg]:gap-x-3 [&>svg]:size-4 [&>svg]:translate-y-0.5 [&>svg]:text-current";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(AlertVariant variant) => variant == AlertVariant.Destructive
        ? "bg-card text-destructive *:data-[slot=alert-description]:text-destructive/90 [&>svg]:text-current"
        : "bg-card text-card-foreground";

    /// <summary><c>alertVariants({ variant })</c>.</summary>
    public static string Get(AlertVariant variant = AlertVariant.Default) => Cn.Join(Base, Variant(variant))!;
}
