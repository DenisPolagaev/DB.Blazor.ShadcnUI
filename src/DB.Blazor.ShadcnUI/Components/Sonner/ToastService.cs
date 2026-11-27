using Microsoft.AspNetCore.Components;

namespace DB.Blazor.ShadcnUI.Components;

/// <summary>One toast: shadcn/ui's sonner renders title + description with an optional action.</summary>
public sealed record ToastMessage(Guid Id, string Title, string? Description, string? Action, DateTimeOffset CreatedAt);

/// <summary>
/// The toast queue behind <see cref="Toaster"/>. Register it scoped (<c>AddScoped&lt;ToastService&gt;()</c>)
/// and call <see cref="Show"/> from anywhere.
/// </summary>
public sealed class ToastService
{
    private readonly List<ToastMessage> _toasts = [];

    /// <summary>Raised when the set of visible toasts changes.</summary>
    public event Action? Changed;

    /// <summary>Currently visible toasts, newest first.</summary>
    public IReadOnlyList<ToastMessage> Toasts => _toasts;

    /// <summary>Queues a toast and returns its id.</summary>
    public Guid Show(string title, string? description = null, string? action = null)
    {
        var toast = new ToastMessage(Guid.NewGuid(), title, description, action, DateTimeOffset.UtcNow);
        _toasts.Insert(0, toast);
        Changed?.Invoke();
        return toast.Id;
    }

    /// <summary>Removes a toast by id.</summary>
    public void Dismiss(Guid id)
    {
        if (_toasts.RemoveAll(toast => toast.Id == id) > 0)
        {
            Changed?.Invoke();
        }
    }
}
