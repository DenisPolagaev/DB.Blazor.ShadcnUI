using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor.Components;

/// <summary>
/// A submenu inside a dropdown: owns its open state and the hover timer that closes it when the
/// pointer leaves both the sub-trigger and the sub-content.
/// </summary>
public partial class DropdownMenuSub : ShadcnComponent
{
    private bool _open;
    private CancellationTokenSource? _close;

    /// <summary><see cref="DropdownMenuSubTrigger"/> and <see cref="DropdownMenuSubContent"/>.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>True when the submenu is open.</summary>
    public bool IsOpen => _open;

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

        _open = true;
        return InvokeAsync(StateHasChanged);
    }

    /// <summary>Closes the submenu.</summary>
    public Task CloseAsync()
    {
        CancelClose();
        if (!_open)
        {
            return Task.CompletedTask;
        }

        _open = false;
        return InvokeAsync(StateHasChanged);
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
