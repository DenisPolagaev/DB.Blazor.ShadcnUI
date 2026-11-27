namespace DB.Blazor.ShadcnUI.Components;

/// <summary>shadcn/ui <c>fieldVariants.orientation</c>.</summary>
public enum FieldOrientation
{
    /// <summary><c>vertical</c></summary>
    Vertical,
    /// <summary><c>horizontal</c></summary>
    Horizontal,
    /// <summary><c>responsive</c></summary>
    Responsive,
}

/// <summary>shadcn/ui <c>FieldLegend.variant</c>.</summary>
public enum FieldLegendVariant
{
    /// <summary><c>legend</c></summary>
    Legend,
    /// <summary><c>label</c></summary>
    Label,
}

/// <summary>The C# port of <c>fieldVariants</c> from <c>registry/new-york-v4/ui/field.tsx</c>.</summary>
public static class FieldVariants
{
    /// <summary>The variant-independent class list.</summary>
    public const string Base = "group/field flex w-full gap-3 data-[invalid=true]:text-destructive";

    /// <summary>Classes for <paramref name="orientation"/>.</summary>
    public static string Orientation(FieldOrientation orientation) => orientation switch
    {
        FieldOrientation.Horizontal =>
            "flex-row items-center [&>[data-slot=field-label]]:flex-auto " +
            "has-[>[data-slot=field-content]]:items-start " +
            "has-[>[data-slot=field-content]]:[&>[role=checkbox],[role=radio]]:mt-px",
        FieldOrientation.Responsive =>
            "flex-col @md/field-group:flex-row @md/field-group:items-center [&>*]:w-full " +
            "@md/field-group:[&>*]:w-auto [&>.sr-only]:w-auto " +
            "@md/field-group:[&>[data-slot=field-label]]:flex-auto " +
            "@md/field-group:has-[>[data-slot=field-content]]:items-start " +
            "@md/field-group:has-[>[data-slot=field-content]]:[&>[role=checkbox],[role=radio]]:mt-px",
        _ => "flex-col [&>*]:w-full [&>.sr-only]:w-auto",
    };

    /// <summary><c>fieldVariants({ orientation })</c>.</summary>
    public static string Get(FieldOrientation orientation = FieldOrientation.Vertical) =>
        Cn.Join(Base, Orientation(orientation))!;

    /// <summary><c>data-orientation</c> value.</summary>
    public static string OrientationValue(FieldOrientation orientation) => orientation switch
    {
        FieldOrientation.Horizontal => "horizontal",
        FieldOrientation.Responsive => "responsive",
        _ => "vertical",
    };
}
