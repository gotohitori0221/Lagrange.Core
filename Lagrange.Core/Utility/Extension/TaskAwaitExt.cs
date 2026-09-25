using System.Runtime.CompilerServices;

namespace Lagrange.Core.Utility.Extension;

internal static class TaskAwaiters
{
    
    
    
    
    
    
    
    public static ForceAsyncAwaiter ForceAsync(this Task task) => new(task);
}

internal readonly struct ForceAsyncAwaiter : ICriticalNotifyCompletion
{
    private readonly Task _task;

    internal ForceAsyncAwaiter(Task task) => _task = task;

    public ForceAsyncAwaiter GetAwaiter() => this;

    public bool IsCompleted => false; 

    public void GetResult() => _task.GetAwaiter().GetResult();

    public void OnCompleted(Action action) =>
        _task.ConfigureAwait(false).GetAwaiter().OnCompleted(action);

    public void UnsafeOnCompleted(Action action) => 
        _task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(action);
}