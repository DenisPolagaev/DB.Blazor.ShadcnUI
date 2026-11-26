using Microsoft.AspNetCore.Components;

namespace DP.ShadcnBlazor;

/// <summary>
/// Shared open state of a modal root (dialog, alert dialog, sheet): controlled through
/// <c>Open</c>/<c>OpenChanged</c> (<c>@bind-Open</c>), uncontrolled with <c>DefaultOpen</c>.
/// </summary>
public abstract class ShadcnModalRoot : ShadcnComponent
{
    private bool _internalOpen;

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

    /// <summary>True when the panel should be rendered.</summary>
    public bool IsOpen => OpenChanged.HasDelegate ? Open : _internalOpen;

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
