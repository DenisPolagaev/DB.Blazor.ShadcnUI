using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>Radix's <c>Menubar</c>: a horizontal bar of menus that share an open state.</summary>
public partial class Menubar : ShadcnComponent, IAsyncDisposable
{
    private readonly List<MenubarMenu> _menus = [];
    private ElementReference _root;
    private IJSObjectReference? _module;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    /// <summary>The bar content: <see cref="MenubarMenu"/> children.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>True when one of the menus is open (the bar then switches menus on hover).</summary>
    internal bool HasOpenMenu => _menus.Any(menu => menu.IsOpen);

    internal void Register(MenubarMenu menu) => _menus.Add(menu);

    internal void Unregister(MenubarMenu menu) => _menus.Remove(menu);

    /// <summary>Closes every menu except <paramref name="menu"/> (one menubar menu is open at a time).</summary>
    internal async Task CloseOthersAsync(MenubarMenu menu)
    {
        foreach (var other in _menus.Where(other => other != menu && other.IsOpen).ToArray())
        {
            await other.CloseAsync();
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            _module ??= await ShadcnJs.ImportAsync(Js);
            await _module.InvokeVoidAsync("bindMenubar", _root);
        }
        catch (Exception exception) when (exception is JSException or InvalidOperationException or JSDisconnectedException or ObjectDisposedException)
        {
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
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

    private string? Classes => MergeClass("cn-menubar flex items-center");
}

