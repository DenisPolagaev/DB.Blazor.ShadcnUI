namespace DB.Blazor.ShadcnUI.Components;

/// <summary>The C# port of <c>alertVariants</c> from the <c>nova</c> style's <c>alert.tsx</c>.</summary>
public static class AlertVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base = "cn-alert group/alert relative w-full";

    /// <summary>Classes for <paramref name="variant"/>.</summary>
    public static string Variant(AlertVariant variant) => variant == AlertVariant.Destructive
        ? "cn-alert-variant-destructive"
        : "cn-alert-variant-default";

    /// <summary><c>alertVariants({ variant })</c>.</summary>
    public static string Get(AlertVariant variant = AlertVariant.Default) => Cn.Join(Base, Variant(variant))!;
}
