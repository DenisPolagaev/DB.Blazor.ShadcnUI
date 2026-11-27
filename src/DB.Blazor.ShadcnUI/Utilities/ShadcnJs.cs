using Microsoft.JSInterop;

namespace DB.Blazor.ShadcnUI;

/// <summary>
/// The library's single JS entry point. Overlays, floating panels and focus management are the
/// only behaviours a Blazor component cannot express, and they all live in
/// <c>wwwroot/dp-shadcn.js</c> (one ES module, no global namespace, no third-party runtime).
/// </summary>
internal static class ShadcnJs
{
    /// <summary>Razor-class-library asset path; resolved against the host's base href.</summary>
    public const string ModulePath = "./_content/DB.Blazor.ShadcnUI/dp-shadcn.js";

    public static ValueTask<IJSObjectReference> ImportAsync(IJSRuntime js) =>
        js.InvokeAsync<IJSObjectReference>("import", ModulePath);
}
