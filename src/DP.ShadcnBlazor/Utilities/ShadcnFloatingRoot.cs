using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor;

/// <summary>
/// Shared open state of a floating root (popover, tooltip, dropdown menu): controlled through
/// <c>Open</c>/<c>OpenChanged</c> (<c>@bind-Open</c>), uncontrolled with <c>DefaultOpen</c>.
/// </summary>
public abstract class ShadcnFloatingRoot : ShadcnComponent
{
    private bool _internalOpen;

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

    /// <summary>Closes the panel.</summary>
    public Task CloseAsync() => SetOpenAsync(false);

    /// <summary>Flips the panel.</summary>
    public Task ToggleAsync() => SetOpenAsync(!IsOpen);

    /// <summary>Sets the open state and notifies the consumer.</summary>
    public async Task SetOpenAsync(bool value)
    {
        if (IsOpen == value)
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
