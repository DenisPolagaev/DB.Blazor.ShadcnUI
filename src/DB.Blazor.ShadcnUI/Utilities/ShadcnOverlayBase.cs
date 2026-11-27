using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DB.Blazor.ShadcnUI;

/// <summary>
/// Base of every overlay panel (dialog, alert dialog, sheet). It owns the element reference, the
/// JS module and the <c>DismissAsync</c> callback the browser invokes on Escape.
/// </summary>
/// <remarks>
/// The component is disposed when the panel closes (<c>@if (root.IsOpen)</c>), so disposal is what
/// unlocks scrolling and restores focus — the same lifecycle Radix gets from unmounting the portal.
/// </remarks>
public abstract class ShadcnOverlayBase : ShadcnComponent, IAsyncDisposable
{
    private IJSObjectReference? _module;
    private DotNetObjectReference<ShadcnOverlayBase>? _self;
    private ElementReference _panel;
    private string? _panelId;
    private bool _attached;
    private bool _disposed;

    /// <summary>Injected JS runtime.</summary>
    [Inject]
    protected IJSRuntime Js { get; set; } = default!;

    /// <summary>Called by the browser when Escape is pressed inside the modal.</summary>
    [JSInvokable]
    public Task DismissAsync() => RequestDismissAsync();

    /// <summary>Closes the overlay (implemented by the panel: it asks its root to close).</summary>
    protected abstract Task RequestDismissAsync();

    /// <summary>
    /// Keeps the JS state in sync with the render: attaches when the panel appears, detaches when
    /// the root closes. The content component itself stays alive while closed (only its markup is
    /// conditional), so <c>firstRender</c> is the wrong hook — this is called on every render.
    /// </summary>
    /// <param name="panel">The rendered panel.</param>
    /// <param name="panelId">The panel's DOM id, used to detach it after the DOM node is gone.</param>
    /// <param name="open">Whether the root is open.</param>
    protected Task SyncAsync(ElementReference panel, string? panelId, bool open) =>
        open ? ShowModalAsync(panel, panelId) : HideModalAsync();

    /// <summary>Registers the panel with the modal stack: focus trap, scroll lock, Escape.</summary>
    protected async Task ShowModalAsync(ElementReference panel, string? panelId = null)
    {
        if (_attached || _disposed)
        {
            return;
        }

        _panel = panel;
        _panelId = panelId;
        _attached = true;

        try
        {
            _module ??= await ShadcnJs.ImportAsync(Js);
            _self ??= DotNetObjectReference.Create(this);
            await _module.InvokeVoidAsync("openModal", panel, _self);
        }
        catch (Exception exception) when (exception is JSException or InvalidOperationException or JSDisconnectedException or ObjectDisposedException)
        {
            // A prerendered page or a shut-down circuit has no DOM to attach to; the markup is
            // still correct, and the panel simply has no JS behaviour for that render.
        }
    }

    /// <summary>Releases the panel: scroll unlock, focus restore.</summary>
    protected async Task HideModalAsync()
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
                // By id, not by ElementReference: the panel node is already gone when a closing
                // component detaches, and resolving a stale reference throws.
                await _module.InvokeVoidAsync("closeModal", _panelId);
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
        await HideModalAsync();
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
