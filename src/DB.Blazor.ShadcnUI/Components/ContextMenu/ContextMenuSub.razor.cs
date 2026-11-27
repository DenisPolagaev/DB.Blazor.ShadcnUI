using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// A submenu inside a context menu: owns its open state, the hover grace period and the two-phase
/// close, exactly like <see cref="DropdownMenuSub"/>.
/// </summary>
public partial class ContextMenuSub : ShadcnComponent
{
    private bool _open;
    private bool _closing;
    private CancellationTokenSource? _close;

    /// <summary><see cref="ContextMenuSubTrigger"/> and <see cref="ContextMenuSubContent"/>.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>True when the submenu is open.</summary>
    public bool IsOpen => _open;

    /// <summary>True while the submenu is rendered — open, or playing its exit animation.</summary>
    public bool IsVisible => _open || _closing;

    /// <summary>The id of the renderless sub-trigger wrapper.</summary>
    public string TriggerId { get; private set; } = string.Empty;

    /// <summary>The id of the submenu panel.</summary>
    public string ContentId { get; private set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        TriggerId = $"{ElementId}-trigger";
        ContentId = $"{ElementId}-content";
    }

    /// <summary>Opens the submenu.</summary>
    public Task ShowAsync()
    {
        CancelClose();
        if (_open)
        {
            return Task.CompletedTask;
        }

        _closing = false;
        _open = true;
        return InvokeAsync(StateHasChanged);
    }

    /// <summary>Closes the submenu with its exit animation.</summary>
    public async Task CloseAsync()
    {
        CancelClose();
        if (!_open || _closing)
        {
            return;
        }

        _closing = true;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(150);
        _closing = false;
        _open = false;
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>Flips the submenu.</summary>
    public Task ToggleAsync() => _open ? CloseAsync() : ShowAsync();

    /// <summary>Closes the submenu after the hover grace period.</summary>
    public void ScheduleClose()
    {
        _close?.Cancel();
        _close?.Dispose();
        _close = new CancellationTokenSource();
        _ = CloseAfterAsync(_close.Token);
    }

    /// <summary>Cancels a scheduled close (the pointer entered the submenu).</summary>
    public void CancelClose()
    {
        _close?.Cancel();
        _close?.Dispose();
        _close = null;
    }

    private async Task CloseAfterAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(150, token);
            if (!token.IsCancellationRequested)
            {
                await CloseAsync();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _close?.Cancel();
        _close?.Dispose();
    }
}
