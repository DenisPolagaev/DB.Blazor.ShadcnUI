using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// Radix's <c>Collapsible</c>: controlled through <c>Open</c>/<c>OpenChanged</c>, uncontrolled with
/// <c>DefaultOpen</c>. The content is only mounted while open, exactly like Radix without
/// <c>forceMount</c>.
/// </summary>
public partial class Collapsible : ShadcnComponent
{
    private bool _internalOpen;

    /// <summary>Controlled open state.</summary>
    [Parameter]
    public bool Open { get; set; }

    /// <summary>Open change callback.</summary>
    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Initial state of an uncontrolled collapsible.</summary>
    [Parameter]
    public bool DefaultOpen { get; set; }

    /// <summary>Disables the trigger.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Trigger + content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The id of the content panel, used by <c>aria-controls</c>.</summary>
    public string ContentId { get; private set; } = string.Empty;

    /// <summary>True when the content is shown.</summary>
    public bool IsOpen => OpenChanged.HasDelegate ? Open : _internalOpen;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _internalOpen = DefaultOpen;
        ContentId = $"{ElementId}-content";
    }

    /// <summary>Opens the collapsible.</summary>
    public Task ShowAsync() => SetOpenAsync(true);

    /// <summary>Closes the collapsible.</summary>
    public Task CloseAsync() => SetOpenAsync(false);

    /// <summary>Flips the collapsible.</summary>
    public Task ToggleAsync() => SetOpenAsync(!IsOpen);

    internal async Task SetOpenAsync(bool value)
    {
        if (IsOpen == value || Disabled)
        {
            return;
        }

        _internalOpen = value;
        if (OpenChanged.HasDelegate)
        {
            Open = value;
        }

        await OpenChanged.InvokeAsync(value);
        await InvokeAsync(StateHasChanged);
    }
}
