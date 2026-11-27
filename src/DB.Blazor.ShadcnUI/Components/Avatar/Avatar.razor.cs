using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>
/// shadcn/ui <c>Avatar</c> root. It also tracks the image state so <see cref="AvatarFallback"/>
/// behaves like Radix's: the fallback is visible until the image has loaded, and stays visible
/// when the image fails.
/// </summary>
public partial class Avatar : ShadcnComponent
{
    /// <summary>Avatar size; <c>default</c> renders <c>size-8</c>.</summary>
    [Parameter]
    public AvatarSize Size { get; set; } = AvatarSize.Default;

    /// <summary>Avatar content: an <see cref="AvatarImage"/>, an <see cref="AvatarFallback"/> and optionally a badge.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>True once the image has loaded; the fallback is then hidden.</summary>
    public bool ImageLoaded { get; private set; }

    /// <summary>Called by <see cref="AvatarImage"/> when the browser loaded the picture.</summary>
    public void NotifyImageLoaded()
    {
        if (ImageLoaded)
        {
            return;
        }

        ImageLoaded = true;
        StateHasChanged();
    }

    /// <summary>Called by <see cref="AvatarImage"/> when the picture could not be loaded.</summary>
    public void NotifyImageFailed()
    {
        if (!ImageLoaded)
        {
            StateHasChanged();
        }
    }
}
