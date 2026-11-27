using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DB.Blazor.ShadcnUI.Demo.Client;

/// <summary>Marker type: the server host uses it to locate the WebAssembly assembly.</summary>
public sealed class ClientAssemblyMarker;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddScoped<DB.Blazor.ShadcnUI.Components.ToastService>();
        await builder.Build().RunAsync();
    }
}
