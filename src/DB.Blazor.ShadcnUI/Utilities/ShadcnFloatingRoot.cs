using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI;

/// <summary>
/// Shared open state of a floating root (popover, tooltip, dropdown menu): controlled through
/// <c>Open</c>/<c>OpenChanged</c> (<c>@bind-Open</c>), uncontrolled with <c>DefaultOpen</c>.
/// </summary>
/// <remarks>
/// Like <see cref="ShadcnModalRoot"/>, closing is two-phase: the panel stays rendered with
/// <c>data-state="closed"</c> for the exit animation, then is removed.
/// </remarks>
public abstract class ShadcnFloatingRoot : ShadcnComponent
{
    private bool _internalOpen;
    private bool _closing;

    /// <summary>Controlled open state.</summary>
    [Parameter]
    public bool Open { get; set; }

    /// <summary>Open change callback; when absent the root is uncontrolled.</summary>
    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Initial state of an uncontrolled root.</summary>
    [Parameter]
    public bool DefaultOpen { get; set; }

    /// <summary>Trigger + content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>True when the panel is open.</summary>
    public bool IsOpen => OpenChanged.HasDelegate ? Open : _internalOpen;

    /// <summary>True while the panel is rendered — open, or playing its exit animation.</summary>
    public bool IsVisible => IsOpen || _closing;

    /// <summary>How long the exit animation runs before the panel is removed.</summary>
    protected virtual int CloseAnimationMilliseconds => 150;

    /// <summary>The id of the renderless trigger wrapper.</summary>
    public string TriggerId { get; private set; } = string.Empty;

    /// <summary>The id of the panel.</summary>
    public string ContentId { get; private set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _internalOpen = DefaultOpen;
        TriggerId = $"{ElementId}-trigger";
        ContentId = $"{ElementId}-content";
    }

    /// <summary>Opens the panel.</summary>
    public Task ShowAsync() => SetOpenAsync(true);

    /// <summary>Closes the panel (with the exit animation).</summary>
    public Task CloseAsync() => SetOpenAsync(false);

    /// <summary>Flips the panel.</summary>
    public Task ToggleAsync() => SetOpenAsync(!IsOpen);

    /// <summary>Sets the open state and notifies the consumer.</summary>
    public async Task SetOpenAsync(bool value)
    {
        if (value)
        {
            if (IsOpen && !_closing)
            {
                return;
            }

            _closing = false;
            _internalOpen = true;
            if (OpenChanged.HasDelegate)
            {
                Open = true;
            }

            await OpenChanged.InvokeAsync(true);
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (!IsOpen || _closing)
        {
            return;
        }

        _closing = true;
        await InvokeAsync(StateHasChanged);

        await Task.Delay(CloseAnimationMilliseconds);

        _closing = false;
        _internalOpen = false;
        if (OpenChanged.HasDelegate)
        {
            Open = false;
        }

        await OpenChanged.InvokeAsync(false);
        await InvokeAsync(StateHasChanged);
    }
}
