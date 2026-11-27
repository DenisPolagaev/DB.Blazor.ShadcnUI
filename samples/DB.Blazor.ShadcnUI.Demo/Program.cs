using DB.Blazor.ShadcnUI.Demo.Client;
using DB.Blazor.ShadcnUI.Demo.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<DB.Blazor.ShadcnUI.Components.ToastService>();

var app = builder.Build();

app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ClientAssemblyMarker).Assembly);

app.Run();
