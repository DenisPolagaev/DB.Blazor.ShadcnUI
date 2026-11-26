using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DP.ShadcnBlazor;

/// <summary>
/// Base of the non-modal floating panels (popover, dropdown menu): places the panel next to its
/// trigger, closes on Escape or an outside pointer press, and gives menus their arrow-key
/// navigation.
/// </summary>
/// <remarks>
/// The content component stays alive while its root is closed (only the markup is conditional), so
/// the JS state is attached from <see cref="SyncAsync"/> on every render rather than from
/// <c>OnAfterRenderAsync(firstRender)</c> — at first render a popover is closed and has no element.
/// </remarks>
public abstract class ShadcnDismissableBase : ShadcnComponent, IAsyncDisposable
{
    private IJSObjectReference? _module;
    private DotNetObjectReference<ShadcnDismissableBase>? _self;
    private ElementReference _panel;
    private bool _attached;
    private bool _disposed;

    private string? _triggerId;
    private string? _contentId;
    private string _side = "bottom";
    private string _align = "center";
    private double _sideOffset = 4;
    private string? _originVar;
    private string? _availableHeightVar;
    private string _focus = "first";

    /// <summary>Injected JS runtime.</summary>
    [Inject]
    protected IJSRuntime Js { get; set; } = default!;

    /// <summary>Called by the browser on Escape or an outside click.</summary>
    [JSInvokable]
    public Task DismissAsync() => RequestDismissAsync();

    /// <summary>Closes the panel (implemented by the panel: it asks its root to close).</summary>
    protected abstract Task RequestDismissAsync();

    /// <summary>Sets the placement used the next time the panel is shown.</summary>
    protected void ConfigurePosition(
        string? triggerId,
        string side = "bottom",
        string align = "center",
        double sideOffset = 4,
        string? originVar = null,
        string? availableHeightVar = null,
        string focus = "first",
        string? contentId = null)
    {
        _triggerId = triggerId;
        _side = side;
        _align = align;
        _sideOffset = sideOffset;
        _originVar = originVar;
        _availableHeightVar = availableHeightVar;
        _focus = focus;
        _contentId = contentId;
    }

    /// <summary>Attaches the panel while <paramref name="open"/> is true, detaches it otherwise.</summary>
    protected Task SyncAsync(ElementReference panel, bool open) => open ? ShowAsync(panel) : HideAsync();

    private async Task ShowAsync(ElementReference panel)
    {
        if (_attached || _disposed)
        {
            return;
        }

        _panel = panel;
        _attached = true;

        try
        {
            _module ??= await ShadcnJs.ImportAsync(Js);
            _self ??= DotNetObjectReference.Create(this);
            await _module.InvokeVoidAsync("openDismissable", panel, _self, new
            {
                triggerId = _triggerId,
                side = _side,
                align = _align,
                sideOffset = _sideOffset,
                originVar = _originVar,
                availableHeightVar = _availableHeightVar,
                focus = _focus,
            });
        }
        catch (Exception exception) when (exception is JSException or InvalidOperationException or JSDisconnectedException or ObjectDisposedException)
        {
        }
    }

    private async Task HideAsync()
    {
        if (!_attached)
        {
            return;
        }

        _attached = false;
        try
        {
            if (_module is not null)
            {
                await _module.InvokeVoidAsync("closeDismissable", _contentId);
            }
        }
        catch (Exception exception) when (exception is JSException or InvalidOperationException or JSDisconnectedException or ObjectDisposedException)
        {
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await HideAsync();
        _self?.Dispose();

        if (_module is not null)
        {
            try
            {
                await _module.DisposeAsync();
            }
            catch (Exception exception) when (exception is JSException or JSDisconnectedException or ObjectDisposedException)
            {
            }
        }

        GC.SuppressFinalize(this);
    }
}
