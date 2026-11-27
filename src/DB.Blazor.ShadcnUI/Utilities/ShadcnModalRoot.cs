using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI;

/// <summary>
/// Shared open state of a modal root (dialog, alert dialog, sheet): controlled through
/// <c>Open</c>/<c>OpenChanged</c> (<c>@bind-Open</c>), uncontrolled with <c>DefaultOpen</c>.
/// </summary>
/// <remarks>
/// Closing is two-phase so the exit animation can play: the panel stays rendered with
/// <c>data-state="closed"</c> for <see cref="CloseAnimationMilliseconds"/>, and only then is it
/// removed (exactly what Radix's Presence does with the animation end event).
/// </remarks>
public abstract class ShadcnModalRoot : ShadcnComponent
{
    private bool _internalOpen;
    private bool _closing;

    /// <summary>Controlled open state (<c>@bind-Open</c>).</summary>
    [Parameter]
    public bool Open { get; set; }

    /// <summary>Open change callback; when absent the root is uncontrolled.</summary>
    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Initial state of an uncontrolled root.</summary>
    [Parameter]
    public bool DefaultOpen { get; set; }

    /// <summary>Content of the root (trigger + content).</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>True when the panel is open (not closing).</summary>
    public bool IsOpen => OpenChanged.HasDelegate ? Open : _internalOpen;

    /// <summary>True while the panel is rendered — open, or playing its exit animation.</summary>
    public bool IsVisible => IsOpen || _closing;

    /// <summary>How long the exit animation runs before the panel is removed.</summary>
    protected virtual int CloseAnimationMilliseconds => 200;

    /// <summary>The id of the panel, used for <c>aria-controls</c> and the trigger wiring.</summary>
    public string ContentId { get; private set; } = string.Empty;

    /// <summary>The id of the title, used for <c>aria-labelledby</c>.</summary>
    public string TitleId { get; private set; } = string.Empty;

    /// <summary>The id of the description, used for <c>aria-describedby</c>.</summary>
    public string DescriptionId { get; private set; } = string.Empty;

    /// <summary>The id of the trigger wrapper.</summary>
    public string TriggerId { get; private set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _internalOpen = DefaultOpen;
        ContentId = $"{ElementId}-content";
        TitleId = $"{ElementId}-title";
        DescriptionId = $"{ElementId}-description";
        TriggerId = $"{ElementId}-trigger";
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

        // Keep the panel mounted with data-state="closed" so `animate-out` can run.
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
