using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// shadcn/ui <c>InputOTP</c>. Radix renders one real input and mirrors its characters into slots;
/// the Blazor port keeps that shape: the input is transparent and the slots paint the value.
/// </summary>
public partial class InputOTP : ShadcnComponent
{
    private string? _internalValue;
    private bool _focused;

    /// <summary>Two-way bound value (<c>@bind-Value</c>).</summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>Value change callback.</summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Number of slots.</summary>
    [Parameter]
    public int Length { get; set; } = 6;

    /// <summary>Disables the control.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>The current value, controlled or internal.</summary>
    public string Current => Value ?? _internalValue ?? string.Empty;

    /// <summary>The character of the slot at <paramref name="index"/>.</summary>
    public string CharAt(int index) => index >= 0 && index < Current.Length ? Current[index].ToString() : string.Empty;

    /// <summary>True when the slot is the one being edited.</summary>
    public bool IsActive(int index) => _focused && index == Math.Min(Current.Length, Length - 1);

    internal async Task SetValueAsync(string? value)
    {
        var normalized = (value ?? string.Empty).Replace(" ", string.Empty);
        if (normalized.Length > Length)
        {
            normalized = normalized[..Length];
        }

        _internalValue = normalized;
        if (ValueChanged.HasDelegate)
        {
            Value = normalized;
        }

        await ValueChanged.InvokeAsync(normalized);
        StateHasChanged();
    }

    internal void SetFocused(bool focused)
    {
        _focused = focused;
        StateHasChanged();
    }
}
