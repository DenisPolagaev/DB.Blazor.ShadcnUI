using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DP.ShadcnBlazor.Demo.Client;

/// <summary>Marker type: the server host uses it to locate the WebAssembly assembly.</summary>
public sealed class ClientAssemblyMarker;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        await builder.Build().RunAsync();
    }
}
