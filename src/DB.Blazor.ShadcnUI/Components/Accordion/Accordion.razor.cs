using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>Radix's <c>Accordion.Root type</c>.</summary>
public enum AccordionType
{
    /// <summary>Only one item open at a time (<c>collapsible</c> governs closing it).</summary>
    Single,
    /// <summary>Any number of items open.</summary>
    Multiple,
}

/// <summary>
/// shadcn/ui <c>Accordion</c> root. Controlled (<see cref="Value"/>/<see cref="Values"/>) or
/// uncontrolled (<see cref="DefaultValue"/>/<see cref="DefaultValues"/>), like Radix.
/// </summary>
public partial class Accordion : ShadcnComponent
{
    private readonly List<string> _uncontrolledValues = [];
    private string? _uncontrolledValue;
    private bool _hasUncontrolledValue;

    /// <summary><c>single</c> (default) or <c>multiple</c>.</summary>
    [Parameter]
    public AccordionType Type { get; set; } = AccordionType.Single;

    /// <summary>For <c>single</c>: whether the open item can be closed again.</summary>
    [Parameter]
    public bool Collapsible { get; set; }

    /// <summary>Controlled single value.</summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>Single value change callback.</summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Controlled multiple values.</summary>
    [Parameter]
    public IReadOnlyList<string>? Values { get; set; }

    /// <summary>Multiple value change callback.</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<string>> ValuesChanged { get; set; }

    /// <summary>Initial single value for an uncontrolled accordion.</summary>
    [Parameter]
    public string? DefaultValue { get; set; }

    /// <summary>Initial values for an uncontrolled <c>multiple</c> accordion.</summary>
    [Parameter]
    public IReadOnlyList<string>? DefaultValues { get; set; }

    /// <summary><see cref="AccordionItem"/> children.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Root class list for the <c>nova</c> style.</summary>
    internal string? RootClasses => Cn.Merge("cn-accordion flex w-full flex-col", Class);

    /// <summary>True when the item with <paramref name="value"/> is expanded.</summary>
    public bool IsOpen(string? value)
    {
        if (value is null)
        {
            return false;
        }

        return Type == AccordionType.Multiple
            ? (Values ?? _uncontrolledValues).Contains(value, StringComparer.Ordinal)
            : string.Equals(CurrentSingle, value, StringComparison.Ordinal);    }

    private string? CurrentSingle => _hasUncontrolledValue ? _uncontrolledValue : Value ?? DefaultValue;

    internal async Task ToggleAsync(string? value)
    {
        if (value is null)
        {
            return;
        }

        if (Type == AccordionType.Multiple)
        {
            var current = (Values ?? _uncontrolledValues).ToArray();            var next = current.Contains(value, StringComparer.Ordinal)
                ? current.Where(item => !string.Equals(item, value, StringComparison.Ordinal)).ToArray()
                : current.Append(value).ToArray();

            _uncontrolledValues.Clear();
            foreach (var item in next)
            {
                _uncontrolledValues.Add(item);
            }

            await ValuesChanged.InvokeAsync(next);
            StateHasChanged();
            return;
        }

        if (IsOpen(value))
        {
            if (!Collapsible)
            {
                return;
            }

            _hasUncontrolledValue = true;
            _uncontrolledValue = null;
            await ValueChanged.InvokeAsync(null);
            StateHasChanged();
            return;
        }

        _hasUncontrolledValue = true;
        _uncontrolledValue = value;
        await ValueChanged.InvokeAsync(value);
        StateHasChanged();
    }
}
