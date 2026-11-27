using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>Radix's <c>MenubarMenu</c>: one menu of the bar, sharing the bar's single open state.</summary>
public partial class MenubarMenu : IDisposable
{
    /// <summary>The menubar that owns this menu.</summary>
    [CascadingParameter]
    public Menubar? Bar { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Bar?.Register(this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Bar?.Unregister(this);
        GC.SuppressFinalize(this);
    }
}
